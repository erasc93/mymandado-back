using MySql.Data.MySqlClient;
using System.Data;

namespace Services.Dapper.DBWire;
public interface ITransactionHandle
{
    IDbConnection? connection { get; }
    IDbTransaction? transaction { get; }
    bool isConnectionClosed { get; }

    void Close();

    /// <summary>
    /// Should allways be followed by 
    /// connection.Close();
    /// </summary>
    public void OpenConnectionBeginTransaction();
    //public void Close();
}


public class TransactionHandle(IConnectionInformation_DB _credentialDatabase) : ITransactionHandle
{
    public IDbConnection? connection { get; private set; }
    public IDbTransaction? transaction { get; private set; }
    public bool isConnectionClosed => connection?.State != ConnectionState.Open;


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
    public void Close()
    {
        if (isConnectionClosed )
        {
            throw new DisposeProblem();
        }
        transaction.Dispose();
        connection.Dispose();
    }
    public class DisposeProblem : Exception { }
}