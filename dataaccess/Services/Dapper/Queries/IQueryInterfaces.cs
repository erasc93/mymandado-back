using MySql.Data.MySqlClient;
using System.Data;

namespace Services.Dapper.Queries;
public interface IQueries
{
    public IFreeQuery free { get; }
    public ICRUD crud { get; }
    public IBulk bulk { get; }

    public IFreeQueryAsync freeAsync { get; }
    public ICRUDAsync crudAsync { get; }
    public IBulkAsync bulkAsync { get; }

}

public interface IFreeQuery
{
    public IEnumerable<T> Query<T>(string sql, Dictionary<string, object>? param = null, IDbTransaction? transaction = null);
    public void Query(string sql, Dictionary<string, object>? param = null, IDbTransaction? transaction = null);
}
public interface ICRUD
{
    public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null) where T : class;
    public T? GetById<T>(int id, IDbTransaction? transaction = null) where T : class;
    public int? Add<T>(T entityToInsert, IDbTransaction? transaction = null) where T : class;
    public bool Update<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class;
    public bool Delete<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class;

}
public interface IBulk
{
    public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null) where T : class;
    void BulkInsert<T>(IEnumerable<T> elements) where T : class;
    void BulkUpdate<T>(IEnumerable<T> elements) where T : class;
    void BulkSynchronize<T>(IEnumerable<T> elements) where T : class;
    void BulkDelete<T>(IEnumerable<T> elements) where T : class;
    void BulkMerge<T>(IEnumerable<T> elements) where T : class;
}

public interface ICRUDAsync
{
    public Task<IEnumerable<T>> GetAll<T>(IDbTransaction? transaction = null) where T : class;
    public Task<T?> GetById<T>(int id, IDbTransaction? transaction = null) where T : class;
    public Task<int?> Add<T>(T entityToInsert, IDbTransaction? transaction = null) where T : class;
    public Task<bool> Update<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class;
    public Task<bool> Delete<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class;
}
public interface IBulkAsync
{
    Task BulkInsert<T>(IEnumerable<T> elements) where T : class;
    Task BulkUpdate<T>(IEnumerable<T> elements) where T : class;
    Task BulkSynchronize<T>(IEnumerable<T> elements) where T : class;
    Task BulkMerge<T>(IEnumerable<T> elements) where T : class;
    Task BulkDelete<T>(IEnumerable<T> elements) where T : class;
}
public interface IFreeQueryAsync
{
    public Task<IEnumerable<T>> Query<T>(string sql, Dictionary<string, object>? param, IDbTransaction? transaction = null);
    public Task Query(string sql, Dictionary<string, object>? param, IDbTransaction? transaction = null);
}
