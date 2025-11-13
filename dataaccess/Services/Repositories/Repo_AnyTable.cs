using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories.Abstractions;
using System.Data;

namespace Services.Repositories;

public class Repo_AnyTable<T>(IQueries query) : ARepository(query) where T : class, IDbTable
{
    public T? GetById(int id)
    {
        return _query.crud.GetById<T>(id);
    }
    public T[] GetAll()
    {
        T[] output;
        output = _query.crud.GetAll<T>();
        return output;
    }

    /// <summary>item id is already updated </summary>
    /// <returns>id of created element</returns>
    public int Add(ref T item)
    {
        int o = _query.crud.Add(ref item);
        return o;
    }
    public int Add(ref T[] item)
    {
        return _query.crud.Add(ref item);
    }

    public bool Delete(T item)
    {
        bool success;

        success = _query.crud.Delete(item);

        if (!success)
        {
            string msg;
            msg = $"Suppression de l'élément {item} impossible";
            throw new Exception(msg);
        }
        return success;
    }
    public void Update(T updated)
    {
        bool success;
        success = _query.crud.Update(updated);

        if (!success)
        {
            string msg;
            msg = $"Mise à jour impossible de l'utilisateur {updated} impossible";
            throw new Exception(msg);
        }
    }
}
