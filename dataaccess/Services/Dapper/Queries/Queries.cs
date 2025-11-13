using MySql.Data.MySqlClient;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using System.Data;
using System.Diagnostics.Eventing.Reader;

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
    public void ExecuteInTransaction(Action action, bool immediatRollback = false)
    {
        _transacHandle.OpenConnectionBeginTransaction();
        try
        {
            action();

            if (immediatRollback)
            {
                _transacHandle.transaction.Rollback();
            }
            else
            {
                _transacHandle.transaction.Commit();
            }
        }
        catch (Exception e)
        {
            if (_transacHandle.transaction is null)
            {
                throw new RollBackException("RollBack could not perform ");
            }
            _transacHandle.transaction.Rollback();
            throw new RollBackException(e.Message+"\n An exception ocurred, rollback was performed correctly ! ", e);
        }
        finally
        {
            _transacHandle.Close();
        }
    }

    public class RollBackException : Exception
    {
        public RollBackException() : base() { }
        public RollBackException(string msg) : base(msg) { }
        public RollBackException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}

