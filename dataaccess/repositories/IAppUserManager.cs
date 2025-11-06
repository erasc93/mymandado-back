using core_mandado.models;
using core_mandado.repositories;
using dbaccess;
using information_schema.tables;

namespace repositories;


//public class Repo_Users : IRepo_Users
//{
//    private ICRUDQuery _dapperCRUD { get; init; }

//    public Repo_Users(ICRUDQuery dapperCRUD)
//    {
//        _dapperCRUD = dapperCRUD;
//    }


//    //public User[] GetAll()
//    //{
//    //    dynamic
//    //        x = _dapperCRUD.Query<dynamic>("show databases;");

//    //    // HARD CODED FOR TEST PURPOSE ONLY
//    //    return Enumerable.Range(1, 6)
//    //                    .Select(index => new User
//    //                    {
//    //                        username = "esalas",
//    //                        password = "fakepassword",
//    //                        email = "email@email.fr",
//    //                        firstname = "emmanuel"
//    //                    })
//    //                .ToArray();
//    //}
//    public void AddUser(User user)
//    {
//        _dapperCRUD.Add(user);
//    }

//    public void Delete(User user)
//    {
//        bool
//            deleted = _dapperCRUD.Delete(user);
//        if (deleted)
//        {
//            return;
//        }
//        else
//        {
//            HandleNotDeletedProduct(user);
//        }


//    }
//    private void HandleNotDeletedProduct(User user)
//    {
//        User dataBaseUser;

//        dataBaseUser = GetUserByID(user.id);
//        if (dataBaseUser is not null)
//        {
//            throw new Exception($"product {user.name} could not be deleted.");
//        }
//        else
//        {
//            throw new ArgumentException($"product {user.id} cannot be found in database.");
//        }

//    }
//    public void Update(User user)
//    {
//        _dapperCRUD.Update(user);
//    }
//    public User GetUserByID(int id)
//    {
//        User
//            output = _dapperCRUD.GetById<User>(id);
//        return output;
//    }
//    public User[] GetAll()
//    {
//        User[] output;
//        output = _dapperCRUD.GetAll<User>().ToArray();

//        return output;
//    }

//    public User GetAppUserByUserName(string username)
//    {
//        User userByName;
//        string query_SelectByuserName =
//        $"select * from Users where username like('{username}') limit 1;";

//        userByName = _dapperCRUD.Query<User>(query_SelectByuserName).First();

//        return userByName;
//    }

//    public User GetAppUserByID(int id)
//    {
//        User userByName;
//        string query_SelectByuserName =
//        $"select * from USERS where usr_id={id} limit 1;";

//        userByName = _dapperCRUD.Query<User>(query_SelectByuserName).First();

//        return userByName;
//    }

//}


