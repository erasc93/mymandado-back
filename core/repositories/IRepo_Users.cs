using core_mandado.models;

namespace core_mandado.repositories;

public interface IRepo_Users
{
    User GetCurrent();

    User? GetUserByName(string userName);
    User[] GetAll();
    User AddByName(string userName);
    void Add(ref User item);
    bool Delete(User user);
    void Update(User updated);
    
}
