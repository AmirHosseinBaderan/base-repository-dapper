# base-repository-dapper
base repository (Crud) for dapper using query builder 

# How use query builder for Dapper

```Csharp
// Create instance of IQueryRepository
IQueryRepository _query = new QueryRepository();

// Query Model for generate query values
Table instance = new(){
  Name = "Amir",
  Age = 22,
}

// Create query
QueryBuilder<Table> query = new QueryBuilder<Table>()
                .Query(instance)
                .Where(t=> t.Name)
                .AndWhere(t=> t.Age);

// Get result
var result = await _query.GetAllAsync(query);

```

#### If you want to get raw sql and use it in Dapper you can try this

```Csharp

// Query Model for generate query values
Table instance = new(){
  Name = "Amir",
  Age = 22,
}

// Create query
QueryBuilder<Table> query = new QueryBuilder<Table>()
                .Query(instance)
                .Where(t=> t.Name)
                .AndWhere(t=> t.Age);

// Raw sql query
string sql = query.Build();

```

#### If you want to get count of items you can use 'Count' instead 'Query' 

``` Csharp
// Create query
QueryBuilder<Table> query = new QueryBuilder<Table>()
                .Count(instance)
                .Where(t=> t.Name)
                .AndWhere(t=> t.Age);
```

### For use 'or' condition in where query can use 'OrWhere' function

``` Csharp
// Create query
QueryBuilder<Table> query = new QueryBuilder<Table>()
                .Count(instance)
                .Where(t=> t.Name)
                .OrWhere(t=> t.Age);
```

## Type of query
``` Csharp
public enum WhereType
{
    Equal,
    NotEqual,
    Is,
    Like,
}
```
#### in where queries can use these types 

``` Csharp
// Create query
QueryBuilder<Table> query = new QueryBuilder<Table>()
                .Count(instance)
                .Where(t=> t.Name,WhereType.Like)
                .OrWhere(t=> t.Age,WhereType.NotEqual);
```
