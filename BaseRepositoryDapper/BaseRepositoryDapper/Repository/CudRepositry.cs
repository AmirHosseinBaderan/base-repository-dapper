using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BaseRepositoryDapper.Repository;

public class CudRepositry<TModel> : ICudRepository<TModel> where TModel : Entity
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public CudRepositry(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<int> AddAsync(TModel model)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        try
        {
            SqlMapper.AddTypeHandler(new TimeOnlyHandler());
            string sql = BuildAddQuery(model);
            var result = await conn.QueryFirstAsync<int>(sql, model);
            return result;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<long> AddAsync(IEnumerable<TModel> models)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        try
        {
            SqlMapper.AddTypeHandler(new TimeOnlyHandler());
            string sql = BuildAddQuery(models.ToList());
            var result = await conn.ExecuteScalarAsync<long>(sql);
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> DeleteAsync(long id)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        try
        {
            SqlMapper.AddTypeHandler(new TimeOnlyHandler());
            string sql = BuildDeleteQuery();
            var result = await conn.ExecuteAsync(sql, new
            {
                Id = id,
            });
            return result;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<int> UpdateAsync(TModel model)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        try
        {
            string sql = BuildUpdateQuery(model);
            var result = await conn.ExecuteAsync(sql, model);
            return result;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<int> UpdateFieldsAsync(TModel model, params Expression<Func<TModel, object>>[] propertiesToUpdate)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        try
        {
            string sql = BuildUpdateQuery(propertiesToUpdate);
            var result = await conn.ExecuteAsync(sql, model);
            return result;
        }
        catch (Exception)
        {

            throw;
        }
    }

    static string BuildAddQuery(TModel model)
    {
        Type type = model.GetType();

        List<PropertyInfo> properties = type.GetProperties().ToList();
        properties.Remove(properties.FirstOrDefault(p => p.Name == "Id"));
        properties.Remove(properties.FirstOrDefault(p => p.Name == nameof(Entity.DomainEvents)));

        string sql = $"INSERT INTO [dbo].[{type.Name}] (";
        sql += string.Join(",", properties.Select(p => $"[{p.Name}]")) + ") ";
        sql += " VALUES (" + string.Join(",", properties.Select(p => $"@{p.Name}")) + ");";

        sql += " SELECT SCOPE_IDENTITY();";

        return sql;
    }

    static string BuildAddQuery(List<TModel> models)
    {
        if (models == null || !models.Any()) return string.Empty;

        Type type = models.First().GetType();

        List<PropertyInfo> properties = type.GetProperties().ToList();
        properties.Remove(properties.FirstOrDefault(p => p.Name == "Id"));
        properties.Remove(properties.FirstOrDefault(p => p.Name == nameof(Entity.DomainEvents)));

        string sql = $"INSERT INTO [dbo].[{type.Name}] (";
        sql += string.Join(",", properties.Select(p => $"[{p.Name}]")) + ") VALUES ";

        for (int i = 0; i < models.Count; i++)
            sql += "(" + string.Join(",", properties.Select(p => $"{p.GetValue(models[i])}")) + "),";

        sql = sql.TrimEnd(',') + "; SELECT SCOPE_IDENTITY();";

        return sql;
    }

    static string BuildUpdateQuery(params Expression<Func<TModel, object>>[] propertiesToUpdate)
    {
        StringBuilder sql = new();
        sql.Append($"UPDATE [dbo].[{typeof(TModel).Name}] ");
        foreach (var property in propertiesToUpdate)
        {
            var propertyInfo = GetPropertyInfo(property);
            var propertyName = propertyInfo.Name;
            sql.Append($" SET [{propertyName}] = @{propertyName} ");
        }
        sql.Append(" WHERE [Id] = @Id");
        return sql.ToString();
    }

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

    static string BuildUpdateQuery(TModel model)
    {
        Type type = model.GetType();

        List<PropertyInfo> properties = type.GetProperties().ToList();
        properties.Remove(properties.FirstOrDefault(p => p.Name == "Id"));
        properties.Remove(properties.FirstOrDefault(p => p.Name == nameof(Entity.DomainEvents)));

        string sql = $"UPDATE [dbo].[{type.Name}] SET ";
        sql += string.Join(",", properties.Select(p => $"[{p.Name}] = @{p.Name}"));
        sql += " WHERE [Id] = @Id";

        return sql;
    }

    static string BuildDeleteQuery()
    {
        Type type = typeof(TModel);

        string sql = $"DELETE [dbo].[{type.Name}] WHERE [Id] = @Id";
        return sql;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

}
