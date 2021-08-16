using System.Text;

namespace FL.ExpressionToSQL.Formatters
{
    public class MSSQLSchemaFormatter : SchemaFormatter
    {        
        public override string Format(string src)
        {
            return $"[{src}]"; ;
        }

        public override string FormatPage(string src, string orderBy, int skipRecords, int recordsToRead)
        {
            var builder = new StringBuilder(src);
            builder.Append($" Order By {orderBy}");
            builder.Append($" OFFSET {skipRecords} ROWS");
            builder.Append($" FETCH NEXT {recordsToRead} ROWS ONLY");
            return builder.ToString();
        }
    }
}
