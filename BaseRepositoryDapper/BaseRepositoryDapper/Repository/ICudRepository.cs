using System.Linq.Expressions;

namespace BaseRepositoryDapper.Repository;

public interface ICudRepository<TModel> : IDisposable
{
    Task<int> AddAsync(TModel model);
    Task<long> AddAsync(IEnumerable<TModel> models);
    Task<int> UpdateAsync(TModel model);
    Task<int> DeleteAsync(long id);
    Task<int> UpdateFieldsAsync(TModel model, params Expression<Func<TModel, object>>[] propertiesToUpdate);
}
