using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories.Abstractions;
using Services.Repositories.Interfaces;
using System.Data;

namespace core;

public class Repo_Users(IQueries query) : ARepository(query),
                                            IRepo_Users,
                                            IRepo_CREATE<User>, IRepo_READ<User>, IRepo_DELETE<User>, IRepo_UPDATE<User>
{

    public bool Login(LoginInfo login)
    {
        return login.username == "manu" || login.username == "cleo";
    }
    public User GetCurrent()
    {
        User
            u = new User() { id = 1, name = "manu" };
        return u;
    }
    public User? GetUserByName(string userName)
    {
        string query;
        Dictionary<string, object> param;

        param = new Dictionary<string, object>()
                    {
                        {"@username",userName},
                    };
        query = $"select * from USERS where usr_name=@username";
        MND_USERS[] mndusers = _query.free.Query<MND_USERS>(query,param).ToArray();

        User? output;
        output = (from u in mndusers
                  select new User()
                  {
                      id = u.usr_id,
                      name = u.usr_name,
                  }
                  ).FirstOrDefault();

        return output;
    }
    public User[] GetAll()
    {
        MND_USERS[] mndusers;
        mndusers = _query.crud.GetAll<MND_USERS>().ToArray();

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
        int? o = _query.crud.Add(mnduser);
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
            usr_id = user.id,
            usr_name = user.name
        };

        success = _query.crud.Delete(mnduser);

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
            usr_id = updated.id,
            usr_name = updated.name
        };
        success = _query.crud.Update(mnduser);

        if (!success)
        {
            string msg;
            msg = $"Mise à jour impossible de l'utilisateur {updated.name} impossible";
            throw new Exception(msg);
        }
    }
}
