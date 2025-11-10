using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using Services.Dapper.Queries;
using System.Data;
using Z.Dapper.Plus;

namespace Services.Dapper.Queries;
//public class Queries : IQueries
//{
//    public Queries(IFreeQuery free, ICRUD crud, IBulk bulk, IFreeQueryAsync freeAsync, ICRUDAsync crudAsync, IBulkAsync bulkAsync)
//    {
//        this.free = free;
//        this.crud = crud;
//        this.bulk = bulk;
//        this.freeAsync = freeAsync;
//        this.crudAsync = crudAsync;
//        this.bulkAsync = bulkAsync;
//    }

//    public IFreeQuery free { get; private set; }
//    public ICRUD crud { get; private set; }
//    public IBulk bulk { get; private set; }

//    public IFreeQueryAsync freeAsync { get; private set; }
//    public ICRUDAsync crudAsync { get; private set; }
//    public IBulkAsync bulkAsync { get; private set; }

//}

public class CRUD : ICRUD
{
    private IConnectionInformation_DB _credentialDatabase;

    public CRUD(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }


    public int? Add<T>(T entityToInsert, IDbTransaction? transaction) where T : class
    {
        int? id = null;
        using (var connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            id = (int?)connection.Insert(entityToInsert);
        }
        return id;
    }
    public bool Update<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        return connection.Update(entityToUpdate);
    }
    public bool Delete<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        return connection.Delete(entityToUpdate);
    }
    public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null) where T : class
    {
        MySqlConnection connection;
        IEnumerable<T> output;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = connection.GetAll<T>(transaction);
        }
        return output;

    }
    public T? GetById<T>(int id, IDbTransaction? transaction = null) where T : class
    {
        T? output;
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = connection.Get<T>(id, transaction);
        }
        return output;

    }
}
public class FreeQuery : IFreeQuery
{
    private IConnectionInformation_DB _credentialDatabase;

    public FreeQuery(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    // --- --- --- FREE MySQL QUERY 
    public IEnumerable<T> Query<T>(string mySql, Dictionary<string, object>? param = null, IDbTransaction? transaction = null)
    {
        MySqlConnection connection;
        IEnumerable<T>
            output;
        var parameters = param is null ? null : new DynamicParameters(param);

        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = connection.Query<T>(mySql, parameters, transaction);
        }
        return output;
    }
    public void Query(string mySql, Dictionary<string, object>? param = null, IDbTransaction? transaction = null)
    {
        MySqlConnection connection;

        var parameters = param is null ? null : new DynamicParameters(param);

        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            connection.Query(sql: mySql, parameters, transaction = null);
        }
    }


}
public class BulkCRUD : IBulk
{
    private IConnectionInformation_DB _credentialDatabase;

    public BulkCRUD(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    public IEnumerable<T> GetAll<T>(IDbTransaction? transaction = null) where T : class
    {
        MySqlConnection connection;
        IEnumerable<T> output;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = connection.GetAll<T>(transaction);
        }
        return output;

    }

    public void BulkInsert<T>(IEnumerable<T> entitiesToInsert) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))

        {
            connection.BulkInsert(entitiesToInsert);
        }
    }
    public void BulkSynchronize<T>(IEnumerable<T> entitiesToInsert) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            connection.BulkSynchronize(entitiesToInsert);
        }
    }
    public void BulkUpdate<T>(IEnumerable<T> entitiesToInsert) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            connection.BulkUpdate(entitiesToInsert);
        }
    }


    public void BulkDelete<T>(IEnumerable<T> entitiesToMerge) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            connection.BulkDelete(entitiesToMerge);
        }
    }
    public void BulkMerge<T>(IEnumerable<T> entitiesToMerge) where T : class
    {
        IDbConnection
            connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            connection.BulkMerge(entitiesToMerge);
        }
    }
}

