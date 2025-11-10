using System.Data;

namespace Services.Dapper.Queries;

// --- --- ---
public interface ITransactionQueries
{
    public ITransacFreeQuery free { get; }
    public ITransacCRUD crud { get; }
    public ITransacBulk bulk { get; }
    public ITransacFreeQueryAsync freeAsync { get; }
    public ITransacCRUDAsync crudAsync { get; }
    public ITransacBulkAsync bulkAsync { get; }
    void ExecuteInTransaction(Action<IDbConnection, IDbTransaction> action);
}

public interface ITransacFreeQuery
{
    public IEnumerable<T> Query<T>(string sql, Dictionary<string, object>? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
    public void Query(string sql, Dictionary<string, object>? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
}
public interface ITransacCRUD
{
    public IEnumerable<T> GetAll<T>(IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public T? GetById<T>(int id, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public int? Add<T>(T entityToInsert, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public bool Update<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public bool Delete<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;

}
public interface ITransacBulk
{
    public IEnumerable<T> GetAll<T>(IDbConnection? connection = null) where T : class;
    void BulkInsert<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkUpdate<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkSynchronize<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkDelete<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    void BulkMerge<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
}
public interface ITransacCRUDAsync
{
    public Task<IEnumerable<T>> GetAll<T>(IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<T?> GetById<T>(int id, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<int?> Add<T>(T entityToInsert, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<bool> Update<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<bool> Delete<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
}
public interface ITransacBulkAsync
{
    Task BulkInsert<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkUpdate<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkSynchronize<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkDelete<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
    Task BulkMerge<T>(IEnumerable<T> elements, IDbConnection? connection = null) where T : class;
}
public interface ITransacFreeQueryAsync
{
    public Task<IEnumerable<T>> Query<T>(string sql, Dictionary<string, object>? param, IDbConnection? connection = null, IDbTransaction? transaction = null);
    public Task Query(string sql, Dictionary<string, object>? param, IDbConnection? connection = null, IDbTransaction? transaction = null);
}
