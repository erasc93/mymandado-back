using MySql.Data.MySqlClient;
using System.Data;

namespace Services.Dapper.Queries;
public interface ITransactionHandle
{
    IDbConnection? connection { get; }
    IDbTransaction? transaction { get; }
    bool UseOwnConnection { get; }

    public void OpenConnectionBeginTransaction();
    //public void Close();
}


public class TransactionHandle(IConnectionInformation_DB _credentialDatabase) : ITransactionHandle
{
    public IDbConnection? connection { get; private set; }
    public IDbTransaction? transaction { get; private set; }
    public bool UseOwnConnection => connection is null || transaction is null;

    /// <summary>
    /// Should allways be followed by 
    /// connection.Close();
    /// </summary>
    public void OpenConnectionBeginTransaction()
    {
        connection = new MySqlConnection(_credentialDatabase.ConnectionString);
        connection.Open();
        transaction = connection.BeginTransaction();
    }
    //public void Close()
    //{
    //    //transaction?.Dispose();
    //    //connection?.Dispose();
    //}
}





//public class Queries : IQueries
//{
//    private IConnectionInformation_DB _credentialDatabase { get; init; }
//    public Queries(
//        IConnectionInformation_DB credentials,
//        IFreeQuery free, ICRUD crud, IBulk bulk,
//        IFreeQueryAsync freeAsync, ICRUDAsync crudAsync, IBulkAsync bulkAsync
//        )
//    {
//        _credentialDatabase = credentials;
//        this.free = free;
//        this.crud = crud;
//        this.bulk = bulk;
//        this.freeAsync = freeAsync;
//        this.crudAsync = crudAsync;
//        this.bulkAsync = bulkAsync;
//    }

//    public IFreeQuery free { get; private set; }
//    public ICRUD crud { get; private set; }
//    public IBulk bulk { get; private set; }

//    public IFreeQueryAsync freeAsync { get; private set; }
//    public ICRUDAsync crudAsync { get; private set; }
//    public IBulkAsync bulkAsync { get; private set; }
//    public void ExecuteInTransaction(Action<IDbConnection, IDbTransaction> action)
//    {
//        using IDbConnection
//            conn = new MySqlConnection(_credentialDatabase.ConnectionString);
//        conn.Open();
//        using IDbTransaction
//            transaction = conn.BeginTransaction();
//        try
//        {
//            action(conn, transaction);
//            transaction.Commit();
//        }
//        catch
//        {
//            transaction.Rollback();
//            throw;
//        }
//    }
//}
