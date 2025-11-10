using Dapper;
using MySql.Data.MySqlClient;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class FreeQueryAsync : IFreeQueryAsync
{
    private IConnectionInformation_DB _credentialDatabase;

    public FreeQueryAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    // --- --- --- FREE MySQL QUERY 
    public async Task<IEnumerable<T>> Query<T>(string mySql, Dictionary<string, object>? param, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        IEnumerable<T> output;
        DynamicParameters? parameters;

        parameters = param is null
        ? null
        : new DynamicParameters(param);

        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await conn.QueryAsync<T>(mySql, parameters, transaction);
        if (useOwnConnection) { conn.Close(); }


        return output;
    }
    public async Task Query(string mySql, Dictionary<string, object>? param, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        DynamicParameters? parameters;
        parameters = param is null
            ? null
            : new DynamicParameters(param);
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.QueryAsync(sql: mySql, parameters, transaction);
        if (useOwnConnection) { conn.Close(); }
    }
}
