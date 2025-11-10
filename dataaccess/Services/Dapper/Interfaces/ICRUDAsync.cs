using System.Data;

namespace Services.Dapper.Interfaces;

public interface ICRUDAsync
{
    public Task<IEnumerable<T>> GetAll<T>(IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<T?> GetById<T>(int id, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<int?> Add<T>(T entityToInsert, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<bool> Update<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public Task<bool> Delete<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
}
