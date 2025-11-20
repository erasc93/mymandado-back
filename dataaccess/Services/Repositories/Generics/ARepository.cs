using Services.Dapper.Interfaces;

namespace Services.Repositories.Generics;

public abstract class ARepository
{
    protected IQueries _query { get; private set; }
    public ARepository(IQueries query)
    {
        _query = query;
    }
}
