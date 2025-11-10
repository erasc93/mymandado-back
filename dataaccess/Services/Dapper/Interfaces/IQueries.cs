using System.Data;

namespace Services.Dapper.Interfaces;

// --- --- ---
public interface IQueries
{
    public IFreeQuery free { get; }
    public ICRUD crud { get; }
    public IBulk bulk { get; }
    public IFreeQueryAsync freeAsync { get; }
    public ICRUDAsync crudAsync { get; }
    public IBulkAsync bulkAsync { get; }
    void ExecuteInTransaction(Action<IDbConnection, IDbTransaction> action);
}
