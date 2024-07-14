using BaseRepositoryDapper.Connection;
using BaseRepositoryDapper.Query;

namespace BaseRepositoryDapper.Repository;

public class QueryRepository<TModel> : IQueryRepository<TModel>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public QueryRepository(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public async Task<TModel> FindAsync(object id)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = $"SELECT * FROM {TableName()} WHERE [Id] = @Id";
        return await conn.QueryFirstOrDefaultAsync<TModel>(sql, new
        {
            Id = id,
        });
    }

    public async Task<TMapTo> FindAsync<TMapTo>(object id)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = $"SELECT * FROM {TableName()} WHERE [Id] = @Id";
        return await conn.QueryFirstOrDefaultAsync<TMapTo>(sql, new
        {
            Id = id,
        });
    }

    public async Task<TModel> FindAsync(QueryBuilder<TModel> query)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = query.Build();
        return await conn.QueryFirstOrDefaultAsync<TModel>(sql);
    }

    public async Task<TMapTo> FindAsync<TMapTo>(QueryBuilder<TModel> query)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = query.Build();
        return await conn.QueryFirstOrDefaultAsync<TMapTo>(sql);
    }

    public async Task<TMapTo> FindAsync<TMapTo>(QueryBuilder<TMapTo> query)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = query.Build();
        return await conn.QueryFirstOrDefaultAsync<TMapTo>(sql);
    }

    public async Task<IEnumerable<TModel>> GetAllAsync()
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = $"SELECT * FROM {TableName()}";
        return await conn.QueryAsync<TModel>(sql);
    }

    public async Task<IEnumerable<TModel>> GetAllAsync(QueryBuilder<TModel> query)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = query.Build();
        return await conn.QueryAsync<TModel>(sql);
    }

    public async Task<IEnumerable<TMapTo>> GetAllAsync<TMapTo>(QueryBuilder<TModel> query)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = query.Build();
        return await conn.QueryAsync<TMapTo>(sql);
    }

    public async Task<IEnumerable<TMapTo>> GetAllAsync<TMapTo>(QueryBuilder<TMapTo> query)
    {
        var conn = _sqlConnectionFactory.GetOpenConnection();
        string sql = query.Build();
        return await conn.QueryAsync<TMapTo>(sql);
    }

    public string TableName()
    {
        Type type = typeof(TModel);
        return $"[dbo].[{type.Name}]";
    }
}
