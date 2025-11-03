using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Data;
using Z.Dapper.Plus;

namespace dbaccess;

public class DapperMySQL_DataAccess : ICRUDQuery, IFreeQuery
{
    private IConnectionInformation_DB _credentialDatabase;

    public DapperMySQL_DataAccess(IConnectionInformation_DB credentialDatabase)
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
    public bool Update<T>(T entityToUpdate, IDbTransaction? transaction=null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        return connection.Update(entityToUpdate);
    }
    public bool Delete<T>(T entityToUpdate, IDbTransaction? transaction=null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        return connection.Delete(entityToUpdate);
    }
    public IEnumerable<T> GetAll<T>(IDbTransaction? transaction=null) where T : class
    {
        MySqlConnection connection;
        IEnumerable<T> output;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = connection.GetAll<T>(transaction);
        }
        return output;

    }
    public T? GetById<T>(int id,IDbTransaction? transaction=null) where T : class
    {
        T? output;
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = connection.Get<T>(id,transaction);
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


    // --- --- --- FREE MySQL QUERY 
    public IEnumerable<T> Query<T>(string mySql)
    {
        MySqlConnection connection;
        IEnumerable<T>
            output;

        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = connection.Query<T>(mySql);
        }

        return output;
    }
    public void Query(string mySql)
    {
        MySqlConnection connection;

        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            connection.Query(sql:mySql);
        }

    }

}