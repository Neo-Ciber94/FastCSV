﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FastCSV.Utils
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Whether if the given type is a nullable type.
        /// </summary>
        /// <param name="type">Type yo check.</param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Whether the given type is an array or enumerable.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollectionType(this Type type)
        {
            return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the element type of the given type if is an array or enumerable.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        public static Type? GetCollectionElementType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType()!;
            }

            if (!typeof(IEnumerable).IsAssignableFrom(type) || type.IsGenericTypeDefinition)
            {
                return null;
            }

            var generics = type.GetGenericArguments();

            if (generics.Length == 1)
            {
                return generics[0];
            }

            return typeof(object);
        }

        /// <summary>
        /// Checks if the type inherits from the specified class type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="classType">Class to test against</param>
        /// <returns></returns>
        public static bool IsAssignableToClass(this Type type, Type classType)
        {
            if (classType.IsClass == false)
            {
                return false;
            }

            if (classType.IsGenericTypeDefinition)
            {
                Type? currentType = type.BaseType;

                while (currentType != null)
                {
                    if (currentType.GetGenericTypeDefinition()  == classType)
                    {
                        return true;
                    }

                    currentType = currentType.BaseType;
                }
            }
            else
            {
                Type? currentType = type.BaseType;

                while (currentType != null)
                {
                    if (currentType == classType)
                    {
                        return true;
                    }

                    currentType = currentType.BaseType;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the type inherits from the specified interface type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="interfaceType">Interface to test against</param>
        /// <returns></returns>
        public static bool IsAssignableToInterface(this Type type, Type interfaceType)
        {
            if (interfaceType.IsInterface == false)
            {
                return false;
            }

            if (interfaceType.IsGenericTypeDefinition)
            {
                var interfaces = type.GetInterfaces();

                foreach(var i in interfaces)
                {
                    if (i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                    {
                        return true;
                    }

                    if (i == interfaceType)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return type.GetInterfaces().Any(i => i == interfaceType);
            }

            return false;
        }
    }
}
