using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace Services.Dapper.Queries;

public class BulkAsync(IConnectionInformation_DB _credentialDatabase,Func<string,IDbConnection>_facDbConnection) : IBulkAsync
{
    public async Task BulkInsert<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkInsertAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkSynchronize<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkSynchronizeAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkUpdate<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkUpdateAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkDelete<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkDeleteAsync(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }

    public async Task BulkMerge<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkMergeAsync(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }
}
