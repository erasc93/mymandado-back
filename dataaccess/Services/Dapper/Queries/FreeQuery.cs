using Dapper;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Services.Dapper.Queries;

public class FreeQuery(
                            IConnectionInformation_DB _credentialDatabase,
                            ITransactionHandle _transacHandle,
                            Func<string, IDbConnection> _facDbConnection
                       ) : IFreeQuery
{
    public IEnumerable<T> Query<T>(string mySql, Dictionary<string, object>? param = null)
    {
        IEnumerable<T> output;

        bool useOwnConnection;
        IDbConnection conn;
        DynamicParameters? parameters;

        useOwnConnection = _transacHandle.isConnectionClosed;
        conn = (useOwnConnection) ? _facDbConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        parameters = param is null ? null : new DynamicParameters(param);

        try
        {
            if (useOwnConnection) { conn.Open(); }
        }
        catch
        {
            throw;
        }
        output = conn.Query<T>(mySql, parameters, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }

        return output;
    }
    public void Query(string mySql, Dictionary<string, object>? param = null)
    {
        var parameters = param is null ? null : new DynamicParameters(param);
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;
        IDbConnection conn;
        conn = (useOwnConnection)
                ? _facDbConnection(_credentialDatabase.ConnectionString)
                : _transacHandle.connection!;

        if (useOwnConnection) { conn.Open(); }
        conn.Query(sql: mySql, parameters, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
    }
}
