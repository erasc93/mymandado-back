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
            u = new User() { id = 1, name = "manu", role = User.Role.admin };
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
        MND_USERS[] mndusers = _query.free.Query<MND_USERS>(query, param).ToArray();

        User? output;
        output = (from u in mndusers
                  select new User()
                  {
                      id = u.usr_id,
                      name = u.usr_name,
                      role = u.usr_role,
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
                  select new User() { id = u.usr_id, name = u.usr_name, role = u.usr_role }
                  ).ToArray();

        return output;
    }
    public User? AddByName(string userName)
    {
        MND_USERS mnduser;
        MND_CART firstCart;
        int? userid = null;
        User? output = null;

        mnduser = new MND_USERS
        {
            usr_name = userName,
            usr_role = User.Role.friend
        };



        _query.ExecuteInTransaction((c, t) =>
        {
            userid = _query.crud.Add<MND_USERS>(mnduser, c, t)!;
            firstCart = new MND_CART
            {
                car_crtnb = 0,
                car_desc = "",
                car_name = "cart",
                car_usrid = (int)userid
            };
            firstCart.car_usrid = (int)_query.crud.Add<MND_CART>(firstCart,c,t)!;
            output = new User()
            {
                id = (int)userid!,
                name = mnduser.usr_name,
                role = mnduser.usr_role
            };
        });


        return output;
    }
    public User AddByNameSafe(string userName)
    {
        MND_USERS mnduser;
        int? userid;
        User output;

        mnduser = new MND_USERS
        {
            usr_name = userName,
            usr_role = User.Role.friend
        };


        userid = _query.crud.Add(mnduser);

        output = new User()
        {
            id = (int)userid!,
            name = mnduser.usr_name,
            role = mnduser.usr_role
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
            usr_name = user.name,
            usr_role = user.role,
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
            usr_name = updated.name,
            usr_role = updated.role,
        };
        success = _query.crud.Update(mnduser);

        if (!success)
        {
            string
            msg = $"Mise à jour impossible de l'utilisateur {updated.name} impossible";
            throw new Exception(msg);
        }
    }
}
