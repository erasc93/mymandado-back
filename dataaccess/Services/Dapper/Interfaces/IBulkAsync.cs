using System.Data;

namespace Services.Dapper.Interfaces;

public interface IBulkAsync
{
    Task BulkInsert<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkUpdate<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkSynchronize<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkDelete<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkMerge<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
}
