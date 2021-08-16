using FL.ExpressionToSQL.ExpressionTree;
using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQL.Operations;
using FL.ExpressionToSQL.Utilities;
using System;
using System.Linq.Expressions;

namespace FL.ExpressionToSQL
{
    public static partial class ExpressionToSQLBuilder
    {
        public static string BuildSelectStatement<TEntity>(this Type type, SchemaFormatter schemaFormatter, Expression<Func<TEntity, bool>> expression = null) where TEntity : class
        {
            if (schemaFormatter == null)
                throw new ArgumentNullException($"SchemaFormatter can't be null!");

            var selectQuery = CreateSelectStatement<TEntity>(type, schemaFormatter);
            if (expression != null)
            {
                var whereCondition = BuildCondition<TEntity>(type, expression, schemaFormatter);
                selectQuery = $"{selectQuery} {whereCondition}";
            }

            return selectQuery + ";";
        }
        public static string BuildSelectStatement<TEntity>(this Type type, Expression<Func<TEntity, object>> orderBy, int skipRecords, int recordsToRead, SchemaFormatter schemaFormatter, Expression<Func<TEntity, bool>> expression = null) where TEntity : class
        {
            if (schemaFormatter == null)
                throw new ArgumentNullException($"SchemaFormatter can't be null!");

            var selectQuery = CreateSelectStatement<TEntity>(type, schemaFormatter);
            if (expression != null)
            {
                var whereCondition = BuildCondition<TEntity>(type, expression, schemaFormatter);
                selectQuery = $"{selectQuery} {whereCondition}";
            }

            if (orderBy != null)
            {
                string orderByString = ConvertToString(orderBy, schemaFormatter);
                selectQuery = schemaFormatter.FormatPage(selectQuery, orderByString, skipRecords, recordsToRead);
            }

            return selectQuery + ";";
        }

        private static string ConvertToString<TEntity>(Expression<Func<TEntity, object>> orderBy, SchemaFormatter schemaFormatter) where TEntity : class
        {
            var expressionVisitor = new ETSExpressionVisitor();
            var expressionBinaryTreeTraverser = new ExpressionBinaryTreeTraverser<TEntity>(schemaFormatter);
            ExpressionBinaryTree binaryTree = expressionVisitor.BuildTree(orderBy);
            return expressionBinaryTreeTraverser.Traverse(binaryTree.Root);
        }

        private static string CreateSelectStatement<TEntity>(Type obj, SchemaFormatter schemaFormatter) where TEntity : class
        {
            const string SelectCommand = "Select {0} from {1}";
            var tableFields = AttributeSchemaFormatter.GetEntityFields<TEntity>(schemaFormatter);
            var joinedFields = string.Join(",", tableFields);
            var tableName = AttributeSchemaFormatter.GetTableName<TEntity>(obj, schemaFormatter);
            var selectQuery = SelectCommand
                .ReplaceOrdinalIgnoreCase("{0}", joinedFields)
                .ReplaceOrdinalIgnoreCase("{1}", tableName);
            return selectQuery;
        }
    }
}
