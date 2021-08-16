using FL.ExpressionToSQL;
using FL.ExpressionToSQL.ETSAttributes;
using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQLTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FL.ExpressionToSQLTests
{
    [TestClass]
    public class MSSQLConnectionUnitTests : SharedMethods
    {
 
        [TestMethod()]
        public void BuildSelectStatementTest()
        {
            AssertBuildSelectStatement<Person>(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive] from [Persons];" });
        }

        [TestMethod()]
        public void BuildSelectStatement_ConditionalTest()
        {
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10 && p.Age >= 20;

            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive] from [Persons] Where (([Persons].[person_id]=10) and ([Persons].[Person Age]>=20));" }, expp);
        }

        [TestMethod()]
        public void BuildSelectStatement_Paging_WithConditionTest()
        {
            Expression<Func<Person, bool>> exph = (p) => p.Id == 10;

            BuildSelectStatementPagingCondition(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive] from [Persons] Where ([Persons].[person_id]=10) Order By [Persons].[GenderId] OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY;", OrderBy = p => p.GenderId, SkipRecords = 5, RecordsToRead = 10 }, exph);
        }

        [TestMethod()]
        public void BuildSelectStatement_Paging_WithoutConditionTest()
        {
            BuildSelectStatementPagingCondition(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive] from [Persons] Order By [Persons].[GenderId] OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY;", OrderBy = p => p.GenderId, SkipRecords = 5, RecordsToRead = 10 }, null);
        }

        [TestMethod()]
        public void BuildSelectStatement_Conditional_WithVariableTest()
        {
            int localField = 30;
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10 && p.Age < localField;

            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive] from [Persons] Where (([Persons].[person_id]=10) and ([Persons].[Person Age]<30));" }, expp);
        }

        [TestMethod()]
        public void BuildUpdateStatementExcludePKTest()
        {
            var person = CreatePersonInstance();
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10;

            AssertBuildUpdateStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Persons] Set [Name] = 'John Smith',[Height] = 12.5,[Person Age] = 20,[Address]='20 South, 111 kkk CA',[DateOfBirth]='5/12/19901:50:12PM', [GenderId]=2, [IsAlive]=True Where ([Persons].[person_id]=10);" }, expp, false);
        }

        [TestMethod()]
        public void BuildUpdateStatement_IncludePKTest()
        {
            var person = CreatePersonInstance();
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10;

            AssertBuildUpdateStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Persons] Set [person_id]=10,[Name] = 'John Smith',[Height] = 12.5,[Person Age] = 20,[Address]='20 South, 111 kkk CA',[DateOfBirth]='5/12/19901:50:12PM', [GenderId]=2,[IsAlive] = True Where ([Persons].[person_id]=10);" }, expp, true);
        }

        [TestMethod()]
        public void BuildDeleteStatementTest()
        {
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10;

            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Delete From [Persons] Where ([Persons].[person_id]=10);" }, expp);
        }

        // Issue #5: Error in Mapping Nullable fields
        [TestMethod()]
        public void BuildSDeletetatement_NullableFieldsTest()
        {
            int localField = 30;  
            Expression<Func<Person, bool>> exph2 = (p) => p.Id == 10 && p.GenderId < localField;

            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Delete From [Persons] Where (([Persons].[person_id]=10) and ([Persons].[GenderId]<30));" }, exph2);
        }
         
        [TestMethod()]
        public void BuildUpdateStatement_NullableFieldsTest()
        {
            int localField = 30;
            var person = CreatePersonInstance();
            Expression<Func<Person, bool>> exph2 = (p) => p.Id == 10 && p.GenderId < localField;
 
            AssertBuildUpdateStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Persons] Set [person_id]=10,[Name] = 'John Smith',[Height] = 12.5,[Person Age] = 20,[Address]='20 South, 111 kkk CA',[DateOfBirth]='5/12/19901:50:12PM', [GenderId]=2,[IsAlive] = True Where (([Persons].[person_id]=10) and ([Persons].[GenderId] < 30));" }, exph2, true);
        }
         
        [TestMethod()]
        public void BuildSelectStatement_NullableFieldsTest()
        {
            int localField = 30;
            Expression<Func<Person, bool>> exph2 = (p) => p.Id == 10 && p.GenderId < localField;
            
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive] from [Persons] Where (([Persons].[person_id]=10) and ([Persons].[GenderId]<30));" }, exph2);
        }
         
        [TestMethod()]
        public void BuildInsertStatement_ExcludePKTest()
        {
            var person = CreatePersonInstance();
            AssertBuildInsertStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Insert Into [Persons] ([Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive]) values ('John Smith',12.5,20,'20 South, 111 kkk CA','5/12/19901:50:12PM',2,True);" }, false);
        }

        [TestMethod()]
        public void BuildInsertStatement_IncludePKTest()
        {
            var person = CreatePersonInstance();
            AssertBuildInsertStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Insert Into [Persons] ([person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth],[GenderId],[IsAlive]) values (10,'John Smith',12.5,20,'20 South, 111 kkk CA','5/12/19901:50:12PM',2,True);" }, true);
        }
    
    }
}
