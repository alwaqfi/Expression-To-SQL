using FL.ExpressionToSQL.ExpressionTree;
using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQL.Operations;
using System;
using System.Linq.Expressions;

namespace FL.ExpressionToSQL
{
    public static partial class ExpressionToSQLBuilder
    {
        public static string BuildCondition<TEntity>(this Type type, Expression<Func<TEntity, bool>> expression, SchemaFormatter schemaFormatter) where TEntity : class
        {
            if (schemaFormatter == null)
                throw new ArgumentNullException($"SchemaFormatter can't be null!");

            var expressionVisitor = new ETSExpressionVisitor();
            var expressionBinaryTreeTraverser = new ExpressionBinaryTreeTraverser<TEntity>(schemaFormatter);
            ExpressionBinaryTree binaryTree = expressionVisitor.BuildTree(expression);

            var condition = " Where " + expressionBinaryTreeTraverser.Traverse(binaryTree.Root);
            return condition;
        }
    }
}
