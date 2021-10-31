using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastCSV.Internal
{
    public interface IReflector
    {
        public PropertyInfo? GetProperty(Type type, string propertyName, BindingFlags bindingFlags);

        public IReadOnlyCollection<PropertyInfo> GetProperties(Type type, BindingFlags bindingFlags);

        public FieldInfo? GetField(Type type, string fieldName, BindingFlags bindingFlags);

        public IReadOnlyCollection<FieldInfo> GetFields(Type type, BindingFlags bindingFlags);

        public ConstructorInfo? GetConstructor(Type type, params Type[] paramsTypes);

        public TAttribute? GetMemberCustomAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute;

        public Type? GetNullableType(Type type);

        public bool IsNullableType(Type nullableType);

        public CsvPropertyInfo GetCsvProperty(MemberInfo member, CsvConverterOptions options);
    }
}
