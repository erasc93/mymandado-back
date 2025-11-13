using System.Data;

namespace core_mandado.Users;

public interface IRepo_Users
{
    bool Login(LoginInfo login);
    User GetCurrent();
    User? GetUserByName(string userName);

    User[] GetAll();
    User AddByName(string userName);

    void Add(ref User item);

    bool Delete(User user);
    void Update(User updated);


}
