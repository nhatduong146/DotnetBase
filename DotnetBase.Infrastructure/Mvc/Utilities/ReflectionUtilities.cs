﻿namespace DotnetBase.Infrastructure.Mvc.Utilities
{
    using Common.Constants;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The Reflection Class
    /// </summary>
    public static class ReflectionUtilities
    {
        /// <summary>
        /// Get all properties from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetAllPropertiesOfType(Type type)
        {
            return type.GetProperties().ToList();
        }

        /// <summary>
        /// Get all property name from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetAllPropertyNamesOfType(Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Get  assemblies
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetAssemblies()
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            return currentAssemblies.Where(a => a.FullName.Contains(GlobalConstants.ASSEMBLY_NAME)).ToArray();
        }

        public static TValue GetAttributValue<TAttribute, TValue>(this PropertyInfo prop, Func<TAttribute, TValue> value) where TAttribute : Attribute
        {
            var att = prop.GetCustomAttributes(
                typeof(TAttribute), true
                ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return value(att);
            }
            return default(TValue);
        }
    }
}