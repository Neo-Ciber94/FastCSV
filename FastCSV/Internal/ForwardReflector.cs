using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastCSV.Internal
{
    public class ForwardReflector : IReflector
    {
        public static ForwardReflector Default { get; } = new ForwardReflector();

        private ForwardReflector() { }

        public FieldInfo? GetField(Type type, string fieldName, BindingFlags bindingFlags)
        {
            return type.GetField(fieldName, bindingFlags);
        }

        public IReadOnlyCollection<FieldInfo> GetFields(Type type, BindingFlags bindingFlags)
        {
            return type.GetFields(bindingFlags);
        }

        public ConstructorInfo? GetConstructor(Type type, params Type[] paramsTypes)
        {
            return type.GetConstructor(paramsTypes);
        }

        public IReadOnlyCollection<PropertyInfo> GetProperties(Type type, BindingFlags bindingFlags)
        {
            return type.GetProperties(bindingFlags);
        }

        public PropertyInfo? GetProperty(Type type, string propertyName, BindingFlags bindingFlags)
        {
            return type.GetProperty(propertyName, bindingFlags);
        }

        public bool IsNullableType(Type type)
        {
            return GetNullableType(type) != null;
        }

        public Type? GetNullableType(Type nullableType)
        {
            return Nullable.GetUnderlyingType(nullableType);
        }
    }
}
