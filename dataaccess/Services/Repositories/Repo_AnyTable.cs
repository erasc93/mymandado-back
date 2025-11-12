using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories.Abstractions;
using System.Data;

namespace Services.Repositories;

public class Repo_AnyTable<T>(IQueries query) : ARepository(query) where T : class, IDbTable
{
    public T? GetById(int id, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        return _query.crud.GetById<T>(id,conn,transaction);
    }
    public T[] GetAll(IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        T[] output;
        output = _query.crud.GetAll<T>(conn, transaction);
        return output;
    }

    /// <summary>item id is already updated </summary>
    /// <returns>id of created element</returns>
    public int Add(ref T item, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        int o = _query.crud.Add(ref item, conn, transaction);
        return o;
    }
    public int Add(ref T[] item, IDbConnection c, IDbTransaction t)
    {
        return _query.crud.Add(ref item, c, t);
    }

    public bool Delete(T item, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        bool success;

        success = _query.crud.Delete(item, conn, transaction);

        if (!success)
        {
            string msg;
            msg = $"Suppression de l'élément {item} impossible";
            throw new Exception(msg);
        }
        return success;
    }
    public void Update(T updated, IDbConnection? conn = null, IDbTransaction? transaction = null)
    {
        bool success;
        success = _query.crud.Update(updated, conn, transaction);

        if (!success)
        {
            string msg;
            msg = $"Mise à jour impossible de l'utilisateur {updated} impossible";
            throw new Exception(msg);
        }
    }
}
