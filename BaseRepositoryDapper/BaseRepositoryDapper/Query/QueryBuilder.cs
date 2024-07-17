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

    public QueryBuilder(Type tableType) => _tableName = tableType.Name;

    public QueryBuilder()
    {

    }

    public QueryBuilder<TModel> Query(TModel model)
    {
        _ = CheckType();

        _query += $"SELECT * FROM [dbo].[{_tableName}] ";
        _model = model;
        return this;
    }

    public QueryBuilder<TModel> Where(Expression<Func<TModel, object>> expression, WhereType type = WhereType.Equal)
    {
        PropertyInfo property = GetPropertyInfo(expression);
        object? value = property.GetValue(_model);

        if (!_query.Contains("WHERE"))
            _query += " WHERE ";

        _query += $" [{property.Name}] ";
        _query += BuildQueryValue(type, value);

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


    public QueryBuilder<TModel> OrderBy(Expression<Func<TModel, object>> expression, OrderType orderType = OrderType.Non)
    {
        PropertyInfo property = GetPropertyInfo(expression);
        _query += $" ORDER BY [{property.Name}] ";
        _query += orderType switch
        {
            OrderType.DESC => " DESC ",
            OrderType.ASC => " ASC ",
            OrderType.Non => " ",
            _ => "  ",
        };
        return this;
    }

    public QueryBuilder<TModel> WithPagination(int page, int pageSize)
    {
        _query += $" OFFSET {page} FETCH NEXT {pageSize} ROWS ONLY ";
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

    static string FormatValue(object? value)
            => value switch
            {
                null => "",
                string strValue => strValue.Replace("'", "''"),
                DateTime dateTimeValue => $"{dateTimeValue:yyyy-MM-dd HH:mm:ss}",
                _ => value.ToString() ?? ""
            };

    static (string format, string startWith, string endWith) QueryFormat(WhereType type)
        => type switch
        {
            WhereType.Equal => (" = ", "", ""),
            WhereType.NotEqual => (" != ", "", ""),
            WhereType.Is => (" IS ", "", ""),
            WhereType.Like => (" LIKE ", "%", "%"),
            _ => (" = ", "", "")
        };

    static string BuildQueryValue(WhereType type, object? value)
    {
        (string format, string startWith, string endWith) = QueryFormat(type);
        string valueFormat = FormatValue(value);

        return $"{format} N'{startWith}{valueFormat}{endWith}'";
    }

    string CheckType()
    {
        if (string.IsNullOrEmpty(_tableName))
        {
            Type type = typeof(TModel);
            _tableName = type.Name;
        }
        return _tableName;
    }
}