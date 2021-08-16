namespace FL.ExpressionToSQL.Formatters
{
    public abstract class SchemaFormatter
    {
        public virtual string FormatValue(string src)
        {
            return $"'{src}'";
        }
        public abstract string Format(string src);
        public abstract string FormatPage(string src, string orderBy, int skipRecords, int recordsToRead);
    }
}
