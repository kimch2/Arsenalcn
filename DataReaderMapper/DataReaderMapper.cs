﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using DataReaderMapper.Internal;
using DataReaderMapper.Mappers;

#if DOTNET
    using IDataRecord = System.Data.Common.DbDataReader;
    using IDataReader = System.Data.Common.DbDataReader;
#endif

namespace DataReaderMapper
{
    public class DataReaderMapper : IObjectMapper
    {
        private static readonly ConcurrentDictionary<BuilderKey, Build> _builderCache =
            new ConcurrentDictionary<BuilderKey, Build>();

        private static readonly ConcurrentDictionary<Type, CreateEnumerableAdapter> _enumerableAdapterCache =
            new ConcurrentDictionary<Type, CreateEnumerableAdapter>();

        private static readonly MethodInfo getValueMethod = typeof (IDataRecord).GetMethod("get_Item",
            new[] {typeof (int)});

        private static readonly MethodInfo isDBNullMethod = typeof (IDataRecord).GetMethod("IsDBNull",
            new[] {typeof (int)});

        static DataReaderMapper()
        {
            FeatureDetector.IsIDataRecordType = t => typeof (IDataRecord).IsAssignableFrom(t);
        }

        public bool YieldReturnEnabled { get; set; }

        public object Map(ResolutionContext context, IMappingEngineRunner mapper)
        {
            if (IsDataReader(context))
            {
                var destinationElementType = TypeHelper.GetElementType(context.DestinationType);
                var results = MapDataReaderToEnumerable(context, mapper, destinationElementType, YieldReturnEnabled);

                if (YieldReturnEnabled)
                {
                    var adapterBuilder = GetDelegateToCreateEnumerableAdapter(destinationElementType);
                    return adapterBuilder(results);
                }

                return results;
            }

            if (IsDataRecord(context))
            {
                var dataRecord = context.SourceValue as IDataRecord;
                var buildFrom = CreateBuilder(context.DestinationType, dataRecord);
                var result = buildFrom(dataRecord);
                MapPropertyValues(context, mapper, result);
                return result;
            }

            return null;
        }

        public bool IsMatch(ResolutionContext context)
        {
            return IsDataReader(context) || IsDataRecord(context);
        }

        private static IEnumerable MapDataReaderToEnumerable(ResolutionContext context, IMappingEngineRunner mapper,
            Type destinationElementType, bool useYieldReturn)
        {
            var dataReader = (IDataReader) context.SourceValue;
            var resolveUsingContext = context;

            if (context.TypeMap == null)
            {
                var configurationProvider = mapper.ConfigurationProvider;
                var typeMap = configurationProvider.FindTypeMapFor(context.SourceType, destinationElementType);
                resolveUsingContext = new ResolutionContext(typeMap, context.SourceValue, context.SourceType,
                    destinationElementType, new MappingOperationOptions(), (IMappingEngine) mapper);
            }

            var buildFrom = CreateBuilder(destinationElementType, dataReader);

            if (useYieldReturn)
                return LoadDataReaderViaYieldReturn(dataReader, mapper, buildFrom, resolveUsingContext);

            return LoadDataReaderViaList(dataReader, mapper, buildFrom, resolveUsingContext, destinationElementType);
        }

        private static IEnumerable LoadDataReaderViaList(IDataReader dataReader, IMappingEngineRunner mapper,
            Build buildFrom, ResolutionContext resolveUsingContext, Type elementType)
        {
            var list = ObjectCreator.CreateList(elementType);

            while (dataReader.Read())
            {
                var result = buildFrom(dataReader);
                MapPropertyValues(resolveUsingContext, mapper, result);
                list.Add(result);
            }

            return list;
        }

        private static IEnumerable LoadDataReaderViaYieldReturn(IDataReader dataReader, IMappingEngineRunner mapper,
            Build buildFrom, ResolutionContext resolveUsingContext)
        {
            while (dataReader.Read())
            {
                var result = buildFrom(dataReader);
                MapPropertyValues(resolveUsingContext, mapper, result);
                yield return result;
            }
        }

        private static bool IsDataReader(ResolutionContext context)
        {
            return typeof (IDataReader).IsAssignableFrom(context.SourceType) &&
                   context.DestinationType.IsEnumerableType();
        }

        private static bool IsDataRecord(ResolutionContext context)
        {
            return typeof (IDataRecord).IsAssignableFrom(context.SourceType);
        }

        private static Build CreateBuilder(Type destinationType, IDataRecord dataRecord)
        {
            Build builder;
            var builderKey = new BuilderKey(destinationType, dataRecord);
            if (_builderCache.TryGetValue(builderKey, out builder))
            {
                return builder;
            }

            var method = new DynamicMethod("DynamicCreate", destinationType, new[] {typeof (IDataRecord)},
                destinationType, true);
            var generator = method.GetILGenerator();

            var result = generator.DeclareLocal(destinationType);
            generator.Emit(OpCodes.Newobj, destinationType.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                var propertyInfo = destinationType.GetProperty(dataRecord.GetName(i),
                    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var endIfLabel = generator.DefineLabel();

                if (propertyInfo != null && propertyInfo.GetSetMethod(true) != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);

                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);

                    if (propertyInfo.PropertyType.IsGenericType()
                        && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof (Nullable<>))
                        )
                    {
                        var nullableType = propertyInfo.PropertyType.GetGenericTypeDefinition().GetGenericArguments()[0];
                        if (!nullableType.IsEnum())
                            generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                        else
                        {
                            generator.Emit(OpCodes.Unbox_Any, nullableType);
                            generator.Emit(OpCodes.Newobj, propertyInfo.PropertyType);
                        }
                    }
                    else
                    {
                        generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));
                    }
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod(true));

                    generator.MarkLabel(endIfLabel);
                }
            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            builder = (Build) method.CreateDelegate(typeof (Build));
            _builderCache[builderKey] = builder;
            return builder;
        }

        private static void MapPropertyValues(ResolutionContext context, IMappingEngineRunner mapper, object result)
        {
            if (context.TypeMap == null)
                throw new AutoMapperMappingException(context, "Missing type map configuration or unsupported mapping.");

            foreach (var propertyMap in context.TypeMap.GetPropertyMaps())
            {
                MapPropertyValue(context, mapper, result, propertyMap);
            }
        }

        private static void MapPropertyValue(ResolutionContext context, IMappingEngineRunner mapper,
            object mappedObject, PropertyMap propertyMap)
        {
            if (!propertyMap.CanResolveValue())
                return;

            var result = propertyMap.ResolveValue(context);
            var newContext = context.CreateMemberContext(null, result.Value, null, result.Type, propertyMap);

            if (!propertyMap.ShouldAssignValue(newContext))
                return;

            try
            {
                var propertyValueToAssign = mapper.Map(newContext);

                if (propertyMap.CanBeSet)
                    propertyMap.DestinationProperty.SetValue(mappedObject, propertyValueToAssign);
            }
            catch (AutoMapperMappingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AutoMapperMappingException(newContext, ex);
            }
        }

        private static CreateEnumerableAdapter GetDelegateToCreateEnumerableAdapter(Type elementType)
        {
            CreateEnumerableAdapter builder;
            if (_enumerableAdapterCache.TryGetValue(elementType, out builder))
            {
                return builder;
            }

            var adapterType = typeof (EnumerableAdapter<>).MakeGenericType(elementType);
            var adapterCtor = adapterType.GetConstructor(new[] {typeof (IEnumerable)});
            var adapterCtorArg = Expression.Parameter(typeof (IEnumerable), "items");
            var adapterCtorExpression = Expression.New(adapterCtor, adapterCtorArg);
            builder =
                (CreateEnumerableAdapter)
                    Expression.Lambda(typeof (CreateEnumerableAdapter), adapterCtorExpression, adapterCtorArg).Compile();

            _enumerableAdapterCache[elementType] = builder;
            return builder;
        }

        private delegate object Build(IDataRecord dataRecord);

        private delegate object CreateEnumerableAdapter(IEnumerable items);

        private class BuilderKey
        {
            private readonly List<string> _dataRecordNames;
            private readonly Type _destinationType;

            public BuilderKey(Type destinationType, IDataRecord record)
            {
                _destinationType = destinationType;
                _dataRecordNames = new List<string>(record.FieldCount);
                for (var i = 0; i < record.FieldCount; i++)
                {
                    _dataRecordNames.Add(record.GetName(i));
                }
            }

            public override int GetHashCode()
            {
                var hash = _destinationType.GetHashCode();
                foreach (var name in _dataRecordNames)
                {
                    hash = hash*37 + name.GetHashCode();
                }
                return hash;
            }

            public override bool Equals(object obj)
            {
                var builderKey = obj as BuilderKey;
                if (builderKey == null)
                    return false;

                if (_dataRecordNames.Count != builderKey._dataRecordNames.Count)
                    return false;

                if (_destinationType != builderKey._destinationType)
                    return false;

                for (var i = 0; i < _dataRecordNames.Count; i++)
                {
                    if (_dataRecordNames[i] != builderKey._dataRecordNames[i])
                        return false;
                }
                return true;
            }
        }

        private class EnumerableAdapter<Item> : IEnumerable<Item>
        {
            private readonly IEnumerable<Item> _items;

            public EnumerableAdapter(IEnumerable items)
            {
                _items = items.Cast<Item>();
            }

            public IEnumerator<Item> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

    internal static class ReflectionExtensions
    {
        public static bool IsEnum(this Type type)
        {
#if DOTNET
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if DOTNET
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }
    }
}