using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFramework;
using IFramework.KK;
using System.Threading.Tasks;
using System;
public class PoolTest : MonoBehaviour, IGameObjectPoolAsset
{
    public GameObject prefab;

    public async AsyncTask<GameObject> LoadAsset(string key)
    {
        await AsyncTask.CompletedTask;
        return prefab;
    }

    public void ReleaseAsset(string key, GameObject asset)
    {

    }

    void Start()
    {
        GameObjectPool.Instance.SetAsset(this);
        GameObjectPool.Instance.Prepare(prefab.name);
    }
    private Queue<CubeView> views = new Queue<CubeView>();
    // Update is called once per frame
    async void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            var view = await GameObjectPool.Instance.Get<CubeView>(prefab.name);
            view.gameObject.name = Guid.NewGuid().ToString();
            views.Enqueue(view);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (views.Count > 0)
            {
                var view = views.Dequeue();
                GameObjectPool.Instance.Set(view);
            }
        }
    }
}
