[![CodeQL](https://github.com/alwaqfi/Expression-To-SQL/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/alwaqfi/Expression-To-SQL/actions/workflows/codeql-analysis.yml)

## About

The goal of this library is to convert from .Net to SQL statement that can be executed by the database. The conversion result is useful for libraries requires native SQL statements such as ADO[dot]Net. 
## How it works
Where Condition, Select and Delete commands builders are called through the Entity type.

    //TEntity: class
    typeof(TEntity).BuildCondition(...);
    typeof(TEntity).BuildSelectStatement(...);
    typeof(TEntity).BuildDeleteStatement(...);
While Inset and Update are called through instances

    //instance 
    instance.BuildInsertStatement(...);
    instance.BuildUpdateStatement(...);

## Limitations

 1. It does not support joins
 2. It does not support Bulk operations 

