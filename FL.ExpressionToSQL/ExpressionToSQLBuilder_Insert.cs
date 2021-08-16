using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQL.Operations;
using FL.ExpressionToSQL.Utilities;
using System;

namespace FL.ExpressionToSQL
{
    public static partial class ExpressionToSQLBuilder
    {
        public static string BuildInsertStatement<TEntity>(this Object obj, bool insertPrimaryKey, SchemaFormatter schemaFormatter) where TEntity : class
        {
            if (schemaFormatter == null)
                throw new ArgumentNullException($"SchemaFormatter can't be null!");

            const string InsertCommand = "Insert Into {0} ({1}) values ({2});";
            var tableName = AttributeSchemaFormatter.GetTableName<TEntity>(typeof(TEntity), schemaFormatter);
            var insertQuery = InsertCommand.ReplaceOrdinalIgnoreCase("{0}", tableName);

            var fields = AttributeSchemaFormatter.GetEntityFieldsAndValues(obj, insertPrimaryKey, schemaFormatter);

            var columns = string.Join(",", fields.Keys);
            insertQuery = insertQuery.ReplaceOrdinalIgnoreCase("{1}", columns);

            var values = string.Join(",", fields.Values);
            insertQuery = insertQuery.ReplaceOrdinalIgnoreCase("{2}", values);

            return insertQuery;
        }
    }
}
