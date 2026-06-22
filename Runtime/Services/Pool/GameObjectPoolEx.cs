namespace IFramework
{
    public static class GameObjectPoolEx
    {
        public static Game UseGameObjectPool(this Game game, IGameObjectPoolAsset asset)
        {
            GameObjectPool pool = new GameObjectPool(asset, game.transform);
            game.UseService(pool);
            return game;
        }
        public static AsyncTask<bool> PreparePool(this Game game, string key)
        {
            GameObjectPool service = game.GetService<GameObjectPool>();
            return service.Prepare(key);
        }
        public static void ClearPool(this Game game, string key)
        {
            GameObjectPool service = game.GetService<GameObjectPool>();
            service?.Clear(key);
        }
        public static void ClearPools(this Game game)
        {
            GameObjectPool service = game.GetService<GameObjectPool>();
            service?.ClearAll();
        }
        public static AsyncTask<T> GetFromPool<T>(this Game game, string key) where T : GameObjectView, IPoolAbleGameObjectView, new()
        {
            GameObjectPool service = game.GetService<GameObjectPool>();
            return service.Get<T>(key);
        }
        public static void SetToPool(this Game game, IPoolAbleGameObjectView view)
        {
            GameObjectPool service = game.GetService<GameObjectPool>();
            service?.Set(view);
        }
    }

}
