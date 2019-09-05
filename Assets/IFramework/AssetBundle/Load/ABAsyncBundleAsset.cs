/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using UnityEngine;

namespace IFramework.AB
{
    public class ABAsyncBundleAsset : ABBundleAsset
    {
        private AssetBundleRequest abRequest;
        private int loadState;

        public ABAsyncBundleAsset(string path, System.Type type) : base(path, type) { }
        public override void OnLoad()
        {
            bundle = ABBundles.LoadAsync(ABAssets.GetBundleName(AssetPath));
            // abRequest = bundle.LoadAssetAsync(ABAssets.Instance.GetAssetName(AssetPath), AssetType);
        }
        public override void OnUnLoad()
        {
            base.OnUnLoad();
            abRequest = null;
            loadState = 0;
        }
        public override bool IsDone
        {
            get
            {

                if (loadState == 2) return true;
                if (bundle.Err != null) return true;
                for (int i = 0; i < bundle.dpBundles.Count; i++) // 依赖没有错误
                {
                    if (bundle.dpBundles[i].Err != null) return true;
                }
                if (loadState == 1)
                {
                    if (abRequest.isDone)
                    {
                        Asset = abRequest.asset;
                        loadState = 2;
                        return true;
                    }
                }
                else
                {
                    bool allReady = true;
                    if (!bundle.IsDone) allReady = false;
                    if (bundle.dpBundles.Count > 0)
                    {
                        if (!bundle.dpBundles.TrueForAll(bundle => bundle.IsDone))
                        {
                            allReady = false;
                        }
                    }
                    if (allReady)
                    {
                        abRequest = bundle.LoadAssetAsync(Path.GetFileName(AssetPath), AssetType);
                        if (abRequest == null)
                        {

                            loadState = 2;
                            return true;
                        }
                        loadState = 1;
                    }
                }
                return false;
            }
        }
    }

}
