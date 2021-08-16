using FL.ExpressionToSQL.ETSAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FL.ExpressionToSQLTests
{
    [ETSTable(TableName = "Persons")]
    public class Person
    {
        [ETSField(FieldName = "person_id", IsPrimaryKey = true)]
        public int Id { set; get; }
        public string Name { set; get; }
        public decimal Height { set; get; }
        [ETSField(FieldName = "Person Age")]
        public int Age { set; get; }
        public string Address { set; get; }
        public DateTime DateOfBirth { set; get; }
        public Nullable<int> GenderId { get; set; }
        public bool IsAlive { get; set; }
    }
}
