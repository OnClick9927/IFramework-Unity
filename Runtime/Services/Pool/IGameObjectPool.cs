namespace IFramework
{
    public interface IGameObjectPool:IService
    {
        void Clear(string key);
        void ClearAll();
        AsyncTask<T> Get<T>(string key) where T : GameObjectView, IPoolAbleGameObjectView, new();
        AsyncTask<bool> Prepare(string key);
        void Set(IPoolAbleGameObjectView view);
    }
}