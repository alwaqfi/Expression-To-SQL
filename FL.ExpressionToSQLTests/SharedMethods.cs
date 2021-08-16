using FL.ExpressionToSQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FL.ExpressionToSQLTests
{
    public class SharedMethods
    {
        #region Assertions
        public void AssertBuildSelectStatement<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem) where TEntity : class
        {
            var selectStm = typeof(TEntity).BuildSelectStatement<TEntity>(dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(selectStm));
        }

        public void AssertBuildSelectStatementConditional<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var selectStm = typeof(TEntity).BuildSelectStatement(dataStatusItem.SchemaFormatter, expression);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(selectStm));
        }

        public void AssertBuildUpdateStatement<TEntity>(TEntity entity, SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression, bool includePK) where TEntity : class
        {
            var updateStm = entity.BuildUpdateStatement(expression, includePK, dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(updateStm));
        }

        public void AssertBuildDeleteStatement<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var deleteStm = typeof(TEntity).BuildDeleteStatement(expression, dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(deleteStm));
        }

        public void BuildSelectStatementPagingCondition<TEntity>(SQLSchemaDataStatusItem<TEntity> dataStatusItem, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var selectStm = typeof(TEntity).BuildSelectStatement(dataStatusItem.OrderBy, dataStatusItem.SkipRecords, dataStatusItem.RecordsToRead, dataStatusItem.SchemaFormatter, expression);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(selectStm));
        }

        public void AssertBuildInsertStatement<TEntity>(TEntity entity, SQLSchemaDataStatusItem<TEntity> dataStatusItem, bool includePK) where TEntity : class
        {
            var insertStm = entity.BuildInsertStatement<TEntity>(includePK, dataStatusItem.SchemaFormatter);
            Assert.AreEqual(ClearString(dataStatusItem.SQLStatement), ClearString(insertStm));
        }
        #endregion

        #region Methods
        public Person CreatePersonInstance()
        {
            var person = new Person();
            person.Id = 10;
            person.Age = 20;
            person.Height = new decimal(12.5);
            person.Name = "John Smith";
            person.Address = "20 South, 111 kkk CA";
            person.DateOfBirth = new DateTime(1990, 5, 12, 13, 50, 12);
            person.IsAlive = true;
            person.GenderId = 2;
            return person;
        }

        public string ClearString(string str)
        {
            return str.Replace(" ", "");
        }
        #endregion
    }
}
