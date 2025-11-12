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
                            IRepo_Cart _repoCart,
                            IRepo_CartItems _repo_CartItems
                        ) : ARepository(query),
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
            u = new User()
            {
                id = 1,
                name = "manu",
                role = User.Role.admin
            };
        return u;
    }
    public User? GetUserByName(string userName, IDbConnection? conn = null, IDbTransaction? trans = null)
    {
        string query;
        Dictionary<string, object>
            param = new Dictionary<string, object>()
                    {
                        {"@username",userName},
                    };
        query = $"select * from USERS where usr_name=@username";
        MND_USERS?
            mndusers = _query.free.Query<MND_USERS>(query, param, conn, trans)
            .FirstOrDefault();
        User?
            output = mndusers is not null
                    ? Factory.ToView(mndusers)
                    : null;
        return output;
    }
    public User[] GetAll(IDbConnection? conn = null, IDbTransaction? trans = null)
    {
        MND_USERS[]
            mndusers = _query.crud.GetAll<MND_USERS>(conn, trans);
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
    public User AddByName(string userName, IDbConnection c, IDbTransaction t)
    {

        MND_USERS
            mnduser = new MND_USERS
            {
                usr_name = userName,
                usr_role = User.Role.friend
            };
        _query.crud.Add<MND_USERS>(ref mnduser, c, t);

        User
            output = new User()
            {
                id = mnduser.usr_id,
                name = mnduser.usr_name,
                role = mnduser.usr_role
            };


        Cart cart = _repoCart.AddNew(output,0,c, t);

        return output;
    }
    //private
    public User? AddByName(string userName)
    {
        User?
            output = null;

        _query.ExecuteInTransaction((c, t) =>
        {
            output = AddByName(userName, c, t);
        });

        return output;
    }
    
    public User AddByNameSafe(string userName, IDbConnection? conn = null, IDbTransaction? trans = null)
    {
        MND_USERS mnduser;
        int? userid;
        User output;

        mnduser = new MND_USERS
        {
            usr_name = userName,
            usr_role = DEFAULTS.role
        };
        _query.crud.Add(ref mnduser, conn, trans);
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
        item = AddByName(item.name)!;
    }
    public void Add(ref User item, IDbConnection conn , IDbTransaction trans )
    {
        item = AddByName(item.name,conn,trans);
    }

    public bool Delete(User user, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        bool success;
        MND_USERS mnduser;

        mnduser = new MND_USERS()
        {
            usr_id = user.id,
            usr_name = user.name,
            usr_role = user.role,
        };

        success = _query.crud.Delete(mnduser, connection, transaction);

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
