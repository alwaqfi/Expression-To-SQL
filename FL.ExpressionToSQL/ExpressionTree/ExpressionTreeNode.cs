using System.Linq.Expressions;

namespace FL.ExpressionToSQL.ExpressionTree
{
    internal class ExpressionTreeNode
    {
        public Expression Node { set; get; }
        public ExpressionTreeNode Left { set; get; }
        public ExpressionTreeNode Right { set; get; }

        public ExpressionTreeNode(Expression expression)
        {
            Node = expression;
        }

        public ExpressionType ExpressionType
        {
            get
            {
                return Node.NodeType;
            }
        }      
    }
}
