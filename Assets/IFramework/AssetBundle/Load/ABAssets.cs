/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IFramework.AB
{
    [MonoSingletonPath("IFramework/AssetBundle")]
	public class ABAssets:MonoSingletonPropertyClass<ABAssets>
	{
        public class ABBundles
        {
            private string rootPath;
            public  AssetBundleManifest manifest;
            Dictionary<string, Bundle> bundles = new Dictionary<string, Bundle>();

            public bool Init(string path)
            {
                rootPath = path;
                Bundle manifestBundle = Load(ABTool.CurrentPlatformName, true, false);
                if (manifestBundle == null || manifestBundle.error != null) return false;
                manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest == null) return false;
                return true;
            }
            public IEnumerator InitAsync(string path, Action<Bundle> OnComplete)
            {
                rootPath = path;
                Bundle manifestBundle = Load(ABTool.CurrentPlatformName, true, true);
                yield return manifestBundle;
                if (manifestBundle == null || manifestBundle.error != null)
                {
                    Log.E("Manifest Load Err");
                    yield break;
                }
                manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest == null)
                    yield break;
                Bundle bundle = Load(ABTool.MainAssetBundleBuildName, false, true);
                yield return bundle;
                if (bundle == null || bundle.error != null)
                    yield break;
                if (OnComplete != null) OnComplete(bundle);
            }


            private Bundle Load(string bundleName, bool loadManifest, bool isAsync)
            {
                if (!loadManifest)
                    if (manifest == null)
                        throw new Exception("Bundles , Please initialize AssetBundleManifest by calling Bundles.Initialize()");
                string bundlePath = rootPath.CombinePath(bundleName);
                Bundle bundle;
                if (!bundles.TryGetValue(bundleName, out bundle))
                {
                    Hash128 version = loadManifest ? new Hash128(1, 0, 0, 0) : manifest.GetAssetBundleHash(bundleName);
                    if (bundle == null)
                    {
                        List<Bundle> dpdenceBundles=new List<Bundle>();
                        if (!loadManifest)
                        {
                            string[] dps = manifest.GetAllDependencies(bundleName);
                            if (dps.Length > 0)
                                for (int i = 0; i < dps.Length; i++)
                                    dpdenceBundles.Add(Load(dps[i], false, isAsync));
                        }
                        if (bundlePath.StartsWith("http://") ||
                          bundlePath.StartsWith("https://") ||
                          bundlePath.StartsWith("file://") ||
                          bundlePath.StartsWith("ftp://"))
                        {
                            bundle = new WebRequestBundle(bundlePath, bundleName, dpdenceBundles, version);

                        }
                        else
                        {
                            if (isAsync)
                                bundle = new AsyncBundle(bundlePath, bundleName, dpdenceBundles ,version);
                            else
                                bundle = new Bundle(bundlePath, bundleName, dpdenceBundles, version);
                        }
                        bundles.Add(bundleName, bundle);
                        bundle.Load();
                    }
                }
                bundle.Retain();
                return bundle;
            }
            public Bundle LoadAsync(string abName)
            {
                return Load(abName, false, true);
            }
            public Bundle LoadSync(string abName)
            {
                return Load(abName, false, false);
            }



            private Queue<Bundle> destoryQueue = new Queue<Bundle>();
            public void ClearUnUseBundles()
            {
                foreach (var item in bundles)
                    if (item.Value.IsDone && item.Value.IsUnused)
                        destoryQueue.Enqueue(item.Value);
                while (destoryQueue.Count > 0)
                {
                    Bundle bundle = destoryQueue.Dequeue();
                    bundles.Remove(bundle.name);
                    bundle.UnLoad();
                    bundle.Dispose();
                    bundle = null;
                }
            }
        }

        private class ManifestXML
        {
            //存储asset对应allBundle的index
            private readonly Dictionary<string, int> amap;
            //存储assetbundleBuid所有asset在allAssets的index
            private readonly Dictionary<string, List<int>> bmap;
            public List<string> allAssets;
            public List<string> allBundles;

            public ManifestXML()
            {
                amap = new Dictionary<string, int>();
                bmap = new Dictionary<string, List<int>>();
                allAssets = new List<string>();
                allBundles = new List<string>();
            }
            public void Load(string txt)
            {
                amap.Clear();
                bmap.Clear();

                allAssets.Clear();
                allBundles.Clear();
                List<ManifestXmlContent> list = Xml.ToObject<List<ManifestXmlContent>>(txt);
                foreach (var content in list)
                {
                    allBundles.Add(content.assetBundleName);
                    bmap.Add(content.assetBundleName, new List<int>());
                    bmap[content.assetBundleName].Add(allAssets.Count - 1);
                    foreach (var asset in content.assetNames)
                    {
                        allAssets.Add(asset);
                        bmap[content.assetBundleName].Add(allAssets.Count - 1);
                        amap[asset] = allBundles.Count - 1;
                    }
                }
            }


            public bool ContainsBundle(string bundle) { return bmap.ContainsKey(bundle); }
            public bool ContainsAsset(string assetPath) { return amap.ContainsKey(assetPath); }
            public string[] GetBundleAssets(string bundleName)
            {
                return Array.ConvertAll<int, string>(bmap[bundleName].ToArray(), input =>
                {
                    return allAssets[input];
                });
            }
            public string GetBundleName(string assetPath) { return allBundles[amap[assetPath]]; }
            public string GetAssetName(string assetPath) { return Path.GetFileName(assetPath); }
        }

        private List<string> allAssetNames { get { return Instance.manifestXML.allAssets; } }
        private List<string> allBundleNames { get { return Instance.manifestXML.allBundles; } }
        public static string GetBundleName(string assetPath) { return Instance.manifestXML.GetBundleName(assetPath); }
        public static string GetAssetName(string assetPath) { return Instance.manifestXML.GetAssetName(assetPath); }
        private ManifestXML manifestXML;
        public static ABBundles Bundles { get; private set; }
        protected override void OnSingletonInit()
        {
            manifestXML = new ManifestXML();
            Bundles = new ABBundles();
        }

        private string InitPath
        {
            get
            {
                string relativePath = ABTool.AssetBundleOutPutPath;
                var url =
#if UNITY_EDITOR
                relativePath;
#else
				Path.Combine(Application.streamingAssetsPath, relativePath) ;
#endif
                return relativePath;
            }
        }
        public static bool Init()
        {
#if UNITY_EDITOR
            if (ABTool.ActiveBundleMode) return Instance.InitializeBundle();
            return true;
#else
			return InitializeBundle();
#endif
        }
        private bool InitializeBundle()
        {
            if (!Bundles.Init(InitPath))
                throw new FileNotFoundException("bundle manifest not exist.");
            var bundle = Bundles.LoadSync(ABTool.MainAssetBundleBuildName);
            if (bundle == null)
                throw new FileNotFoundException("assets manifest not exist.");
            TextAsset xml = bundle.LoadAsset<TextAsset>(ABTool.ManifestPath.GetFileName());
            if (xml == null)
                throw new FileNotFoundException("assets manifest not exist.");
            using (var reader = new StringReader(xml.text))
            {
                Instance.manifestXML.Load(reader.ReadToEnd());
                reader.Close();
            }
            bundle.Release();
            Resources.UnloadAsset(xml);
            xml = null;
            return true;
        }

        public static void InitAsync(Action onComplete)
        {

            Instance.StartCoroutine(Bundles.InitAsync(Instance.InitPath, bundle =>
            {
                if (bundle == null)
                {
                    if (onComplete != null) onComplete();
                    return;
                }
                var asset = bundle.LoadAsset<TextAsset>(ABTool.ManifestPath.GetFileName());
                if (asset != null)
                {
                    using (var reader = new StringReader(asset.text))
                    {
                        Instance.manifestXML.Load(reader.ReadToEnd());
                        reader.Close();
                    }
                    bundle.Release();
                    Resources.UnloadAsset(asset);
                    asset = null;
                }
                if (onComplete != null) onComplete();

            }));
        }
        public static void Unload(Asset asset)
        {
            asset.Release();
        }
        public static Asset Load<T>(string path) where T : UnityEngine.Object
        {
            return Load(path, typeof(T));
        }
        public static Asset LoadAsync<T>(string path)
        {
            return LoadAsync(path, typeof(T));
        }

        public static Asset Load(string path, Type type)
        {
            return Load(path, type, false);
        }
        public static Asset LoadAsync(string path, Type type)
        {
            return Load(path, type, true);
        }

        private static Asset Load(string path, Type type, bool isAsynnc)
        {
            Asset asset = Instance.assets.Find(obj => { return obj.assetPath == path; });
            if (asset == null)
            {
                if ((Application.platform== RuntimePlatform.LinuxEditor 
                    || Application.platform== RuntimePlatform.OSXEditor 
                    || Application.platform== RuntimePlatform.WindowsEditor) 
                    && !ABTool.ActiveBundleMode)
                {
                    asset = new Asset(path, type);
                }
                else
                {
                    if (isAsynnc)
                        asset = new AsyncBundleAsset(path, type);
                    else
                        asset = new BundleAsset(path, type);
                }
                Instance.assets.Add(asset);
                asset.Load();
            }
            asset.Retain();
            return asset;
        }

        private List<Asset> assets = new List<Asset>();
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

            Bundles.ClearUnUseBundles();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
