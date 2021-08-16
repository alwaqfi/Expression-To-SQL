using FL.ExpressionToSQL.ExpressionTree;
using FL.ExpressionToSQL.Formatters;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FL.ExpressionToSQL.Operations
{
    internal class ExpressionBinaryTreeTraverser<TEntity> where TEntity : class
    {
        private readonly SchemaFormatter _schemaFormatter;

        public ExpressionBinaryTreeTraverser(SchemaFormatter schemaFormatter)
        {
            _schemaFormatter = schemaFormatter;
        }
        public string Traverse(ExpressionTreeNode node)
        {
            if (node.Left != null && node.Right != null)
                return $"({Traverse(node.Left)} {SQLOperatorResolver.Resolve(node.ExpressionType)} {Traverse(node.Right)})";
            else if (node.Left != null)
            {
                return Traverse(node.Left);
            }
            else
                return ParseExpression(node.Node);
        }
        private string ParseExpression(Expression exp)
        {
            var val = "";
            switch (exp.NodeType)
            {

                case ExpressionType.MemberAccess:
                    val= ParseNode(exp as MemberExpression);
                    break;
                case ExpressionType.Constant:
                    val = ParseNode(exp as ConstantExpression);
                    break;
                case ExpressionType.Convert:
                    val =  ParseNode(exp as UnaryExpression);
                    break;                
            }
            return val;
        }
        private string ParseNode(UnaryExpression node)
        {
            return ParseExpression(node.Operand);
        }
        private string ParseNode(MemberExpression node)
        {
            var str = "";
            if (node.Expression == null)
            {
                str = ParseNode(node, node.Member);
            } 
            else if (node.Expression.NodeType == ExpressionType.Parameter)
            {
                var tableName = ParseNode(node.Expression as ParameterExpression);

                str = $"{tableName}.{AttributeSchemaFormatter.GetFieldName(node.Member, _schemaFormatter)}";
            }
            else if (node.Expression.NodeType == ExpressionType.Constant)
            {
                var constExpVal = ((ConstantExpression)node.Expression).Value;
                str = ParseNode(node, constExpVal);
            }
            else
            {
                // Do nothing
            }

            return str;
        }
        private string ParseNode(MemberExpression node, object constExpVal)
        {
            string str = "";
            object objVal = ParseMemberInfo(constExpVal, node.Member);
            if (objVal != null)
            {
                var constVal = Expression.Constant(objVal);
                str = AttributeSchemaFormatter.FormatValue(constVal.Type, constVal.Value.ToString(), _schemaFormatter);
            }

            return str;
        }
        private string ParseNode(ConstantExpression node)
        {
            return node.Value.ToString();
        }
        private string ParseNode(ParameterExpression node)
        {
            return AttributeSchemaFormatter.GetTableName<TEntity>(node.Type, _schemaFormatter);
        }

        // This is needed to get the value of local fields 
        private static object ParseMemberInfo(object constantExp, MemberInfo memberExp)
        {
            object objVal = null;
            if (memberExp is FieldInfo)
            {
                objVal = ((FieldInfo)memberExp).GetValue(constantExp);
            }
            if (memberExp is PropertyInfo)
            {
                objVal = ((PropertyInfo)memberExp).GetValue(constantExp, null);
            }

            return objVal;
        }
    }
}
