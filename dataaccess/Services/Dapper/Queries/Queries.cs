using MySql.Data.MySqlClient;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;

public class Queries : IQueries
{
    private IConnectionInformation_DB _credentialDatabase { get; init; }
    public Queries(IConnectionInformation_DB credentials,
        IFreeQuery free, ICRUD crud, IBulk bulk,
        IFreeQueryAsync freeAsync, ICRUDAsync crudAsync, IBulkAsync bulkAsync)
    {
        _credentialDatabase = credentials;
        this.free = free;
        this.crud = crud;
        this.bulk = bulk;
        this.freeAsync = freeAsync;
        this.crudAsync = crudAsync;
        this.bulkAsync = bulkAsync;
    }

    public IFreeQuery free { get; private set; }
    public ICRUD crud { get; private set; }
    public IBulk bulk { get; private set; }

    public IFreeQueryAsync freeAsync { get; private set; }
    public ICRUDAsync crudAsync { get; private set; }
    public IBulkAsync bulkAsync { get; private set; }
    public void ExecuteInTransaction(Action<IDbConnection, IDbTransaction> action)
    {


        using IDbConnection conn = new MySqlConnection(_credentialDatabase.ConnectionString);
        conn.Open();
        using IDbTransaction transaction = conn.BeginTransaction();

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
