using System;

namespace FL.ExpressionToSQL.Utilities
{
    internal static class StringExtensions
    {
        public static string ReplaceOrdinalIgnoreCase(this string str, string oldVal, string newVal)
        {

#if NET472
            return str?.Replace(oldVal, newVal);
#else
            return str?.Replace(oldVal, newVal, StringComparison.OrdinalIgnoreCase);
#endif
        }
    }
}
