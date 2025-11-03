namespace repositories.ICRUD;

public interface IRepo_CREATE<T>
{
    void Add(ref T item);
    T[] GetAll();
}
