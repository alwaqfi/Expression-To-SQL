using FL.ExpressionToSQL.ETSAttributes;
using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQL.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FL.ExpressionToSQL.Operations
{

    internal static class AttributeSchemaFormatter
    {
        public static string GetTableName<TEntity>(Type type, SchemaFormatter schemaFormatter) where TEntity : class
        {
            var attr = typeof(TEntity).GetAttribute<ETSTableAttribute>();
            return schemaFormatter.Format(IfNullElse(attr, attr?.TableName, typeof(TEntity).Name));
        }
        public static string GetTableName<TEntity>(this Object obj, SchemaFormatter schemaFormatter) where TEntity : class
        {
            var attr = typeof(TEntity).GetAttribute<ETSTableAttribute>();
            return schemaFormatter.Format(IfNullElse(attr, attr?.TableName, typeof(TEntity).Name));
        }
        public static Dictionary<string, string> GetEntityFieldsAndValues<TEntity>(TEntity entity, bool includePrimaryKey, SchemaFormatter schemaFormatter) where TEntity : class
        {
            var dic = new Dictionary<string, string>();
            var properties = typeof(TEntity).GetPropertiesAndValues(entity);
            bool isPK;

            foreach (var key in properties.Keys)
            {
                var fieldName = GetFieldName(key, schemaFormatter, out isPK);
                if (includePrimaryKey || (!includePrimaryKey && !isPK))
                {

                    var fieldValue = FormatValue(key.PropertyType, key.GetValue(entity), schemaFormatter);
                    if (!string.IsNullOrEmpty(fieldValue))
                        dic.Add(fieldName, fieldValue);
                }
            }
            return dic;
        }

        public static List<string> GetEntityFields<TEntity>(SchemaFormatter schemaFormatter) where TEntity : class
        {
            var lst = new List<string>();
            var properties = typeof(TEntity).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var fieldName = GetFieldName(properties[i], schemaFormatter);
                lst.Add(fieldName);
            }
            return lst;
        }

        public static string FormatValue(Type type, object fieldValue, SchemaFormatter schemaFormatter)
        {
            if (fieldValue == null)
                return string.Empty;
            else if (!type.IsNumeric() && !type.IsBoolean())
                return schemaFormatter.FormatValue(fieldValue.ToString());
            else
                return fieldValue.ToString();
        }

        public static string GetFieldName(MemberInfo memberInfo, SchemaFormatter schemaFormatter, out bool isKey)
        {
            var attr = memberInfo.GetCustomAttribute<ETSFieldAttribute>();
            isKey = attr != null ? attr.IsPrimaryKey : false;
            return schemaFormatter.Format(IfNullElse(attr, attr?.FieldName, memberInfo?.Name));
        }
        public static string GetFieldName(MemberInfo memberInfo, SchemaFormatter schemaFormatter)
        {
            var attr = memberInfo.GetCustomAttribute<ETSFieldAttribute>();
            return schemaFormatter.Format(IfNullElse(attr, attr?.FieldName, memberInfo?.Name));
        }

        private static string FormatValue(PropertyInfo prop, string fieldValue, SchemaFormatter schemaFormatter)
        {
            if (!prop.PropertyType.IsNumeric() && !prop.PropertyType.IsBoolean())
                return schemaFormatter.FormatValue(fieldValue);
            else
                return fieldValue;
        }

        #region Supportive methods


        private static string IfNullElse(object obj, string ifNotNull, string ifNull)
        {
            string val = "";

            if (obj != null && ifNotNull != null)
            {
                val = ifNotNull;
            }
            else
            {
                val = ifNull;
            }

            return val;
        }
        #endregion
    }
}
