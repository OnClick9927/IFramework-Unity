/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IFramework.AB
{
    [MonoSingletonPath("IFramework/AB")]
	public class ABAssets:MonoBehaviour,ISingleton
	{
        private static ABAssets Instance { get { return MonoSingletonProperty<ABAssets>.Instance; } }
        private List<string> allAssetNames { get { return Instance.manifest.allAssets; } }
        private List<string> allBundleNames { get { return Instance.manifest.allBundles; } }
        public static string GetBundleName(string assetPath) { return Instance.manifest.GetBundleName(assetPath); }
        public static string GetAssetName(string assetPath) { return Instance.manifest.GetAssetName(assetPath); }
        private ABManifest manifest;
        void ISingleton.OnSingletonInit()
        {
            manifest = new ABManifest();

        }
        public static bool Init()
        {
#if UNITY_EDITOR
            if (ABTool.ActiveBundleMode) return InitializeBundle();
            return true;
#else
			return InitializeBundle();
#endif
        }
        private static bool InitializeBundle()
        {
            //string relativePath = Path.Combine(ABTool.AssetBundleOutPutPath, ABTool.CurrentPlatformName);
            string relativePath = Path.Combine(ABTool.AssetBundleOutPutPath,"");

            var url =
#if UNITY_EDITOR
                relativePath + "/";
#else
				Path.Combine(Application.streamingAssetsPath, relativePath) + "/"; 
#endif
            if (ABBundles.Init(url))
            {
                var bundle = ABBundles.LoadSync("help");
                if (bundle != null)
                {
                    
                    TextAsset asset = bundle.LoadAsset<TextAsset>(ABTool.ManifestPath.GetFileName());
                    if (asset != null)
                    {
                        using (var reader = new StringReader(asset.text))
                        {
                            Instance.manifest.Load(reader.ReadToEnd());
                            reader.Close();
                        }
                        bundle.Release();
                        Resources.UnloadAsset(asset);
                        asset = null;
                    }
                    return true;
                }
                throw new FileNotFoundException("assets manifest not exist.");
            }
            throw new FileNotFoundException("bundle manifest not exist.");
        }

        public static void InitAsync(Action onComplete)
        {
            string relativePath = Path.Combine(ABTool.AssetBundleOutPutPath, ABTool.CurrentPlatformName);
            var url =
#if UNITY_EDITOR
                relativePath + "/";
#else
				Path.Combine(Application.streamingAssetsPath, relativePath) + "/"; 
#endif
            Instance.StartCoroutine(ABBundles.InitAsync(url, bundle =>
            {
                if (bundle != null)
                {
                    var asset = bundle.LoadAsset<TextAsset>("Manifest.txt");
                    if (asset != null)
                    {
                        using (var reader = new StringReader(asset.text))
                        {
                            Instance.manifest.Load(reader.ReadToEnd());
                            reader.Close();
                        }
                        bundle.Release();
                        Resources.UnloadAsset(asset);
                        asset = null;
                    }
                }
                if (onComplete != null) onComplete();
            }));
        }
        public static void Unload(ABAsset asset)
        {
            asset.Release();
        }
        public static ABAsset Load<T>(string path) where T : UnityEngine.Object
        {
            return Load(path, typeof(T));
        }
        public static ABAsset LoadAsync<T>(string path)
        {
            return LoadAsync(path, typeof(T));
        }

        public static ABAsset Load(string path, Type type)
        {
            return Load(path, type, false);
        }
        public static ABAsset LoadAsync(string path, Type type)
        {
            return Load(path, type, true);
        }
        private static ABAsset Load(string path, Type type, bool isAsynnc)
        {
            ABAsset asset = Instance.assets.Find(obj => { return obj.AssetPath == path; });
            if (asset == null)
            {
#if UNITY_EDITOR
                if (ABTool.ActiveBundleMode)
                    if (isAsynnc)
                        asset= new ABAsyncBundleAsset(path, type);
                    else
                        asset= new ABBundleAsset(path, type);
                else
                    asset = new ABAsset(path, type);
#else
                if (isAsynnc)
                    asset = new ABAsyncBundleAsset(path, type);
                else
                    asset = new ABBundleAsset(path, type);
#endif
                Instance.assets.Add(asset);
                asset.Load();
            }
            asset.Retain();
            return asset;
        }

        private List<ABAsset> assets = new List<ABAsset>();
        IEnumerator gc = null;

        IEnumerator GC()
        {
            System.GC.Collect();
            yield return 0;
            yield return Resources.UnloadUnusedAssets();
        }
        private void Update()
        {
            bool removed = false;
            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                if (!asset.IsDone && asset.IsUnused)
                {
                    asset.UnLoad();
                    asset = null;
                    assets.RemoveAt(i);
                    i--;
                    removed = true;
                }
            }

            if (removed)
            {
                if (gc != null)
                {
                    StopCoroutine(gc);
                }
                gc = GC();
                StartCoroutine(gc);
            }

            ABBundles.ClearUnUseBundles();
        }

        public void Dispose()
        {
            
        }
    }
}
