using MySql.Data.MySqlClient;
using Services.Dapper.Interfaces;
using System.Data;

namespace Services.Dapper.Queries;
public class Queries(
        IFreeQuery free, ICRUD crud, IBulk bulk,
        IFreeQueryAsync freeAsync, ICRUDAsync crudAsync, IBulkAsync bulkAsync,
        ITransactionHandle _transacHandle
        ) : IQueries
{
    public IFreeQuery free { get; } = free;
    public ICRUD crud { get; } = crud;
    public IBulk bulk { get; } = bulk;

    public IFreeQueryAsync freeAsync { get; } = freeAsync;
    public ICRUDAsync crudAsync { get; } = crudAsync;
    public IBulkAsync bulkAsync { get; } = bulkAsync;
    public void ExecuteInTransaction(Action action)
    {
        _transacHandle.OpenConnectionBeginTransaction();
        try
        {
            action();
            _transacHandle.transaction!.Commit();
        }
        catch (Exception e)
        {
            if (_transacHandle.transaction is null)
            {
                throw new RollBackException("RollBack could not perform ");
            }
            _transacHandle.transaction.Rollback();
            throw new RollBackException("An exception ocurred, rollback was performed correctly ! ", e);
        }
    }


    public class RollBackException : Exception
    {
        public RollBackException() : base() { }
        public RollBackException(string msg) : base(msg) { }
        public RollBackException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}

