using Dapper;
using MySql.Data.MySqlClient;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class FreeQuery : IFreeQuery
{
    private IConnectionInformation_DB _credentialDatabase;

    public FreeQuery(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    public IEnumerable<T> Query<T>(string mySql, Dictionary<string, object>? param = null, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        IEnumerable<T>
            output;
        DynamicParameters?
            parameters = param is null ? null : new DynamicParameters(param);
        bool
            useOwnConnection = conn is null;
        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);

        if (useOwnConnection) { conn.Open(); }
        output = conn.Query<T>(mySql, parameters, transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public void Query(string mySql, Dictionary<string, object>? param = null, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {


        var parameters = param is null ? null : new DynamicParameters(param);
        bool
            useOwnConnection = conn is null;
        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);

        if (useOwnConnection) { conn.Open(); }
        conn.Query(sql: mySql, parameters, transaction = null);
        if (useOwnConnection) { conn.Close(); }
    }
}
