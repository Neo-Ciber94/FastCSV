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
        private readonly Dictionary<(Type, BindingFlags), IReadOnlyCollection<MemberInfo>> membersCollections;
        private readonly Dictionary<Type, Type> nullableTypes;

        private CachedReflector()
        {
            constructors = new Dictionary<(Type, Type[]), ConstructorInfo>();
            members = new Dictionary<(Type, string, BindingFlags), MemberInfo>();
            membersCollections = new Dictionary<(Type, BindingFlags), IReadOnlyCollection<MemberInfo>>();
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

            if (!membersCollections.TryGetValue(key, out IReadOnlyCollection<MemberInfo>? values))
            {
                values = type.GetFields(bindingFlags);
                membersCollections.Add(key, values);
            }

            return (IReadOnlyCollection<FieldInfo>)values;
        }

        public IReadOnlyCollection<PropertyInfo> GetProperties(Type type, BindingFlags bindingFlags)
        {
            var key = (type, bindingFlags);

            if (!membersCollections.TryGetValue(key, out IReadOnlyCollection<MemberInfo>? values))
            {
                values = type.GetFields(bindingFlags);
                membersCollections.Add(key, values);
            }

            return (IReadOnlyCollection<PropertyInfo>)values;
        }

        public Type? GetNullableType(Type type)
        {
            if (nullableTypes.TryGetValue(type, out Type? nullableType))
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

    internal readonly struct ReflectorMemberKey
    {
        public Type Type { get; }

        public string Name { get; }

        public BindingFlags BindingFlags { get; }

        public ReflectorMemberKey(Type type, string name, BindingFlags bindingFlags)
        {
            Type = type;
            Name = name;
            BindingFlags = bindingFlags;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReflectorMemberKey key &&
                   EqualityComparer<Type>.Default.Equals(Type, key.Type) &&
                   Name == key.Name &&
                   BindingFlags == key.BindingFlags;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name, BindingFlags);
        }
    }

    internal readonly struct ReflectorCollectionMemberKey
    {
        public Type Type { get; }

        public BindingFlags BindingFlags { get; }

        public ReflectorCollectionMemberKey(Type type, BindingFlags bindingFlags)
        {
            Type = type;
            BindingFlags = bindingFlags;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReflectorCollectionMemberKey key &&
                   EqualityComparer<Type>.Default.Equals(Type, key.Type) &&
                   BindingFlags == key.BindingFlags;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, BindingFlags);
        }
    }

    internal readonly struct ReflectorConstructorKey
    {
  
    }
}
