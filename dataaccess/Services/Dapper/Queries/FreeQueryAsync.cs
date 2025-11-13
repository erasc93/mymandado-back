using Dapper;
using MySql.Data.MySqlClient;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class FreeQueryAsync(IConnectionInformation_DB _credentialDatabase, ITransactionHandle _transacHandle) : IFreeQueryAsync
{
    // --- --- --- FREE MySQL QUERY 
    public async Task<IEnumerable<T>> Query<T>(string mySql, Dictionary<string, object>? param)
    {
        IEnumerable<T> output;
        DynamicParameters? parameters;

        parameters = param is null
        ? null
        : new DynamicParameters(param);

        bool
            useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        output = await conn.QueryAsync<T>(mySql, parameters, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }


        return output;
    }
    public async Task Query(string mySql, Dictionary<string, object>? param)
    {
        DynamicParameters? parameters;
        parameters = param is null
            ? null
            : new DynamicParameters(param);
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        await conn.QueryAsync(sql: mySql, parameters, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
    }
}
