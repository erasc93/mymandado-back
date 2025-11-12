using System.Data;

namespace core_mandado.Users;

public interface IRepo_Users
{
    bool Login(LoginInfo login);
    User GetCurrent();
    User? GetUserByName(string userName, IDbConnection? conn = null, IDbTransaction? trans = null);

    User[] GetAll(IDbConnection? conn = null, IDbTransaction? trans = null);
    User AddByName(string userName, IDbConnection conn, IDbTransaction trans);
    User AddByName(string userName);
    void Add(ref User item);
    bool Delete(User user, IDbConnection? conn=null, IDbTransaction? trans=null);
    void Update(User updated);

}
