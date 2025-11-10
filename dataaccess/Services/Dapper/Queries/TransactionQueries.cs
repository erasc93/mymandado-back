using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Data;
using Z.Dapper.Plus;

namespace Services.Dapper.Queries;

public class TransactionQueries : ITransactionQueries
{
    private IConnectionInformation_DB _credentialDatabase { get; init; }
    public TransactionQueries(IConnectionInformation_DB credentials,
        ITransacFreeQuery free, ITransacCRUD crud, ITransacBulk bulk,
        ITransacFreeQueryAsync freeAsync, ITransacCRUDAsync crudAsync, ITransacBulkAsync bulkAsync)
    {
        _credentialDatabase = credentials;
        this.free = free;
        this.crud = crud;
        this.bulk = bulk;
        this.freeAsync = freeAsync;
        this.crudAsync = crudAsync;
        this.bulkAsync = bulkAsync;
    }

    public ITransacFreeQuery free { get; private set; }
    public ITransacCRUD crud { get; private set; }
    public ITransacBulk bulk { get; private set; }

    public ITransacFreeQueryAsync freeAsync { get; private set; }
    public ITransacCRUDAsync crudAsync { get; private set; }
    public ITransacBulkAsync bulkAsync { get; private set; }
    public void ExecuteInTransaction(Action<IDbConnection, IDbTransaction> action)
    {

        IDbConnection conn;

        using (conn = new MySqlConnection(_credentialDatabase.ConnectionString))
        {
            using var transaction = conn.BeginTransaction();

            try
            {
                action(conn, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}


public class TransacFreeQuery : ITransacFreeQuery
{
    private IConnectionInformation_DB _credentialDatabase;

    public TransacFreeQuery(IConnectionInformation_DB credentialDatabase)
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

// --- --- --- FREE MySQL QUERY 
public class TransacCRUD : ITransacCRUD
{
    private IConnectionInformation_DB _credentialDatabase;

    public TransacCRUD(IConnectionInformation_DB credentialDatabase)
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
public class TransacBulkCRUD : ITransacBulk
{
    private IConnectionInformation_DB _credentialDatabase;

    public TransacBulkCRUD(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    public IEnumerable<T> GetAll<T>(IDbConnection? conn = null) where T : class
    {
        IEnumerable<T> output;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = conn.GetAll<T>();
        if (useOwnConnection) { conn.Close(); }
        return output;

    }

    public void BulkInsert<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkInsert(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public void BulkSynchronize<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkSynchronize(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }

    }
    public void BulkUpdate<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkUpdate(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public void BulkDelete<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkDelete(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }

    public void BulkMerge<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        conn.BulkMerge(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }
}

public class TransacCRUDAsync : ITransacCRUDAsync
{
    private IConnectionInformation_DB _credentialDatabase;

    public TransacCRUDAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    public async Task<int?> Add<T>(T entityToInsert, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        int? id;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        id = await conn.InsertAsync(entityToInsert, transaction);
        if (useOwnConnection) { conn.Close(); }
        return id;
    }
    public async Task<bool> Update<T>(T entityToUpdate, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        using IDbConnection connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        bool output,
                    useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await connection.UpdateAsync(entityToUpdate, transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public async Task<bool> Delete<T>(T entityToUpdate, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        bool
            output,
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await conn.DeleteAsync(entityToUpdate, transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
    public async Task<IEnumerable<T>> GetAll<T>(IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        IEnumerable<T> output;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await conn.GetAllAsync<T>(transaction);
        if (useOwnConnection) { conn.Close(); }

        return output;
    }
    public async Task<T?> GetById<T>(int id, IDbConnection? conn = null, IDbTransaction? transaction = null) where T : class
    {
        T?
            output;
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        output = await conn.GetAsync<T>(id, transaction);
        if (useOwnConnection) { conn.Close(); }
        return output;
    }
}
public class TransacBulkAsync : ITransacBulkAsync
{
    private IConnectionInformation_DB _credentialDatabase;
    public TransacBulkAsync(IConnectionInformation_DB credentialDatabase)
    {
        _credentialDatabase = credentialDatabase;
    }

    public async Task BulkInsert<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkInsertAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkSynchronize<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkSynchronizeAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkUpdate<T>(IEnumerable<T> entitiesToInsert, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkUpdateAsync(entitiesToInsert);
        if (useOwnConnection) { conn.Close(); }
    }
    public async Task BulkDelete<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkDeleteAsync(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }

    public async Task BulkMerge<T>(IEnumerable<T> entitiesToMerge, IDbConnection? conn = null) where T : class
    {
        bool
            useOwnConnection = conn is null;

        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);
        if (useOwnConnection) { conn.Open(); }
        await conn.BulkMergeAsync(entitiesToMerge);
        if (useOwnConnection) { conn.Close(); }
    }
}
public class TransacFreeQueryAsync : ITransacFreeQueryAsync
{
    private IConnectionInformation_DB _credentialDatabase;

    public TransacFreeQueryAsync(IConnectionInformation_DB credentialDatabase)
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




//bool
//            useOwnConnection = conn is null;
//        conn ??= new MySqlConnection(_credentialDatabase.ConnectionString);

//        if (useOwnConnection) { conn.Open(); }
//        if (useOwnConnection) { conn.Close(); }
