using core_mandado.Cart;
using core_mandado.Users;
using Google.Protobuf.WellKnownTypes;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories.Abstractions;
using Services.Repositories.Interfaces;
using System.Data;

namespace core;

public class Repo_Users(
                            IQueries query,
                            IRepo_Cart _repoCart
                        ) : ARepository(query), IRepo_CREATE<User>, IRepo_READ<User>, IRepo_DELETE<User>, IRepo_UPDATE<User>,
                            IRepo_Users
{
    public bool Login(LoginInfo login)
    {
        return login.username == "manu" || login.username == "cleo";
    }
    public User GetCurrent()
    {
        User
            u = new User()
            {
                id = 1,
                name = "manu",
                role = User.Role.admin
            };
        return u;
    }
    public User? GetUserByName(string userName)
    {
        string query;
        Dictionary<string, object>
            param = new Dictionary<string, object>()
                    {
                        {"@username",userName},
                    };
        query = $"select * from USERS where usr_name=@username";
        MND_USERS?
            mndusers = _query.free.Query<MND_USERS>(query, param)
            .FirstOrDefault();
        User?
            output = mndusers is not null
                    ? Factory.ToView(mndusers)
                    : null;
        return output;
    }
    public User[] GetAll()
    {
        MND_USERS[]
            mndusers = _query.crud.GetAll<MND_USERS>();
        User[]
            output = (from u in mndusers
                      select new User()
                      {
                          id = u.usr_id,
                          name = u.usr_name,
                          role = u.usr_role
                      }
                  ).ToArray();
        return output;
    }
    public User AddByName(string userName)
    {

        MND_USERS
            mnduser = new MND_USERS
            {
                usr_name = userName,
                usr_role = User.Role.friend
            };
        _query.crud.Add<MND_USERS>(ref mnduser);

        User
            output = new User()
            {
                id = mnduser.usr_id,
                name = mnduser.usr_name,
                role = mnduser.usr_role
            };


        Cart cart = _repoCart.AddNew(output, 0);

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
            usr_role = DEFAULTS.role
        };
        _query.crud.Add(ref mnduser);
        output = new User()
        {
            id = mnduser.usr_id,
            name = mnduser.usr_name,
            role = mnduser.usr_role
        };
        return output;
    }
    public void Add(ref User item)
    {
        item = AddByName(item.name);
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
    private static class Factory
    {
        public static User ToView(MND_USERS u)
        {
            User
                output = new User()
                {
                    id = u.usr_id,
                    name = u.usr_name,
                    role = u.usr_role
                };
            return output;
        }
    }
    private static class DEFAULTS
    {
        public const User.Role role = User.Role.friend;
    }
}
