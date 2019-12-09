/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.AB
{
    public delegate string OverrideDataPath(string BundleName);
	public class ABBundles
	{
        public static string[] activeVariants { get; private set; }

        public static string datapath;
        public static AssetBundleManifest manifest;
        public static event OverrideDataPath overrideDataPath;
        public static string[] GetAllDependences(string abName)
        {
            return manifest.GetAllDependencies(abName);
        }
        private static string GetOverrideDataPath(string path)
        {
            if (overrideDataPath!=null)
            {
                foreach (OverrideDataPath method in overrideDataPath.GetInvocationList())
                {
                    string res = method(path);
                    if (res != null)
                        return res;
                }
            }
            return datapath;
        }
        public static bool Init(string path)
        {
            activeVariants = new string[0];
            datapath = path;
            ABBundle manifestBundle = Load(ABTool.CurrentPlatformName, true, false);
            if (manifestBundle == null || manifestBundle.Err != null) return false;
            manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null) return false;
            return true;
        }
        public static IEnumerator InitAsync(string path,Action<ABBundle> OnComplete)
        {
            activeVariants = new string[0];

            datapath = path;
            ABBundle manifestBundle = Load(ABTool.CurrentPlatformName, true, true);
            yield return manifestBundle;
            if (manifestBundle == null || manifestBundle.Err != null)
            {
                Log.E("Manifest Load Err");
                yield break;
            }
            manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null)
            {
                yield break;
            }
            ABBundle bundle = Load("manifest", false, true);
            yield return bundle;
            if (bundle==null || bundle.Err!=null)
            {
                yield break;
            }
            if (OnComplete != null) OnComplete(bundle);
            
        }
        private static void LoadDependBundle(ABBundle bundle, string abName, bool isAsync)
        {
            string[] dps = manifest.GetAllDependencies(abName);
            if (dps.Length <= 0) return;
            for (int i = 0; i < dps.Length; i++)
            {
                bundle.dpBundles.Add(Load(dps[i], false, isAsync));
            }
        }
        private static ABBundle Load(string abName, bool loadManifest, bool isAsync)
        {
            if (!loadManifest)
            {
                if (manifest==null)
                {
                    Log.E( "Bundles , Please initialize AssetBundleManifest by calling Bundles.Initialize()");
                    return null;
                }
                abName = RemapVariantName(abName);
            }
            string path =  GetOverrideDataPath(abName)+abName;
            ABBundle bundle;
            if (!bundles.TryGetValue(abName,out bundle))
            {
                Hash128 version = loadManifest ? new Hash128(1, 0, 0, 0) : manifest.GetAssetBundleHash(abName);
                if (bundle == null)
                {
                    if (path.StartsWith("http://") ||
                      path.StartsWith("https://") ||
                      path.StartsWith("file://") ||
                      path.StartsWith("ftp://"))
                    {
#if UNITY_2017_4_OR_NEWER
                        bundle = new ABwebRequestBundle(path, version);
#else
                        bundle = new ABWWWBundle(path, version);
#endif
                    }
                    else
                    {
                        if (isAsync)
                            bundle = new ABAsyncBundle(path, version);
                        else
                            bundle = new ABBundle(path, version);
                    }
                    bundle.ABName = abName;
                    bundles.Add(abName, bundle);
                    bundle.Load();
                    if (!loadManifest)
                    {
                        LoadDependBundle(bundle, abName, isAsync);
                    }
                }
            }
            bundle.Retain();
            return bundle;
        }
        private static string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
            // Get base bundle name
            string baseName = assetBundleName.Split('.')[0];
            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                string curBaseName = curSplit[0];
                string curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;
                int found = System.Array.IndexOf(activeVariants, curVariant);
                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;
                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            if (bestFit == int.MaxValue - 1)
            {
                Log.W("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
            }
            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            return assetBundleName;
        }

        public static ABBundle LoadAsync(string abName)
        {
            return Load(abName, false, true);
        }
        public static ABBundle LoadSync(string abName)
        {
            return Load(abName, false, false);
        }

        private static void UnLoadDependBundles(ABBundle bundle)
        {
            for (int i = 0; i < bundle.dpBundles.Count; i++)
            {
                bundle.dpBundles[i].Release();
            }
            bundle.dpBundles.Clear();

        }

        readonly internal static Dictionary<string, ABBundle> bundles = new Dictionary<string, ABBundle>();
        private readonly static List<ABBundle> bundleToDestroy = new List<ABBundle>();

        public static void ClearUnUseBundles()
        {
            foreach (var item in bundles)
            {
                if (item.Value.IsDone && item.Value.IsUnused)
                {
                    bundleToDestroy.Add(item.Value);
                }
            }
            for (int i = 0; i < bundleToDestroy.Count; i++)
            {
                ABBundle bundle = bundleToDestroy[i];
                bundles.Remove(bundle.ABName);
                bundle.UnLoad();
                UnLoadDependBundles(bundle);
                bundle = null;
            }
            bundleToDestroy.Clear();
        }
    }
}
