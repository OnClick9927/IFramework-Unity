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
using System.Collections.Generic;
using System.IO;

namespace IFramework.AB
{
	public class ABManifest
	{
        public bool ContainsBundle(string bundle) { return bmap.ContainsKey(bundle); }
        public bool ContainsAsset(string assetPath) { return amap.ContainsKey(assetPath); }
        public string[] GetBundleAssets(string bundleName)
        {
            return Array.ConvertAll<int, string>(bmap[bundleName].ToArray(), input =>
            {
                return allAssets[input];
            });
        }



        public  readonly Dictionary<string, int> amap = new Dictionary<string, int>();
        public  readonly Dictionary<string, List<int>> bmap = new Dictionary<string, List<int>>();
        public List<string> allAssets = new List<string>();
        public List<string> allBundles = new List<string>();
        public string GetBundleName(string assetPath) { return allBundles[amap[assetPath]]; }
        public string GetAssetName(string assetPath) { return Path.GetFileName(assetPath); }

        private void Resetval()
        {
            amap.Clear();
            bmap.Clear();

            allAssets.Clear();
            allBundles.Clear();
        }
        public void Load(string txt)
        {
            Resetval();
            List<ABManifestContent> list = Xml.ToObject<List<ABManifestContent>>(txt);
            foreach (var item in list)
            {
                allBundles.Add(item.assetBundleName);
                bmap.Add(item.assetBundleName, new List<int>());
                bmap[item.assetBundleName].Add(allAssets.Count - 1);
                foreach (var asset in item.assetNames)
                {
                    allAssets.Add(asset);
                    bmap[item.assetBundleName].Add(allAssets.Count - 1);
                    amap[asset] = allBundles.Count - 1;
                }
            }

            //allBundles = bundles.ToArray();
            //allAssets = assets.ToArray();
        }
    }
}
