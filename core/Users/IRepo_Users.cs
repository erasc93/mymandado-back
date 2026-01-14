using System.Data;

namespace core_mandado.Users;

public interface IRepo_Users
{
    bool Login(LoginInfo login);
    User GetUserByName(string userName);

    User[] GetAll();
    User AddByName(LoginInfo loginInfo,User.Role role=User.Role.friend);


    bool Delete(User user);
    void Update(User updated);


}
