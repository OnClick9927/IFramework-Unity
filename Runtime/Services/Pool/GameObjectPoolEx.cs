namespace IFramework
{
    public static class GameObjectPoolEx
    {
        public static Game UseGameObjectPool(this Game game, IGameObjectPoolAsset asset)
        {
            GameObjectPool pool = new GameObjectPool(asset, game.transform);
            game.Use<IGameObjectPool>(pool);
            return game;
        }
        public static IGameObjectPool GameObjectPool(this Game game) => game.GetRequiredService<IGameObjectPool>();
    }

}
