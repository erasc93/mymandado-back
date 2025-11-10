using System.Data;

namespace Services.Dapper.Interfaces;

public interface IBulk
{
    public IEnumerable<T> GetAll<T>(IDbConnection? connection = null) where T : class;
    void BulkInsert<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkUpdate<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkSynchronize<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkDelete<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkMerge<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
}
