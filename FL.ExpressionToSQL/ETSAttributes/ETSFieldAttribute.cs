using System;

namespace FL.ExpressionToSQL.ETSAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ETSFieldAttribute : Attribute
    {
        public string FieldName { set; get; }
        public bool IsPrimaryKey { set; get; }
    }
}
