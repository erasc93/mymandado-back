using System.Data;

namespace Services.Repositories.Interfaces;

public interface IRepo_UPDATE<T>
{
    void Update(T updated);
}
