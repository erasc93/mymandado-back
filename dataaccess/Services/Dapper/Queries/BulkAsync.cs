using MySql.Data.MySqlClient;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System.Data;
using Z.Dapper.Plus;

namespace Services.Dapper.Queries;

public class BulkAsync : IBulkAsync
{
    private IConnectionInformation_DB _credentialDatabase;
    public BulkAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    public async Task BulkInsert<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkInsertAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkSynchronize<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkSynchronizeAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkUpdate<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkUpdateAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkDelete<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkDeleteAsync(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }

    public async Task BulkMerge<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkMergeAsync(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }
}
