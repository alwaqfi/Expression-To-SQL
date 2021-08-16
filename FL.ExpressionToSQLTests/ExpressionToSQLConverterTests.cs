using FL.ExpressionToSQL;
using FL.ExpressionToSQL.ETSAttributes;
using FL.ExpressionToSQL.Formatters;
using FL.ExpressionToSQLTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FL.ExpressionToSQL.Tests
{
    [TestClass()]
    public class ExpressionToSQLBuilderTests
    {


        // Issue #7: DateTime type not enclosed by quotes
      

        [TestMethod()]
        public void BuildSelectStatementTest()
        {
            AssertBuildSelectStatement<Person>(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth] from [Persons];" });
            AssertBuildSelectStatement<Person>(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `person_id`,`Name`,`Height`,`Person Age`,`Address`,`DateOfBirth` from `Persons`;" });
            AssertBuildSelectStatement<Human>(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [Id],[First Name],[GenderId],[IsAlive] from [Human];" });
            AssertBuildSelectStatement<Human>(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `Id`,`First Name`,`GenderId`,`IsAlive` from `Human`;" });
        }

        [TestMethod()]
        public void BuildSelectStatementConditionalTest()
        {
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10 && p.Age >= 20;
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth] from [Persons] Where (([Persons].[person_id]=10) and ([Persons].[Person Age]>=20));" }, expp);
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `person_id`,`Name`,`Height`,`Person Age`,`Address`,`DateOfBirth` from `Persons` Where ((`Persons`.`person_id`=10) and (`Persons`.`Person Age`>=20));" }, expp);

            Expression<Func<Human, bool>> exph = (p) => p.Id == 10;
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [Id],[First Name],[GenderId],[IsAlive] from [Human] Where ([Human].[Id]=10);" }, exph);
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `Id`,`First Name`,`GenderId`,`IsAlive` from `Human` Where (`Human`.`Id`=10);" }, exph);
        }

        [TestMethod()]
        public void BuildSelectStatementPagingConditionTest()
        {
            Expression<Func<Human, bool>> exph = (p) => p.Id == 10;
            BuildSelectStatementPagingCondition(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [Id],[First Name],[GenderId],[IsAlive] from [Human] Where ([Human].[Id]=10) Order By [Human].[GenderId] OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY;", OrderBy = p => p.GenderId, SkipRecords = 5, RecordsToRead = 10 }, exph);
            BuildSelectStatementPagingCondition(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `Id`,`First Name`,`GenderId`,`IsAlive` from `Human` Where (`Human`.`Id`=10) Order By `Human`.`GenderId` LIMIT 5,10;", OrderBy = p => p.GenderId, SkipRecords = 5, RecordsToRead = 10 }, exph);
        }

        [TestMethod()]
        public void BuildSelectStatementPagingWithoutConditionTest()
        {
             
            BuildSelectStatementPagingCondition(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [Id],[First Name],[GenderId],[IsAlive] from [Human] Order By [Human].[GenderId] OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY;", OrderBy = p => p.GenderId, SkipRecords = 5, RecordsToRead = 10 }, null);
            BuildSelectStatementPagingCondition(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `Id`,`First Name`,`GenderId`,`IsAlive` from `Human` Order By `Human`.`GenderId` LIMIT 5,10;", OrderBy = p => p.GenderId, SkipRecords = 5, RecordsToRead = 10 }, null);
        }
        [TestMethod()]
        public void BuildSelectStatementConditionalWithVariableTest()
        {
            int localField = 30;

            Expression<Func<Person, bool>> expp = (p) => p.Id == 10 && p.Age < localField;
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth] from [Persons] Where (([Persons].[person_id]=10) and ([Persons].[Person Age]<30));" }, expp);
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `person_id`,`Name`,`Height`,`Person Age`,`Address`,`DateOfBirth` from `Persons` Where ((`Persons`.`person_id`=10) and (`Persons`.`Person Age`<30));" }, expp);

            Expression<Func<Human, bool>> exph = (p) => p.Id == 10 && p.Id < localField;
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [Id],[First Name],[GenderId],[IsAlive] from [Human] Where (([Human].[Id]=10) and ([Human].[Id]<30));" }, exph);
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `Id`,`First Name`,`GenderId`,`IsAlive` from `Human` Where ((`Human`.`Id`=10) and (`Human`.`Id`<30));" }, exph);
        }

        [TestMethod()]
        public void BuildUpdateStatementExcludePKTest()
        {
            var person = CreatePersonInstance();
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10;
            AssertBuildUpdateStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Persons] Set [Name] = 'John Smith',[Height] = 12.5,[Person Age] = 20,[Address]='20 South, 111 kkk CA',[DateOfBirth]='9/7/202012:00:00AM' Where ([Persons].[person_id]=10);" }, expp, false);
            AssertBuildUpdateStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Update `Persons` Set `Name` = 'John Smith',`Height` = 12.5,`Person Age` = 20,`Address`='20 South, 111 kkk CA',`DateOfBirth`='9/7/202012:00:00AM' Where (`Persons`.`person_id`=10);" }, expp, false);

            var human = CreateHumanInstance();
            Expression<Func<Human, bool>> exph = (p) => p.Id == 10;
            AssertBuildUpdateStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Human] Set [Id]=10,[First Name] = 'John Smith', [IsAlive] = True Where ([Human].[Id]=10);" }, exph, false);
            AssertBuildUpdateStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Update `Human` Set `Id`=10,`First Name` = 'John Smith', `IsAlive` = True Where (`Human`.`Id`=10);" }, exph, false);
        }

        [TestMethod()]
        public void BuildUpdateStatementIncludePKTest()
        {
            var person = CreatePersonInstance();
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10;
            AssertBuildUpdateStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Persons] Set [person_id]=10,[Name] = 'John Smith',[Height] = 12.5,[Person Age] = 20,[Address]='20 South, 111 kkk CA',[DateOfBirth]='9/7/202012:00:00AM' Where ([Persons].[person_id]=10);" }, expp, true);
            AssertBuildUpdateStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Update `Persons` Set `person_id`=10,`Name` = 'John Smith',`Height` = 12.5,`Person Age` = 20,`Address`='20 South, 111 kkk CA',`DateOfBirth`='9/7/202012:00:00AM' Where (`Persons`.`person_id`=10);" }, expp, true);

            var human = CreateHumanInstance();
            Expression<Func<Human, bool>> exph = (p) => p.Id == 10;
            AssertBuildUpdateStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Human] Set [Id]=10,[First Name] = 'John Smith', [IsAlive] = True Where ([Human].[Id]=10);" }, exph, true);
            AssertBuildUpdateStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Update `Human` Set `Id`=10,`First Name` = 'John Smith', `IsAlive` = True Where (`Human`.`Id`=10);" }, exph, true);
        }

        [TestMethod()]
        public void BuildDeleteStatementTest()
        {
            Expression<Func<Person, bool>> expp = (p) => p.Id == 10;
            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Delete From [Persons] Where ([Persons].[person_id]=10);" }, expp);
            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Delete From `Persons` Where (`Persons`.`person_id`=10);" }, expp);

            Expression<Func<Human, bool>> exph = (p) => p.Id == 10;
            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Delete From [Human] Where ([Human].[Id]=10);" }, exph);
            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Delete From `Human` Where (`Human`.`Id`=10);" }, exph);
        }

        // Issue #5: Error in Mapping Nullable fields
        [TestMethod()]
        public void BuildStatementNullableFieldsTest()
        {
            int localField = 30;
            var human = CreateHumanInstance();
            Expression<Func<Human, bool>> exph = (p) => p.GenderId == 10;
            Expression<Func<Human, bool>> exph2 = (p) => p.Id == 10 && p.GenderId < localField;

            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Delete From [Human] Where ([Human].[GenderId]=10);" }, exph);
            AssertBuildDeleteStatement(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Delete From `Human` Where (`Human`.`GenderId`=10);" }, exph);

            AssertBuildUpdateStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Update [Human] Set [Id]=10,[First Name] = 'John Smith', [IsAlive] = True Where ([Human].[GenderId]=10);" }, exph, true);
            AssertBuildUpdateStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Update `Human` Set `Id`=10,`First Name` = 'John Smith', `IsAlive` = True Where (`Human`.`GenderId`=10);" }, exph, true);

            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Select [Id],[First Name],[GenderId],[IsAlive] from [Human] Where (([Human].[Id]=10) and ([Human].[GenderId]<30));" }, exph2);
            AssertBuildSelectStatementConditional(new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Select `Id`,`First Name`,`GenderId`,`IsAlive` from `Human` Where ((`Human`.`Id`=10) and (`Human`.`GenderId`<30));" }, exph2);
        }

        [TestMethod()]
        public void BuildInsertStatementExcludePKTest()
        {
            var person = CreatePersonInstance();
            AssertBuildInsertStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Insert Into [Persons] ([Name],[Height],[Person Age],[Address],[DateOfBirth]) values ('John Smith',12.5,20,'20 South, 111 kkk CA','9/7/202012:00:00AM');" }, false);
            AssertBuildInsertStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Insert Into `Persons` (`Name`,`Height`,`Person Age`,`Address`,`DateOfBirth`) values ('John Smith',12.5,20,'20 South, 111 kkk CA','9/7/202012:00:00AM');" }, false);

            var human = CreateHumanInstance();
            AssertBuildInsertStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Insert Into [Human] ([Id],[First Name],[IsAlive]) values (10,'John Smith',True);" }, false);
            AssertBuildInsertStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Insert Into `Human` (`Id`,`First Name`,`IsAlive`) values (10,'John Smith',True);" }, false);
        }

        [TestMethod()]
        public void BuildInsertStatementIncludePKTest()
        {
            var person = CreatePersonInstance();
            AssertBuildInsertStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Insert Into [Persons] ([person_id],[Name],[Height],[Person Age],[Address],[DateOfBirth]) values (10,'John Smith',12.5,20,'20 South, 111 kkk CA','9/7/202012:00:00AM');" }, true);
            AssertBuildInsertStatement(person, new SQLSchemaDataStatusItem<Person> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Insert Into `Persons` (`person_id`,`Name`,`Height`,`Person Age`,`Address`,`DateOfBirth`) values (10,'John Smith',12.5,20,'20 South, 111 kkk CA','9/7/202012:00:00AM');" }, true);

            var human = CreateHumanInstance();
            AssertBuildInsertStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MSSQLSchemaFormatter(), SQLStatement = "Insert Into [Human] ([Id],[First Name],[IsAlive]) values (10,'John Smith',True);" }, true);
            AssertBuildInsertStatement(human, new SQLSchemaDataStatusItem<Human> { SchemaFormatter = new MySQLSchemaFormatter(), SQLStatement = "Insert Into `Human` (`Id`,`First Name`,`IsAlive`) values (10,'John Smith',True);" }, true);
        }

        #region Assertions
        private void AssertBuildSelectStatement<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem) where TEntity : class
        {
            var selectStm = typeof(TEntity).BuildSelectStatement<TEntity>(dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(selectStm));
        }

        private void AssertBuildSelectStatementConditional<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var selectStm = typeof(TEntity).BuildSelectStatement(dataStatusItem.SchemaFormatter, expression);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(selectStm));
        }

        private void AssertBuildUpdateStatement<TEntity>(TEntity entity, SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression, bool includePK) where TEntity : class
        {
            var updateStm = entity.BuildUpdateStatement(expression, includePK, dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(updateStm));
        }

        private void AssertBuildDeleteStatement<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var deleteStm = typeof(TEntity).BuildDeleteStatement(expression, dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(deleteStm));
        }

        private void BuildSelectStatementPagingCondition<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var selectStm = typeof(TEntity).BuildSelectStatement(dataStatusItem.OrderBy, dataStatusItem.SkipRecords, dataStatusItem.RecordsToRead, dataStatusItem.SchemaFormatter, expression);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(selectStm));
        }

        private void AssertBuildInsertStatement<TEntity>(TEntity entity, SQLSchemaDataStatusItem<TEntity> dataStatusItem, bool includePK) where TEntity : class
        {
            var insertStm = entity.BuildInsertStatement<TEntity>(includePK, dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(insertStm));
        }
        #endregion

        #region Methods
        private Person CreatePersonInstance()
        {
            var person = new Person();
            person.Id = 10;
            person.Age = 20;
            person.Height = new decimal(12.5);
            person.Name = "John Smith";
            person.Address = "20 South, 111 kkk CA";
            person.DateOfBirth = new DateTime(1990, 5, 12, 13, 50, 12);
            return person;
        }
        private Human CreateHumanInstance()
        {
            var human = new Human();
            human.Id = 10;
            human.Name = "John Smith";
            human.IsAlive = true;
            return human;
        }
        private string ClearString(string str)
        {
            return str.Replace(" ", "");
        }
        #endregion
    }
}