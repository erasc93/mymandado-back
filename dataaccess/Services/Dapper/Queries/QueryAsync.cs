using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Data;
using Z.Dapper.Plus;

namespace Services.Dapper.Queries;

public class CRUDAsync : ICRUDAsync
{
    private IConnectionInformation_DB _credentialDatabase;

    public CRUDAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }


    public async Task<int?> Add<T>(T entityToInsert, IDbTransaction? transaction = null) where T : class
    {
        int? id = null;
        using (var connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            id = await connection.InsertAsync(entityToInsert, transaction);
        }
        return id;
    }
    public async Task<bool> Update<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        return await connection.UpdateAsync(entityToUpdate, transaction);
    }
    public async Task<bool> Delete<T>(T entityToUpdate, IDbTransaction? transaction = null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        return await connection.DeleteAsync(entityToUpdate, transaction);
    }
    public async Task<IEnumerable<T>> GetAll<T>(IDbTransaction? transaction = null) where T : class
    {
        MySqlConnection connection;
        IEnumerable<T> output;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = await connection.GetAllAsync<T>(transaction);
        }
        return output;

    }
    public async Task<T?> GetById<T>(int id, IDbTransaction? transaction = null) where T : class
    {
        T? output;
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = await connection.GetAsync<T>(id, transaction);
        }
        return output;

    }
}
public class BulkAsync : IBulkAsync
{
    private IConnectionInformation_DB _credentialDatabase;

    public BulkAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }
    public async Task BulkInsert<T>(IEnumerable<T> entitiesToInsert) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            await connection.BulkInsertAsync(entitiesToInsert);
        }
    }
    public async Task BulkSynchronize<T>(IEnumerable<T> entitiesToInsert) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            await connection.BulkSynchronizeAsync(entitiesToInsert);
        }
    }
    public async Task BulkUpdate<T>(IEnumerable<T> entitiesToInsert) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            await connection.BulkUpdateAsync(entitiesToInsert);
        }
    }


    public async Task BulkDelete<T>(IEnumerable<T> entitiesToMerge) where T : class
    {
        MySqlConnection connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            await connection.BulkDeleteAsync(entitiesToMerge);
        }
    }
    public async Task BulkMerge<T>(IEnumerable<T> entitiesToMerge) where T : class
    {
        IDbConnection
            connection;
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            await connection.BulkMergeAsync(entitiesToMerge);
        }
    }
}
public class FreeQueryAsync : IFreeQueryAsync
{
    private IConnectionInformation_DB _credentialDatabase;

    public FreeQueryAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    // --- --- --- FREE MySQL QUERY 
    public async Task<IEnumerable<T>> Query<T>(string mySql, Dictionary<string, object>? param, IDbTransaction? transaction = null)
    {
        MySqlConnection connection;
        IEnumerable<T>
            output;

        var parameters = param is null
            ? null
            : new DynamicParameters(param);

        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            output = await connection.QueryAsync<T>(mySql, parameters, transaction);
        }

        return output;
    }
    public async Task Query(string mySql, Dictionary<string, object>? param, IDbTransaction? transaction = null)
    {
        MySqlConnection connection;

        var parameters = param is null
            ? null
            : new DynamicParameters(param);
        using (connection = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            await connection.QueryAsync(sql: mySql, parameters, transaction);
        }

    }

}
