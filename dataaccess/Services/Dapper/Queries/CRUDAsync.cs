using Dapper.Contrib.Extensions;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Dapper.Queries;

public class CRUDAsync(IConnectionInformation_DB _credentialDatabase, ITransactionHandle _transacHandle,Func<string,IDbConnection>_facDbConnection) : ICRUDAsync
{
    public async Task<int?> Add<T>(T entityToInsert) where T : class
    {
        int? id;
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? _facDbConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        id = await conn.InsertAsync(entityToInsert, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return id;
    }
    public async Task<bool> Update<T>(T entityToUpdate) where T : class
    {
        using IDbConnection connection = _facDbConnection(_credentialDatabase.ConnectionString);
        bool output,
                    useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? _facDbConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        output = await connection.UpdateAsync(entityToUpdate, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public async Task<bool> Delete<T>(T entityToUpdate) where T : class
    {
        bool
            output,
            useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? _facDbConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        output = await conn.DeleteAsync(entityToUpdate, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public async Task<T[]> GetAll<T>() where T : class
    {
        IEnumerable<T> output;
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? _facDbConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        output = await conn.GetAllAsync<T>(_transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }

        return output.ToArray();
    }
    public async Task<T?> GetById<T>(int id) where T : class
    {
        T?
            output;
        bool
            useOwnConnection = _transacHandle.isConnectionClosed;

        IDbConnection conn = (useOwnConnection) ? _facDbConnection(_credentialDatabase.ConnectionString) : _transacHandle.connection!;
        if (useOwnConnection) { conn.Open(); }
        output = await conn.GetAsync<T>(id, _transacHandle.transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
}
