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
            //if (ABAssets.Init())
            //{
            //    StartCoroutine(Load());

            //}
            ABAssets.InitAsync(() =>
            {
                StartCoroutine(Load());
            });

        }
        [SerializeField] string assetPath = "Assets/Examples/ABExample/Logo.prefab";
        IEnumerator Load()
        {
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
                    ReleaseAssetOnDestroy.Register(go, asset);
                    GameObject.Destroy(go, 10);
                }
            }

            yield return new WaitForSeconds(11);

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
