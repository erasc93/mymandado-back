using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class CRUD(IConnectionInformation_DB _credentialDatabase, ITransactionHandle _transacHandle) : ICRUD
{

    public int Add<T>(ref T entityToInsert) where T : class
    {
        int id;
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;
        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;

        if (useOwnConnection) { conn.Open(); }
        id = (int)conn.Insert(entityToInsert, _transacHandle.transaction); // todo: verify update id works
        if (useOwnConnection) { conn.Close(); }
        return id;
    }

    ///<summary>element is updated when inserted</summary>
    /// <returns>number of inserted rows</returns>
    public int Add<T>(ref T[] entityToInsert) where T : class
    {
        int id;
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;
        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;

        if (useOwnConnection) { conn.Open(); }
        id = (int)conn.Insert(entityToInsert, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return id;
    }

    public bool Update<T>(T entityToUpdate) where T : class
    {
        bool
            useOwnConnection = _transacHandle.isConnectionClosed,
            success;

        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        success = conn.Update(entityToUpdate, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return success;
    }
    public bool Delete<T>(T entityToUpdate) where T : class
    {
        bool
            useOwnConnection = _transacHandle.isConnectionClosed,
            success;

        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        success = conn.Delete(entityToUpdate, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return success;
    }
    public T[] GetAll<T>() where T : class
    {
        IEnumerable<T> output;
        bool
        useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection
            conn = (useOwnConnection)
            ? new MySqlConnection(_credentialDatabase.ConnectionString)
            : _transacHandle.connection!;

        if (useOwnConnection) { conn.Open(); }
        output = conn.GetAll<T>(_transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }

        return output.ToArray();
    }
    public T? GetById<T>(int id) where T : class
    {
        T? output;
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? new MySqlConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        output = conn.Get<T>(id, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }

        return output;

    }
}
