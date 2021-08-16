using System;

namespace FL.ExpressionToSQL.ETSAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ETSTableAttribute : Attribute
    {
        public string TableName { set; get; }
    }
}
