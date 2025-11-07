using MySql.Data.MySqlClient;
using System.Data;

namespace Services.Dapper;

public interface IFreeQuery
{
    public IEnumerable<T> Query<T>(string sql);
    public void Query(string sql);
}
public interface ICRUDQuery : IFreeQuery
{
    public IEnumerable<T> GetAll<T>(IDbTransaction transaction = null) where T : class;
    public T? GetById<T>(int id, IDbTransaction transaction = null) where T : class;
    public int? Add<T>(T entityToInsert, IDbTransaction? transaction = null) where T : class;
    public bool Update<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class;
    public bool Delete<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class;

    void BulkInsert<T>(IEnumerable<T> elements) where T : class;
    void BulkUpdate<T>(IEnumerable<T> elements) where T : class;
    void BulkSynchronize<T>(IEnumerable<T> elements) where T : class;
    void BulkDelete<T>(IEnumerable<T> elements) where T : class;
    void BulkMerge<T>(IEnumerable<T> elements) where T : class;
}
