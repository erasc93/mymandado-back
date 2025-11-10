using Services.Dapper.Queries;

namespace Services.Repositories.Abstractions;

public abstract class ARepository
{
    protected ITransactionQueries _query { get; private set; }
    public ARepository(ITransactionQueries query)
    {
        _query = query;
    }
}
