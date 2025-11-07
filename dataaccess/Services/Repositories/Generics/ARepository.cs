using Services.Dapper;

namespace Services.Repositories.Abstractions;

public abstract class ARepository
{
    protected ICRUDQuery _query;
    public ARepository(ICRUDQuery query) { _query = query; }
}
