using System.Data;

namespace Services.Repositories.Interfaces;

public interface IRepo_CREATE<T>
{
    void Add(ref T item);
}
