/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections;
using UnityEngine;
using IFramework.AB;
namespace IFramework_Demo
{
	public class ABExample:MonoBehaviour
	{
        void Start()
        {
<<<<<<< HEAD
            // 初始化，判断是否初始化成功。
            if (!ABAssets.Init())
            {
                Debug.LogError("Assets.Initialize falied.");
            }
            // 协程加载资源
            StartCoroutine(Load());
=======
            //if (ABAssets.Init())
            //{
            //    StartCoroutine(Load());

            //}
            ABAssets.InitAsync(() =>
            {
                StartCoroutine(Load());
            });

>>>>>>> acb3275adbbd7eb9ea0ca7fc2286f1da44a5e9ab
        }
        [SerializeField] string assetPath = "Assets/Examples/ABExample/Logo.prefab";
        IEnumerator Load()
        {
            // 异步加载 用于加载 内存较大资源。
            var asset = ABAssets.LoadAsync<GameObject>(assetPath);

            if (asset != null)
            {
                while (!asset.IsDone)
                {
                    yield return 0;
                }

                var prefab = asset.asset;
                if (prefab != null)
                {
                    var go = Instantiate(prefab) as GameObject;
                    //登记 加载的资源 ，方便在 调用销毁方法时释放。
                    ReleaseAssetOnDestroy.Register(go, asset);
                    GameObject.Destroy(go, 10);
                }
            }

            yield return new WaitForSeconds(11);

            //同步加载资源
            asset = ABAssets.Load<GameObject>(assetPath);
            if (asset != null)
            {
                var prefab = asset.asset;
                if (prefab != null)
                {
                    var go = Instantiate(prefab) as GameObject;
                    ReleaseAssetOnDestroy.Register(go, asset);
                    GameObject.Destroy(go, 3);
                }
            }
        }

    }
}
