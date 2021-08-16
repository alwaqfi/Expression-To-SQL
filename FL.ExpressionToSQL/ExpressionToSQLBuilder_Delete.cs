using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQL.Operations;
using FL.ExpressionToSQL.Utilities;
using System;
using System.Linq.Expressions;

namespace FL.ExpressionToSQL
{
    public static partial class ExpressionToSQLBuilder
    {
        public static string BuildDeleteStatement<TEntity>(this Type type, Expression<Func<TEntity, bool>> expression, SchemaFormatter schemaFormatter) where TEntity : class
        {

            if (schemaFormatter == null)
                throw new ArgumentNullException($"SchemaFormatter can't be null!");

            const string DeleteCommand = "Delete From {0}";
            var tableName = AttributeSchemaFormatter.GetTableName<TEntity>(typeof(TEntity), schemaFormatter);
            var deleteQuery = DeleteCommand.ReplaceOrdinalIgnoreCase("{0}", tableName);
            var whereCondition = BuildCondition(type, expression, schemaFormatter);
            deleteQuery = $"{deleteQuery} {whereCondition};";

            return deleteQuery;
        }
    }
}
