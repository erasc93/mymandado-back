using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class CRUD : ICRUD
{
    private IConnectionInformation_DB _credentialDatabase;

    public CRUD(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }


    public int? Add<T>(T entityToInsert, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        int? id = null;
        bool
            useOwnConnection = conn is null;
        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);

        if (useOwnConnection) { conn.Open(); }
        id = (int?)conn.Insert(entityToInsert);
        if (useOwnConnection) { conn.Close(); }
        return id;
    }
    public bool Update<T>(T entityToUpdate, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        bool
            useOwnConnection = conn is null,
            success;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        success = conn.Update(entityToUpdate, transaction);
        if (useOwnConnection) { conn.Close(); }
        return success;
    }
    public bool Delete<T>(T entityToUpdate, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {

        bool
            useOwnConnection = conn is null,
            success;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        success = conn.Delete(entityToUpdate, transaction);
        if (useOwnConnection) { conn.Close(); }
        return success;
    }
    public IEnumerable<T> GetAll<T>(IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        IEnumerable<T> output;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = conn.GetAll<T>(transaction);
        if (useOwnConnection) { conn.Close(); }

        return output;

    }
    public T? GetById<T>(int id, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        T? output;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = conn.Get<T>(id, transaction);
        if (useOwnConnection) { conn.Close(); }

        return output;

    }
}
