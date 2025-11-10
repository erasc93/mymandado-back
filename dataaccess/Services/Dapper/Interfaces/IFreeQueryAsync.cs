using System.Data;

namespace Services.Dapper.Interfaces;

public interface IFreeQueryAsync
{
    public Task<IEnumerable<T>> Query<T>(string sql, Dictionary<string, object>? param, IDbConnection? connection = null, IDbTransaction? transaction = null);
    public Task Query(string sql, Dictionary<string, object>? param, IDbConnection? connection = null, IDbTransaction? transaction = null);
}
