using Dapper;
using MySql.Data.MySqlClient;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class FreeQuery(IConnectionInformation_DB _credentialDatabase, ITransactionHandle _transacHandle) : IFreeQuery
{
    public IEnumerable<T> Query<T>(string mySql, Dictionary<string, object>? param = null)
    {
        IEnumerable<T>
            output;
        DynamicParameters?
            parameters = param is null ? null : new DynamicParameters(param);
        bool
            useOwnConnection = _transacHandle.UseOwnConnection;
        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;

        if (useOwnConnection) { conn.Open(); }
        output = conn.Query<T>(mySql, parameters, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public void Query(string mySql, Dictionary<string, object>? param = null)
    {


        var parameters = param is null ? null : new DynamicParameters(param);
        bool
            useOwnConnection = _transacHandle.UseOwnConnection;
        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;

        if (useOwnConnection) { conn.Open(); }
        conn.Query(sql: mySql, parameters, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
    }
}
