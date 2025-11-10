using System.Data;

namespace Services.Dapper.Interfaces;

public interface IFreeQuery
{
    public IEnumerable<T> Query<T>(string sql, Dictionary<string, object>? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
    public void Query(string sql, Dictionary<string, object>? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
}
