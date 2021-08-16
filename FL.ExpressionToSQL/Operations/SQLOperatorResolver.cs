using System.Linq.Expressions;

namespace FL.ExpressionToSQL.Operations
{
    internal static class SQLOperatorResolver
    {
        public static string Resolve(ExpressionType expressionType)
        {
            var resovedType = "";
            switch (expressionType)
            {
                case ExpressionType.Add:
                    resovedType = "+";
                    break;
                case ExpressionType.AndAlso:
                    resovedType = "and";
                    break;
                case ExpressionType.Divide:
                    resovedType = "/";
                    break;
                case ExpressionType.Equal:
                    resovedType = "=";
                    break;
                case ExpressionType.GreaterThan:
                    resovedType = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    resovedType = ">=";
                    break;
                case ExpressionType.LessThan:
                    resovedType = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    resovedType = "<=";
                    break;
                case ExpressionType.Negate:
                    resovedType = "NEGATE";
                    break;
                case ExpressionType.NotEqual:
                    resovedType = "<>";
                    break;
                case ExpressionType.OrElse:
                    resovedType = "or";
                    break;
                case ExpressionType.Subtract:
                    resovedType = "-";
                    break;
                case ExpressionType.AddAssign:
                    resovedType = "+=";
                    break;
                case ExpressionType.AndAssign:
                    resovedType = "&=";
                    break;
                case ExpressionType.DivideAssign:
                    resovedType = "/=";
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    resovedType = "^-=";
                    break;
                case ExpressionType.MultiplyAssign:
                    resovedType = "*=";
                    break;
                case ExpressionType.OrAssign:
                    resovedType = "|*=";
                    break;
                case ExpressionType.SubtractAssign:
                    resovedType = "-=";
                    break;
                case ExpressionType.AddAssignChecked:
                    resovedType = "+=";
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    resovedType = "*=";
                    break;
                case ExpressionType.SubtractAssignChecked:
                    resovedType = "-=";
                    break;
                case ExpressionType.And:
                    resovedType = "&";
                    break;
                case ExpressionType.Or:
                    resovedType = "|";
                    break;
                case ExpressionType.ExclusiveOr:
                    resovedType = "^";
                    break;
            }

            return resovedType;
        }
    }
}
