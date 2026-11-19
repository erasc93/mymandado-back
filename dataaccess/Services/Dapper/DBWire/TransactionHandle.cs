using System;
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


public class TransactionHandle(IConnectionInformation_DB _credentialDatabase, Func<string, IDbConnection> _facDbConnection) : ITransactionHandle
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
        connection = _facDbConnection(_credentialDatabase.ConnectionString);
        connection.Open();
        transaction = connection.BeginTransaction();
    }
    public void Close()
    {
        if (isConnectionClosed)
        {
            throw new DisposeProblem();
        }
        transaction!.Dispose();
        connection!.Dispose();
    }
    public class DisposeProblem : Exception { }
}