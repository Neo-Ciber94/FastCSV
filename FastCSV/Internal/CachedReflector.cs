using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastCSV.Internal
{
    public class CachedReflector : IReflector
    {
        public static CachedReflector Default { get; } = new CachedReflector();

        private readonly Dictionary<(Type, Type[]), ConstructorInfo> constructors = new();
        private readonly Dictionary<(Type, string, BindingFlags), MemberInfo> members = new();
        private readonly Dictionary<(Type, BindingFlags), IReadOnlyCollection<FieldInfo>> fieldsCollection = new();
        private readonly Dictionary<(Type, BindingFlags), IReadOnlyCollection<PropertyInfo>> propertiesCollection = new();
        private readonly Dictionary<MemberInfo, CsvPropertyInfo> csvProperties = new();
        private readonly Dictionary<Type, Type> nullableTypes = new();
        private readonly Dictionary<(MemberInfo, Type), Attribute> memberAttributes = new();
        private readonly Dictionary<Type, bool> readOnlyTypes = new();

        private CachedReflector()
        {
        }

        public ConstructorInfo? GetConstructor(Type type, params Type[] paramsTypes)
        {
            var key = (type, paramsTypes);

            if (!constructors.TryGetValue(key, out ConstructorInfo? constructor))
            {
                constructor = type.GetConstructor(paramsTypes);

                if (constructor == null)
                {
                    return null;
                }

                constructors.Add(key, constructor);
            }

            return constructor;
        }

        public FieldInfo? GetField(Type type, string fieldName, BindingFlags bindingFlags)
        {
            var key = (type, fieldName, bindingFlags);

            if (!members.TryGetValue(key, out MemberInfo? member))
            {
                member = type.GetField(fieldName, bindingFlags);

                if (member == null)
                {
                    return null;
                }

                members.Add(key, member);
            }

            return (FieldInfo?)member;
        }

        public PropertyInfo? GetProperty(Type type, string propertyName, BindingFlags bindingFlags)
        {
            var key = (type, propertyName, bindingFlags);

            if (!members.TryGetValue(key, out MemberInfo? member))
            {
                member = type.GetField(propertyName, bindingFlags);

                if (member == null)
                {
                    return null;
                }

                members.Add(key, member);
            }

            return (PropertyInfo?)member;
        }

        public IReadOnlyCollection<FieldInfo> GetFields(Type type, BindingFlags bindingFlags)
        {
            var key = (type, bindingFlags);

            if (!fieldsCollection.TryGetValue(key, out IReadOnlyCollection<FieldInfo>? values))
            {
                values = type.GetFields(bindingFlags);
                fieldsCollection.Add(key, values);
            }

            return values;
        }

        public IReadOnlyCollection<PropertyInfo> GetProperties(Type type, BindingFlags bindingFlags)
        {
            var key = (type, bindingFlags);

            if (!propertiesCollection.TryGetValue(key, out IReadOnlyCollection<PropertyInfo>? values))
            {
                values = type.GetProperties(bindingFlags);
                propertiesCollection.Add(key, values);
            }

            return values;
        }

        public TAttribute? GetMemberCustomAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            var key = (member, typeof(TAttribute));

            if (!memberAttributes.TryGetValue(key, out Attribute? attribute))
            {
                attribute = member.GetCustomAttribute<TAttribute>();

                // We only cache readonly attributes
                if (IsReadOnlyType(typeof(TAttribute)))
                {
                    memberAttributes.Add(key, attribute!);
                }
            }

            return (TAttribute?)attribute;
        }

        public Type? GetNullableType(Type type)
        {
            if (!nullableTypes.TryGetValue(type, out Type? nullableType))
            {
                nullableType = Nullable.GetUnderlyingType(type);

                if (nullableType != null)
                {
                    nullableTypes.Add(type, nullableType);
                }
            }

            return nullableType;
        }

        public bool IsNullableType(Type nullableType)
        {
            return GetNullableType(nullableType) != null;
        }

        public CsvPropertyInfo GetCsvProperty(MemberInfo member, CsvConverterOptions options)
        {
            if (!csvProperties.TryGetValue(member, out CsvPropertyInfo? property))
            {
                property = CsvConverter.CreateCsvPropertyInfo(member, options);
                csvProperties.Add(member, property);
            }

            return property;
        }

        private bool IsReadOnlyType(Type type)
        {
            if (!readOnlyTypes.TryGetValue(type, out var isReadOnly))
            {
                isReadOnly = IsReadOnlyTypeInternal(type);
                readOnlyTypes.Add(type, isReadOnly);
            }

            return isReadOnly;
        }

        private bool IsReadOnlyTypeInternal(Type type)
        {
            var props = GetProperties(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fields = GetFields(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach(var p in props)
            {
                if (p.CanWrite)
                {
                    return false;
                }
            }

            foreach (var f in fields)
            {
                if (!f.IsInitOnly)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
