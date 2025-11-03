using dbaccess;

namespace repositories.Abstractions;

public abstract class ARepository
{
    protected ICRUDQuery _query;
    public ARepository(ICRUDQuery query) { _query = query; }
}
