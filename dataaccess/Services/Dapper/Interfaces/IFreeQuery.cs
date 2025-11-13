using System.Data;

namespace Services.Dapper.Interfaces;

public interface IFreeQuery
{
    public IEnumerable<T> Query<T>(string sql, Dictionary<string, object>? param = null);
    public void Query(string sql, Dictionary<string, object>? param = null);
}
