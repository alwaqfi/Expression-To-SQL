using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQL.Operations;
using FL.ExpressionToSQL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FL.ExpressionToSQL
{
    public static partial class ExpressionToSQLBuilder
    {
        public static string BuildUpdateStatement<TEntity>(this Object obj, Expression<Func<TEntity, bool>> expression, bool updatePrimaryKey, SchemaFormatter schemaFormatter) where TEntity : class
        {
            if (schemaFormatter == null)
                throw new ArgumentNullException($"SchemaFormatter can't be null!");

            const string UpdateCommand = "Update {0} Set {1}";
            var tableName = AttributeSchemaFormatter.GetTableName<TEntity>(typeof(TEntity), schemaFormatter);
            var updateQuery = UpdateCommand.ReplaceOrdinalIgnoreCase("{0}", tableName);

            var fields = AttributeSchemaFormatter.GetEntityFieldsAndValues(obj, updatePrimaryKey, schemaFormatter);
            var setValues = BuildUpdateSetValues(fields);
            var columns = string.Join(",", setValues);
            updateQuery = updateQuery.ReplaceOrdinalIgnoreCase("{1}", columns);
            var whereCondition = BuildCondition(obj.GetType(), expression, schemaFormatter);
            updateQuery = $"{updateQuery} {whereCondition};";
            return updateQuery;
        }

        private static List<string> BuildUpdateSetValues(Dictionary<string, string> fields)
        {
            var setValues = new List<string>();
            foreach (var kv in fields)
            {
                setValues.Add(kv.Key + "=" + kv.Value);
            }
            return setValues;
        }
    }
}
