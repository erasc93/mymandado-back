using models.tables;
using Services.Dapper.Queries;
using Services.Repositories.Abstractions;
using System.Data;

namespace Services.Repositories;

public class Repo_AnyTable<T>(IQueries query) : ARepository(query) where T : class, IDbTable
{
    public T? GetById(int id, IDbTransaction? transaction = null)
    {
        return _query.crud.GetById<T>(id);
    }
    public T[] GetAll(IDbTransaction? transaction = null)
    {
        T[] output;
        output = [.. _query.crud.GetAll<T>()];
        return output;
    }

    public void Add(ref T item, IDbTransaction? transaction = null)
    {
        int? o = _query.crud.Add(item, transaction);
    }

    public bool Delete(T item, IDbTransaction? transaction = null)
    {
        bool success;

        success = _query.crud.Delete(item, transaction);

        if (!success)
        {
            string msg;
            msg = $"Suppression de l'élément {item} impossible";
            throw new Exception(msg);
        }
        return success;
    }
    public void Update(T updated, IDbTransaction? transaction=null)
    {
        bool success;
        success = _query.crud.Update(updated,transaction);

        if (!success)
        {
            string msg;
            msg = $"Mise à jour impossible de l'utilisateur {updated} impossible";
            throw new Exception(msg);
        }
    }
}
