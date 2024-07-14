using System.Linq.Expressions;
using System.Reflection;

namespace BaseRepositoryDapper.Query;


/// <summary>
/// Query builder for dapper on <see cref="TModel"/>
/// </summary>
/// <typeparam name="TModel">Type of model for query</typeparam>
public class QueryBuilder<TModel>
{
    string _query = "";

    TModel? _model;

    string _tableName = "";

    public QueryBuilder(Type tableType)
    {
        _tableName = tableType.Name;
    }

    public QueryBuilder()
    {

    }

    public QueryBuilder<TModel> Query(TModel model)
    {
        var type = typeof(TModel);
        if (string.IsNullOrEmpty(_tableName))
            _tableName = type.Name;

        _query += $"SELECT * FROM [dbo].[{_tableName}] ";
        _model = model;
        return this;
    }

    public QueryBuilder<TModel> Where(Expression<Func<TModel, object>> expression, WhereType type = WhereType.Equal)
    {
        var property = GetPropertyInfo(expression);
        object? value = property.GetValue(_model);

        if (!_query.Contains("WHERE"))
            _query += " WHERE ";

        _query += $" [{property.Name}] ";
        _query += type switch
        {
            WhereType.Equal => " = ",
            WhereType.NotEqual => " != ",
            WhereType.Is => " IS ",
            WhereType.Like => " LIKE ",
            _ => " = "
        };
        _query += FormatValue(value ?? "");

        return this;
    }

    public QueryBuilder<TModel> And()
    {
        _query += " AND ";
        return this;
    }

    public QueryBuilder<TModel> Or()
    {
        _query += " OR ";
        return this;
    }

    public string Build()
        => _query.ToString();

    static PropertyInfo GetPropertyInfo<T, P>(Expression<Func<T, P>> property)
    {
        if (property.Body is UnaryExpression unaryExp)
        {
            if (unaryExp.Operand is MemberExpression memberExp)
                return (PropertyInfo)memberExp.Member;
        }
        else if (property.Body is MemberExpression memberExp)
            return (PropertyInfo)memberExp.Member;


        throw new ArgumentException($"The expression doesn't indicate a valid property. [ {property} ]");
    }

    static string FormatValue(object value)
            => value switch
            {
                string strValue => $"N'{strValue.Replace("'", "''")}'",
                DateTime dateTimeValue => $"'{dateTimeValue:yyyy-MM-dd HH:mm:ss}'",
                _ => value.ToString() ?? ""
            };
}

public enum WhereType
{
    Equal,
    NotEqual,
    Is,
    Like,
}

