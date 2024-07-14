using System.Data;

namespace BaseRepositoryDapper.Connection;

public interface ISqlConnectionFactory
{
    IDbConnection GetOpenConnection();
}
