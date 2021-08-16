using System;
using System.Collections.Generic;
using System.Reflection;

namespace FL.ExpressionToSQL.ETSAttributes
{
    internal static class ETSAttributeReader
    {
        public static TAttribute GetAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetCustomAttribute<TAttribute>();
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type, string propName)
            where TAttribute : Attribute
        {
            return type.GetProperty(propName).GetCustomAttribute<TAttribute>();
        }

        public static Dictionary<PropertyInfo, object> GetPropertiesAndValues<TEntity>(this Type type, TEntity entity)
            where TEntity : class
        {
            var dic = new Dictionary<PropertyInfo, object>();
            var properties = entity.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var fieldValue = properties[i].GetValue(entity);
                dic.Add(properties[i], fieldValue);
            }
            return dic;
        }
    }
}
