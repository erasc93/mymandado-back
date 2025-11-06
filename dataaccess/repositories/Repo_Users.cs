using core_mandado.models;
using core_mandado.repositories;
using dataaccess.information_schema.tables;
using dbaccess;
using repositories.Abstractions;
using repositories.ICRUD;
using System.Collections;
using System.Data;

namespace repositories;



public class Repo_Users(ICRUDQuery query) : ARepository(query), IRepo_CREATE<User>, IRepo_READ<User>, IRepo_DELETE<User>, IRepo_UPDATE<User>, IRepo_Users
{
    public User GetCurrent()
    {
        User u = new User() { id = 1, name = "manu"};
        return u;
    }
    public User? GetUserByName(string userName)
    {
        string query;
        query = $"select * from USERS where usr_name='{userName}'";
        MND_USERS[] mndusers = _query.Query<MND_USERS>(query).ToArray();

        User? output;
        output = (from u in mndusers
                  select new User() { id = u.usr_id, name = u.usr_name }
                  ).FirstOrDefault();

        return output;
    }
    public User[] GetAll()
    {
        MND_USERS[] mndusers;
        mndusers = _query.GetAll<MND_USERS>().ToArray();

        User[] output;
        output = (from u in mndusers
                  select new User() { id = u.usr_id, name = u.usr_name }
                  ).ToArray();

        return output;
    }

    public User AddByName(string userName)
    {
        MND_USERS mnduser;
        mnduser = new MND_USERS() { usr_name = userName };
        int? o = _query.Add<MND_USERS>(mnduser);
        User output;
        output = new User()
        {
            id = (int)o,
            name = mnduser.usr_name
        };
        return output;
    }
    public void Add(ref User item)
    {
        AddByName(item.name);
    }

    public bool Delete(User user)
    {
        bool success;

        MND_USERS mnduser;

        mnduser = new MND_USERS()
        {
            usr_id = (int)user.id,
            usr_name = user.name
        };

        success = _query.Delete<MND_USERS>(mnduser);

        if (!success)
        {
            string msg;
            msg = $"Suppression de l'utilisateur {user.name} impossible";
            throw new Exception(msg);
        }
        return success;
    }
    public void Update(User updated)
    {
        bool success;

        MND_USERS mnduser;

        mnduser = new MND_USERS()
        {
            usr_id = (int)updated.id,
            usr_name = updated.name
        };
        success = _query.Update<MND_USERS>(mnduser);

        if (!success)
        {
            string msg;
            msg = $"Mise à jour impossible de l'utilisateur {updated.name} impossible";
            throw new Exception(msg);
        }
    }
}
