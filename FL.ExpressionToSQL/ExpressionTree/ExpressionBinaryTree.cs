using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FL.ExpressionToSQL.ExpressionTree
{
    internal class ExpressionBinaryTree
    {    
        private ExpressionTreeNode _root;
        private Dictionary<Expression, ExpressionTreeNode> _temp = new Dictionary<Expression, ExpressionTreeNode>();

        public ExpressionTreeNode Root => _root;

        public void AddNode(Expression expression, Expression left, Expression right)
        {
            var key = _temp.Keys.FirstOrDefault(k => k == expression);

            if (key == null)
            {
                AddNewBranch(expression, left, right);
            }
            else
            {
                UpdateExistingBranch(left, right, key);
            }
        }

        private void UpdateExistingBranch(Expression left, Expression right, Expression key)
        {
            ExpressionTreeNode val;

            if (_temp.TryGetValue(key, out val))
            {
                if (left != null)
                {
                    var nl = new ExpressionTreeNode(left);
                    val.Left = nl;
                    if (!_temp.ContainsKey(left))
                        _temp.Add(left, nl);
                }

                if (right != null)
                {
                    var nr = new ExpressionTreeNode(right);
                    val.Right = nr;
                    if (!_temp.ContainsKey(right))
                        _temp.Add(right, nr);
                }
            }
        }

        private void AddNewBranch(Expression expression, Expression left, Expression right)
        {

            var newNode = new ExpressionTreeNode(expression);
            if (_root == null)
                _root = newNode;
            _temp.Add(expression, newNode);

            if (left != null)
            {
                AddLeft(left, newNode);
            }

            if (right != null)
            {
                AddRight(right, newNode);
            }
        }

        private void AddRight(Expression right, ExpressionTreeNode newNode)
        {
            var nr = new ExpressionTreeNode(right);
            newNode.Right = nr;
            _temp.Add(right, nr);
        }

        private void AddLeft(Expression left, ExpressionTreeNode newNode)
        {
            var nl = new ExpressionTreeNode(left);
            newNode.Left = nl;
            _temp.Add(left, nl);
        }
    }
}
