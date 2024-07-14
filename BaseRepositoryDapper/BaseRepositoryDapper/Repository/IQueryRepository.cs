using BaseRepositoryDapper.Query;

namespace BaseRepositoryDapper.Repository;

public interface IQueryRepository<TModel> : IDisposable
{
    Task<IEnumerable<TModel>> GetAllAsync();

    Task<IEnumerable<TModel>> GetAllAsync(QueryBuilder<TModel> query);

    Task<IEnumerable<TMapTo>> GetAllAsync<TMapTo>(QueryBuilder<TModel> query);

    Task<IEnumerable<TMapTo>> GetAllAsync<TMapTo>(QueryBuilder<TMapTo> query);

    Task<TModel> FindAsync(object id);

    Task<TModel> FindAsync(QueryBuilder<TModel> query);

    Task<TMapTo> FindAsync<TMapTo>(object id);

    Task<TMapTo> FindAsync<TMapTo>(QueryBuilder<TModel> query);

    Task<TMapTo> FindAsync<TMapTo>(QueryBuilder<TMapTo> query);
}
