using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class CRUDAsync : ICRUDAsync
{
    private IConnectionInformation_DB _credentialDatabase;

    public CRUDAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    public async Task<int?> Add<T>(T entityToInsert, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        int? id;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        id = await conn.InsertAsync(entityToInsert, transaction);
        if (useOwnConnection) { conn.Close(); }
        return id;
    }
    public async Task<bool> Update<T>(T entityToUpdate, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        bool output,
                    useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await connection.UpdateAsync(entityToUpdate, transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public async Task<bool> Delete<T>(T entityToUpdate, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        bool
            output,
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await conn.DeleteAsync(entityToUpdate, transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public async Task<IEnumerable<T>> GetAll<T>(IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        IEnumerable<T> output;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await conn.GetAllAsync<T>(transaction);
        if (useOwnConnection) { conn.Close(); }

        return output;
    }
    public async Task<T?> GetById<T>(int id, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        T?
            output;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await conn.GetAsync<T>(id, transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
}
