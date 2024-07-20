using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BaseRepositoryDapper.Query;
/// <summary>
/// Query builder for dapper on <see cref="TModel"/>
/// </summary>
/// <typeparam name="TModel">Type of model for query</typeparam>
public partial class QueryBuilder<TModel>
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
        _ = CheckType();

        _query += $"SELECT * FROM [dbo].[{_tableName}] ";
        _model = model;
        return this;
    }

    public QueryBuilder<TModel> Count(TModel model)
    {
        _ = CheckType();

        _query += $"SELECT COUNT(*) FROM [dbo].[{_tableName}] ";
        _model = model;
        return this;
    }

    public QueryBuilder<TModel> Where(Expression<Func<TModel, object>> expression, WhereType type = WhereType.Equal)
    {
        PropertyInfo property = GetPropertyInfo(expression);
        object? value = property.GetValue(_model);

        if (value is not null && !string.IsNullOrEmpty(value.ToString()))
        {
            _query += AddCondition("");
            _query += $" [{property.Name}] ";
            _query += BuildQueryValue(type, value);
        }

        return this;
    }

    public QueryBuilder<TModel> OrWhere(Expression<Func<TModel, object>> expression, WhereType type = WhereType.Equal)
    {
        PropertyInfo property = GetPropertyInfo(expression);
        object? value = property.GetValue(_model);

        if (value is not null && !string.IsNullOrEmpty(value.ToString()))
        {
            _query += AddCondition("OR");
            _query += $" [{property.Name}] ";
            _query += BuildQueryValue(type, value);
        }

        return this;
    }

    public QueryBuilder<TModel> AndWhere(Expression<Func<TModel, object>> expression, WhereType type = WhereType.Equal)
    {
        PropertyInfo property = GetPropertyInfo(expression);
        object? value = property.GetValue(_model);

        if (value is not null && !string.IsNullOrEmpty(value.ToString()))
        {
            _query += AddCondition("AND");
            _query += $" [{property.Name}] ";
            _query += BuildQueryValue(type, value);
        }

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
        if (page != 0 && pageSize != 0)
            _query += $" OFFSET {(page - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        return this;
    }

    public string Build()
    {
        _query = RemoveUnusedWhereClauses(_query);
        return _query.Trim();
    }

    // Remove WHERE clauses that don't affect the query
    // Example: "SELECT * FROM MyTable WHERE 1=1 AND ColumnName = 'Value'"
    static string RemoveUnusedWhereClauses(string query) =>
        WhereRegex().Replace(query, "WHERE ");


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

    static string BuildQueryValue(WhereType type, object value)
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

    string AddCondition(string condition)
           => _query.Contains("WHERE") ? $" {condition} " : " WHERE ";

    [GeneratedRegex(@"\bWHERE\s+1=1\s+AND\s+", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex WhereRegex();
}