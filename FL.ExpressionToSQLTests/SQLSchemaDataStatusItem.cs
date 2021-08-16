using FL.ExpressionToSQL.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FL.ExpressionToSQLTests
{
    public class SQLSchemaDataStatusItem<TEnitty> where TEnitty : class
    {
        public SchemaFormatter SchemaFormatter { set; get; }
        public string SQLStatement { set; get; }
        public Expression<Func<TEnitty, object>> OrderBy { set; get; }
        public int SkipRecords { set; get; }
        public int RecordsToRead { set; get; }
    }
}
