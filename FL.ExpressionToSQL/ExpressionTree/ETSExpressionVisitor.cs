/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

/* ****************************************************************************
 *  
 *  Modifications copyright (C) 2020 <Asif AlWaqfi>
 *
 * ***************************************************************************/

using FL.ExpressionToSQL.ExpressionTree;
using System;
using System.Linq.Expressions;


namespace FL.ExpressionToSQL.ExpressionTree
{
    internal class ETSExpressionVisitor : ExpressionVisitor
    {
        private ExpressionBinaryTree _binaryTree = new ExpressionBinaryTree();

        public ExpressionBinaryTree BuildTree<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            Visit(expression);
            return _binaryTree;
        }

        public ExpressionBinaryTree BuildTree<TEntity>(Expression<Func<TEntity, object>> expression) where TEntity : class
        {
            Visit(expression);
            return _binaryTree;
        }
        /// <summary>
        /// Visits the children of the <see cref="BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            // Walk children in evaluation order: left, conversion, right
            _binaryTree.AddNode(node, node.Left, node.Right);
            return ValidateBinary(
                node,
                node.Update(
                    Visit(node.Left),
                    VisitAndConvert(node.Conversion, "VisitBinary"),
                    Visit(node.Right)
                )
            );
        }

        /// <summary>
        /// Visits the <see cref="ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            _binaryTree.AddNode(node, null, null);
            return node;
        }

        /// <summary>
        /// Visits the <see cref="DefaultExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitDefault(DefaultExpression node)
        {
            _binaryTree.AddNode(node, null, null);
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="Expression&lt;T&gt;" />.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _binaryTree.AddNode(node, node.Body, null);
            return node.Update(Visit(node.Body), VisitAndConvert(node.Parameters, "VisitLambda"));
        }        

        /// <summary>
        /// Visits the children of the <see cref="MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
           _binaryTree.AddNode(node, null, null);
            return node.Update(node.Expression);
        }

        /// <summary>
        /// Visits the <see cref="ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
           // _binaryTree.AddNode(node, null, null);
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="TypeBinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            _binaryTree.AddNode(node, node.Expression, null);
            return node.Update(Visit(node.Expression));
        }

        /// <summary>
        /// Visits the children of the <see cref="ElementInit" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return node.Update(Visit(node.Arguments));
        }

        /// <summary>
        /// Visits the children of the <see cref="UnaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            return ValidateUnary(node, node.Update(Visit(node.Operand)));
        }
        //
        // Prevent some common cases of invalid rewrites.
        //
        // Essentially, we don't want the rewritten node to be semantically
        // bound by the factory, which may do the wrong thing. Instead we
        // require derived classes to be explicit about what they want to do if
        // types change.
        //
        private static UnaryExpression ValidateUnary(UnaryExpression before, UnaryExpression after)
        {
            if (before != after && before.Method == null)
            {
                if (after.Method != null)
                {
                    throw new Exception($"{after.Method}, VisitUnary");
                }

                // rethrow has null operand
                if (before.Operand != null && after.Operand != null)
                {
                    ValidateChildType(before.Operand.Type, after.Operand.Type, "VisitUnary");
                }
            }
            return after;
        }

        private static BinaryExpression ValidateBinary(BinaryExpression before, BinaryExpression after)
        {
            if (before != after && before.Method == null)
            {
                if (after.Method != null)
                {
                    throw new Exception($"{after.Method}, VisitBinary");
                }

                ValidateChildType(before.Left.Type, after.Left.Type, "VisitBinary");
                ValidateChildType(before.Right.Type, after.Right.Type, "VisitBinary");
            }
            return after;
        }

        // Value types must stay as the same type, otherwise it's now a
        // different operation, e.g. adding two doubles vs adding two ints.
        private static void ValidateChildType(Type before, Type after, string methodName)
        {
            if (before.IsValueType)
            {
                if (AreEquivalent(before, after))
                {
                    // types are the same value type
                    return;
                }
            }
            else if (!after.IsValueType)
            {
                // both are reference types
                return;
            }

            // Otherwise, it's an invalid type change.
            throw new Exception($"{before},{after}, {methodName}");
        }

        private static bool AreEquivalent(Type t1, Type t2)
        {
            return t1 == t2 || t1.IsEquivalentTo(t2);
        }

    }
}
