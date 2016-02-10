using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DataReaderMapper.Internal;

namespace DataReaderMapper
{
    /// <summary>
    ///     Contains cached reflection information for easy retrieval
    /// </summary>
    [DebuggerDisplay("{Type}")]
    public class TypeDetails
    {
        public TypeDetails(Type type)
            : this(type, _ => true, _ => true, new MethodInfo[0])
        {
        }

        public TypeDetails(Type type, Func<PropertyInfo, bool> shouldMapProperty, Func<FieldInfo, bool> shouldMapField)
            : this(type, shouldMapProperty, shouldMapField, new MethodInfo[0])
        {
        }

        public TypeDetails(Type type, Func<PropertyInfo, bool> shouldMapProperty, Func<FieldInfo, bool> shouldMapField,
            IEnumerable<MethodInfo> sourceExtensionMethodSearch)
        {
            Type = type;
            var membersToMap = MembersToMap(shouldMapProperty, shouldMapField);
            var publicReadableMembers = GetAllPublicReadableMembers(membersToMap);
            var publicWritableMembers = GetAllPublicWritableMembers(membersToMap);
            PublicReadAccessors = BuildPublicReadAccessors(publicReadableMembers);
            PublicWriteAccessors = BuildPublicAccessors(publicWritableMembers);
            PublicNoArgMethods = BuildPublicNoArgMethods();
            Constructors = TypeExtensions.GetDeclaredConstructors(type).Where(ci => !ci.IsStatic).ToArray();
            PublicNoArgExtensionMethods = BuildPublicNoArgExtensionMethods(sourceExtensionMethodSearch);
        }

        public Type Type { get; }

        public IEnumerable<ConstructorInfo> Constructors { get; }

        public IEnumerable<MemberInfo> PublicReadAccessors { get; }

        public IEnumerable<MemberInfo> PublicWriteAccessors { get; }

        public IEnumerable<MethodInfo> PublicNoArgMethods { get; }

        public IEnumerable<MethodInfo> PublicNoArgExtensionMethods { get; }

        private Func<MemberInfo, bool> MembersToMap(Func<PropertyInfo, bool> shouldMapProperty,
            Func<FieldInfo, bool> shouldMapField)
        {
            return m =>
            {
                var property = m as PropertyInfo;
                if (property != null)
                {
                    return !TypeExtensions.IsStatic(property) && shouldMapProperty(property);
                }
                var field = (FieldInfo) m;
                return !field.IsStatic && shouldMapField(field);
            };
        }

        private IList<MethodInfo> BuildPublicNoArgExtensionMethods(IEnumerable<MethodInfo> sourceExtensionMethodSearch)
        {
            var sourceExtensionMethodSearchArray = sourceExtensionMethodSearch.ToArray();

            var explicitExtensionMethods = sourceExtensionMethodSearchArray
                .Where(method => method.GetParameters()[0].ParameterType == Type)
                .ToList();

            var genericInterfaces = Type.GetInterfaces().Where(t => TypeExtensions.IsGenericType(t)).ToList();

            if (TypeExtensions.IsInterface(Type) && TypeExtensions.IsGenericType(Type))
                genericInterfaces.Add(Type);

            explicitExtensionMethods.AddRange(
                from method in sourceExtensionMethodSearchArray.Where(method => method.IsGenericMethodDefinition)
                let parameterType = method.GetParameters()[0].ParameterType
                let interfaceMatch = genericInterfaces
                    .Where(
                        t => TypeExtensions.GetGenericParameters(t).Length == parameterType.GetGenericArguments().Length)
                    .FirstOrDefault(
                        t =>
                            method.MakeGenericMethod(t.GetGenericArguments()).GetParameters()[0].ParameterType
                                .IsAssignableFrom(t))
                where interfaceMatch != null
                select method.MakeGenericMethod(interfaceMatch.GetGenericArguments()));

            return explicitExtensionMethods;
        }

        private static MemberInfo[] BuildPublicReadAccessors(IEnumerable<MemberInfo> allMembers)
        {
            // Multiple types may define the same property (e.g. the class and multiple interfaces) - filter this to one of those properties
            var filteredMembers = allMembers
                .OfType<PropertyInfo>()
                .GroupBy(x => x.Name) // group properties of the same name together
                .Select(x => x.First())
                .OfType<MemberInfo>() // cast back to MemberInfo so we can add back FieldInfo objects
                .Concat(allMembers.Where(x => x is FieldInfo)); // add FieldInfo objects back

            return filteredMembers.ToArray();
        }

        private static MemberInfo[] BuildPublicAccessors(IEnumerable<MemberInfo> allMembers)
        {
            // Multiple types may define the same property (e.g. the class and multiple interfaces) - filter this to one of those properties
            var filteredMembers = allMembers
                .OfType<PropertyInfo>()
                .GroupBy(x => x.Name) // group properties of the same name together
                .Select(x =>
                    x.Any(y => y.CanWrite && y.CanRead)
                        ? // favor the first property that can both read & write - otherwise pick the first one
                        x.First(y => y.CanWrite && y.CanRead)
                        : x.First())
                .Where(pi => pi.CanWrite || PrimitiveExtensions.IsListOrDictionaryType(pi.PropertyType))
                .OfType<MemberInfo>() // cast back to MemberInfo so we can add back FieldInfo objects
                .Concat(allMembers.Where(x => x is FieldInfo)); // add FieldInfo objects back

            return filteredMembers.ToArray();
        }

        private IEnumerable<MemberInfo> GetAllPublicReadableMembers(Func<MemberInfo, bool> membersToMap)
        {
            return GetAllPublicMembers(PropertyReadable, FieldReadable, membersToMap);
        }

        private IEnumerable<MemberInfo> GetAllPublicWritableMembers(Func<MemberInfo, bool> membersToMap)
        {
            return GetAllPublicMembers(PropertyWritable, FieldWritable, membersToMap);
        }

        private static bool PropertyReadable(PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead;
        }

        private bool FieldReadable(FieldInfo fieldInfo)
        {
            return true;
        }

        private static bool PropertyWritable(PropertyInfo propertyInfo)
        {
            var propertyIsEnumerable = (typeof (string) != propertyInfo.PropertyType)
                                       && typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);

            return propertyInfo.CanWrite || propertyIsEnumerable;
        }

        private bool FieldWritable(FieldInfo fieldInfo)
        {
            return !fieldInfo.IsInitOnly;
        }

        private IEnumerable<MemberInfo> GetAllPublicMembers(
            Func<PropertyInfo, bool> propertyAvailableFor,
            Func<FieldInfo, bool> fieldAvailableFor,
            Func<MemberInfo, bool> memberAvailableFor)
        {
            var typesToScan = new List<Type>();
            for (var t = Type; t != null; t = TypeExtensions.BaseType(t))
                typesToScan.Add(t);

            if (TypeExtensions.IsInterface(Type))
                typesToScan.AddRange(Type.GetInterfaces());

            // Scan all types for public properties and fields
            return typesToScan
                .Where(x => x != null) // filter out null types (e.g. type.BaseType == null)
                .SelectMany(x => TypeExtensions.GetDeclaredMembers(x)
                    .Where(mi => mi.DeclaringType != null && mi.DeclaringType == x)
                    .Where(
                        m =>
                            (m is FieldInfo && fieldAvailableFor((FieldInfo) m)) ||
                            (m is PropertyInfo && propertyAvailableFor((PropertyInfo) m) &&
                             !((PropertyInfo) m).GetIndexParameters().Any()))
                    .Where(memberAvailableFor)
                );
        }

        private MethodInfo[] BuildPublicNoArgMethods()
        {
            return TypeExtensions.GetAllMethods(Type)
                .Where(mi => mi.IsPublic && !mi.IsStatic && mi.DeclaringType != typeof (object))
                .Where(m => (m.ReturnType != typeof (void)) && (m.GetParameters().Length == 0))
                .ToArray();
        }
    }
}