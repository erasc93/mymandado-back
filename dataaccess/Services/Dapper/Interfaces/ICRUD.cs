using System.Data;

namespace Services.Dapper.Interfaces;

public interface ICRUD
{
    public T[] GetAll<T>(IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public T? GetById<T>(int id, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;

    /// <returns>element id</returns>
    public int Add<T>(ref T entityToInsert, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;

    /// <returns>number of inserted rows</returns>
    int Add<T>(ref T[] entityToInsert, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class;

    public bool Update<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;
    public bool Delete<T>(T entityToUpdate, IDbConnection? connection = null, IDbTransaction? transaction = null) where T : class;

}
