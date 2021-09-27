using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Utils
{
    public static class MemberInfoExtensions
    {
        public static object? GetValue(this MemberInfo memberInfo, object? obj)
        {
            return memberInfo switch
            {
                PropertyInfo p => p.GetValue(obj),
                FieldInfo f => f.GetValue(obj),
                _ => throw new InvalidOperationException($"Cannot get value from {memberInfo}")
            };
        }

        public static void SetValue(this MemberInfo memberInfo, object? obj, object? value)
        {
            switch (memberInfo)
            {
                case PropertyInfo p:
                    p.SetValue(obj, value);
                    break;
                case FieldInfo f:
                    f.SetValue(obj, value);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot get value from {memberInfo}");
            }
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                PropertyInfo p => p.PropertyType,
                FieldInfo f => f.FieldType,
                _ => throw new InvalidOperationException($"Cannot get type from {memberInfo}")
            };
        }
    }
}
