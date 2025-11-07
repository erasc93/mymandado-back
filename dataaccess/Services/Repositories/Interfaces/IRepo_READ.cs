namespace Services.Repositories.Interfaces;

public interface IRepo_READ<T>
{
    T? GetUserByName(string name);
    T[] GetAll();
}
