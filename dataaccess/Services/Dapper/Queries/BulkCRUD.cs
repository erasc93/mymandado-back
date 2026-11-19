using Dapper.Contrib.Extensions;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using Z.Dapper.Plus;

namespace Services.Dapper.Queries;

public class BulkCRUD(IConnectionInformation_DB _credentialDatabase, Func<string, IDbConnection> _facDbConnection) : IBulk
{

    public IEnumerable<T> GetAll<T>(IDbConnection? conn = null) where T : class
    {
        IEnumerable<T> output;
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = conn.GetAll<T>();
        if (useOwnConnection) { conn.Close(); }
        return output;

    }

    public void BulkInsert<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkInsert(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public void BulkSynchronize<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkSynchronize(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }

    }
    public void BulkUpdate<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkUpdate(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public void BulkDelete<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkDelete(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }

    public void BulkMerge<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= _facDbConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkMerge(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }
}
