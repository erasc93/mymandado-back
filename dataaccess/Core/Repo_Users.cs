using core_mandado.Cart;
using core_mandado.Users;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories.Generics;
using System.Data;
using _crypt = BCrypt.Net;
using Microsoft.Extensions.Configuration;
namespace core;

public class Repo_Users(
                            IQueries query,
                            IRepo_Cart _repoCart,
                            IConfiguration _config
                        ) : ARepository(query),
                            IRepo_Users
{
    public bool Login(LoginInfo login)
    {

        (User? user, string storedHash) = GetUserWithHash(login.username);

        string password = login.password ?? "x582y" ?? throw new Exception("should provide password");
        bool isValidCredentials = _crypt.BCrypt.Verify(password, storedHash);

        if (!isValidCredentials) throw new Exception();
        return isValidCredentials;
    }
    public User GetUserByName(string userName)
    {
        string query;
        Dictionary<string, object>
            param = new()
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

        if (output is null) throw new Exception($"user '{userName}' could not be found");
        return output;
    }

    public (User?, string hash) GetUserWithHash(string userName)
    {
        string query;
        Dictionary<string, object>
            param = new()
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
        return (output, mndusers.usr_hashword);
    }
    public User[] GetAll()
    {
        MND_USERS[]
            mndusers = _query.crud.GetAll<MND_USERS>();
        User[]
            output = [.. (from u in mndusers
                      select new User()
                      {
                          id = u.usr_id,
                          name = u.usr_name,
                          role = u.usr_role
                      }
                  )];
        return output;
    }
    public User AddByName(LoginInfo loginInfo, User.Role role)
    {
        MND_USERS
            mnduser = new()
            {
                usr_name = loginInfo.username,
                usr_hashword = _crypt.BCrypt.HashPassword(loginInfo.password ?? "1331", _crypt.BCrypt.GenerateSalt()),
                usr_role = role
            };
        _query.crud.Add<MND_USERS>(ref mnduser);

        User
            output = new()
            {
                id = mnduser.usr_id,
                name = mnduser.usr_name,
                role = mnduser.usr_role
            };


        _repoCart.AddEmptyCart(output, numero: 0, name: "cart", description: "");

        return output;
    }

    //public void Add(ref User item,string password)
    //{
    //    item = AddByName(new() { username=item.name,password=password});
    //}

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
                output = new()
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
        public const User.Role ROLE = User.Role.friend;
    }
}
