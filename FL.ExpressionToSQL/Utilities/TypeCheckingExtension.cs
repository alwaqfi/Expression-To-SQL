using System;
using System.Collections.Generic;
using System.Text;

namespace FL.ExpressionToSQL.Utilities
{
    internal static class TypeCheckingExtension
    {
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>   {
                typeof(int),  typeof(double),  typeof(decimal),
                typeof(long), typeof(short),   typeof(sbyte),
                typeof(byte), typeof(ulong),   typeof(ushort),
                typeof(uint), typeof(float)    
        };

        public static bool IsNumeric(this Type type)
        {
            return NumericTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);
        }

        public static bool IsBoolean(this Type type)
        {
            return type == typeof(bool) || Nullable.GetUnderlyingType(type) == typeof(bool);
        }
    }
}
