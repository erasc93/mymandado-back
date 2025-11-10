using Services.Dapper.Queries;

namespace Services.Repositories.Abstractions;

public abstract class ARepository
{
    protected IQueries _query { get; private set; }
    public ARepository(IQueries query)
    {
        _query = query;
    }
}
