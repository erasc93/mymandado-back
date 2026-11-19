using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Services.Dapper.Interfaces;

public interface IFreeQueryAsync
{
    public Task<IEnumerable<T>> Query<T>(string sql, Dictionary<string, object>? param);
    public Task Query(string sql, Dictionary<string, object>? param);
}
