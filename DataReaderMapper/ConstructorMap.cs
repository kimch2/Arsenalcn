﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DataReaderMapper.Internal;

namespace DataReaderMapper
{
    public class ConstructorMap
    {
        private static readonly DelegateFactory DelegateFactory = new DelegateFactory();
        private readonly ILazy<LateBoundParamsCtor> _runtimeCtor;

        public ConstructorMap(ConstructorInfo ctor, IEnumerable<ConstructorParameterMap> ctorParams)
        {
            Ctor = ctor;
            CtorParams = ctorParams;

            _runtimeCtor = LazyFactory.Create(() => DelegateFactory.CreateCtor(ctor, CtorParams));
        }

        public ConstructorInfo Ctor { get; }
        public IEnumerable<ConstructorParameterMap> CtorParams { get; }

        public Expression NewExpression(Expression instanceParameter)
        {
            var parameters = CtorParams.Select(map => map.GetExpression(instanceParameter));
            return Expression.New(Ctor, parameters);
        }

        public object ResolveValue(ResolutionContext context, IMappingEngineRunner mappingEngine)
        {
            var ctorArgs = new List<object>();

            foreach (var map in CtorParams)
            {
                var result = map.ResolveValue(context);

                var sourceType = result.Type;
                var destinationType = map.Parameter.ParameterType;

                var typeMap = mappingEngine.ConfigurationProvider.ResolveTypeMap(result, destinationType);

                var targetSourceType = typeMap != null ? typeMap.SourceType : sourceType;

                var newContext = context.CreateTypeContext(typeMap, result.Value, null, targetSourceType,
                    destinationType);

                if (typeMap == null && map.Parameter.IsOptional)
                {
                    var value = map.Parameter.DefaultValue;
                    ctorArgs.Add(value);
                }
                else
                {
                    var value = mappingEngine.Map(newContext);
                    ctorArgs.Add(value);
                }
            }

            return _runtimeCtor.Value(ctorArgs.ToArray());
        }
    }
}