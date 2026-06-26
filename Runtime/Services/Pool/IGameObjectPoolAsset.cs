using UnityEngine;

namespace IFramework
{
    public interface IGameObjectPoolAsset
    {
        AsyncTask<GameObject> LoadAsset(string key);
        void ReleaseAsset(string key, GameObject asset);
    }

}
