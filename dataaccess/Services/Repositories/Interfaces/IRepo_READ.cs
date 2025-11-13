using System.Data;

namespace Services.Repositories.Interfaces;

public interface IRepo_READ<T>
{
    T[] GetAll();
    T? GetUserByName(string name);
}
