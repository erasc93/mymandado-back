using System.Data;

namespace Services.Dapper.Interfaces;

public interface ICRUD
{
    public IEnumerable<T> GetAll<T>(IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public T? GetById<T>(int id, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public int? Add<T>(T entityToInsert, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public bool Update<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public bool Delete<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;

}
