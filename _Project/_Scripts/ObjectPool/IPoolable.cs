public interface IPoolable
{
    void OnGetFromPool();   // когда объект достают из пула
    void OnReturnToPool();  // когда объект возвращают в пул (сброс)
}