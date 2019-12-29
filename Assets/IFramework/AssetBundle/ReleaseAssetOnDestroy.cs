/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.AB
{
	public class ReleaseAssetOnDestroy:MonoBehaviour
	{
        public Asset asset;

        public static ReleaseAssetOnDestroy Register(GameObject go, Asset asset)
        {
            ReleaseAssetOnDestroy component = go.GetComponent<ReleaseAssetOnDestroy>();
            if (component == null)
            {
                component = go.AddComponent<ReleaseAssetOnDestroy>();
            }
            component.asset = asset;
            return component;
        }

        private void OnDestroy()
        {
            if (asset != null)
            {
                asset.Release();
                asset = null;
            }
        }
    }
}
