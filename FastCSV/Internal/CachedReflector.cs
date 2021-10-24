using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastCSV.Internal
{
    public class CachedReflector : IReflector
    {
        public static CachedReflector Default { get; } = new CachedReflector();

        private readonly Dictionary<(Type, Type[]), ConstructorInfo> constructors;
        private readonly Dictionary<(Type, string, BindingFlags), MemberInfo> members;
        private readonly Dictionary<(Type, BindingFlags), IReadOnlyCollection<FieldInfo>> fieldsCollection;
        private readonly Dictionary<(Type, BindingFlags), IReadOnlyCollection<PropertyInfo>> propertiesCollection;
        private readonly Dictionary<Type, Type> nullableTypes;

        private CachedReflector()
        {
            constructors = new Dictionary<(Type, Type[]), ConstructorInfo>();
            members = new Dictionary<(Type, string, BindingFlags), MemberInfo>();
            fieldsCollection = new Dictionary<(Type, BindingFlags), IReadOnlyCollection<FieldInfo>>();
            propertiesCollection = new Dictionary<(Type, BindingFlags), IReadOnlyCollection<PropertyInfo>>();
            nullableTypes = new Dictionary<Type, Type>();
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
    }
}
