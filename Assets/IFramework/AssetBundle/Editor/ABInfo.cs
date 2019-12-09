/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Linq;
using System.IO;

namespace IFramework.AB
{
    internal class ABInfo:ScriptableObject
	{
        public ABCollectInfo CollectInfo=new ABCollectInfo();
        public ABTempInfo TempInfo = new ABTempInfo();
    }
    [Serializable]
    internal class ABTempInfo
    {
        [SerializeField]
        public List<AssetDependenceInfo> dps = new List<AssetDependenceInfo>();
        public List<ABBuildItem> Builds = new List<ABBuildItem>();

        private AssetDependenceInfo ContainsAsset(string assetpath)
        {
            for (int i = 0; i < dps.Count; i++)
            {
                if (dps[i].AssetPath==assetpath)
                {
                    return dps[i];
                }
            }
            return default(AssetDependenceInfo);
        }
        private ABBuildItem ContainsBundle(string bundleName)
        {
            for (int i = 0; i < Builds.Count; i++)
            {
                if (Builds[i].assetBundleName == bundleName)
                {
                    return Builds[i];
                }
            }
            return default(ABBuildItem);
        }

        public void RemoveBundle(string bundleName)
        {
            ABBuildItem item = ContainsBundle(bundleName);
            if (item==null)
                Debug.Log("no Bundle");
            else
            {
                Builds.Remove(item);
                BuildsToDps();
            }
        }
        public void RemoveBundles(string[] bundleName)
        {
            for (int i = 0; i < bundleName.Length; i++)
            {
                ABBuildItem item = ContainsBundle(bundleName[i]);
                if (item == null)
                    Debug.Log("no Bundle");
                else
                    Builds.Remove(item);
            }
            BuildsToDps();
        }
        public void RemoveAsset(string bundleName, string AssetName)
        {
            ABBuildItem item = ContainsBundle(bundleName);
            if (item == null)
                Debug.Log("no Bundle");
            else
                for (int i = item.assetNames.Count - 1; i >= 0; i--)
                    if (item.assetNames[i] == AssetName)
                        item.assetNames.RemoveAt(i);
            if (item.assetNames.Count==0)
                Builds.Remove(item);
            BuildsToDps();
        }
        public void RemoveAssets(string bundleName, string[] AssetName)
        {
            ABBuildItem item = ContainsBundle(bundleName);
            if (item == null)
                Debug.Log("no Bundle");
            else
            {
                AssetName.ForEach((assetname) => {

                    for (int i = item.assetNames.Count - 1; i >= 0; i--)
                        if (item.assetNames[i] == assetname)
                            item.assetNames.RemoveAt(i);
                });
            }
            BuildsToDps();
        }

        public List<AssetBundleBuild> ToAssetBundleBuild()
        {
           return  Builds.ConvertAll<AssetBundleBuild>((item) => { return new AssetBundleBuild() {

                assetBundleName = item.assetBundleName,
                assetBundleVariant = item.assetBundleVariant,
                assetNames = item.assetNames.ToArray(),
            }; });
        }

        public void ReadAssetbundleBuild(List<AssetBundleBuild> list)
        {
            Builds.Clear();
            dps.Clear();
            list.ReverseForEach((item) =>
            {
                Builds.Add(new ABBuildItem()
                {
                    assetBundleName = item.assetBundleName,
                    assetBundleVariant = item.assetBundleVariant,
                    assetNames = item.assetNames.ToList(),
                    addressableNames = item.addressableNames != null? item.addressableNames.ToList():null
                });
            });
            BuildsToDps();
        }
        private void BuildsToDps()
        {
            dps.Clear();
            Builds.ReverseForEach((buildItem) => {
                buildItem.CrossRef = false;
                buildItem.FileLength = 0;
                for (int i = 0; i < buildItem.assetNames.Count; i++)
                {
                    string assetpath = buildItem.assetNames[i];
                    AssetDependenceInfo info = ContainsAsset(assetpath);
                    if (info==null)
                    {
                        info = new AssetDependenceInfo();
                        info.AssetPath = assetpath;
                        info.AssetName = assetpath.GetFileNameWithoutExtend();
                        info.ThumbNail = AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath<Object>(assetpath));
                        info.FileLength = IO.GetFileLength(assetpath);
                        info.Size = IO.GetFileSize(info.FileLength);

                        AssetImporter importer = AssetImporter.GetAtPath(assetpath);
                        info.BundleName = importer.assetBundleName + "." + importer.assetBundleVariant;

                        dps.Add(info);
                    }
                    else
                    {
                        buildItem.CrossRef = true;
                    }
                    info.AssetBundles.Add(buildItem.assetBundleName);
                    buildItem.FileLength += info.FileLength;
                }
                buildItem.Size = IO.GetFileSize(buildItem.FileLength);


            });

        }
    }
    [Serializable]
    internal class ABBuildItem
    {
        public string assetBundleName;
        public string assetBundleVariant;
        public List<string> assetNames = new List<string>();
        public List<string> addressableNames = new List<string>();

        public bool CrossRef;
        public long FileLength;
        public string Size;
    }
    [Serializable]
    internal class AssetDependenceInfo
    {
        public string AssetPath;
        public string AssetName;
        public Texture2D ThumbNail;
        public string BundleName;

        public string Size;
        public long FileLength;
        [SerializeField]
        public List<string> AssetBundles = new List<string>();
    }




    [Serializable]
    internal class ABCollectInfo
    {
        public List<ABCollectItem> Collects = new List<ABCollectItem>();
        public void AddCollecter(string path)
        {
            ABCollectItem collect = new ABCollectItem();
            collect.SearchPath = path;
            collect.SetSubAssets();
            Collects.Add(collect);
        }
    }
    [Serializable]
    internal class ABCollectItem
    {

        public ABCollectType CollectType;
        public string SearchPath;
        public string BundleName;
        public SubAssetInfo subAsset;

        public void SetSubAssets()
        {
            subAsset = new SubAssetInfo()
            {
                Selected = false,
                path = SearchPath,
                fileType = FileType.Folder,
                name = Path.GetFileName(SearchPath)
            };
            subAsset.SetSubAssets();
        }

        [NonSerialized] List<string> result = new List<string>();
        public List<string> GetSubAssets()
        {
            result.Clear();
            SetAssetPath(subAsset);
            return result;
        }
        private void SetAssetPath(SubAssetInfo subAsset)
        {
            if (subAsset.fileType != FileType.Folder && subAsset.Selected)
                result.Add(subAsset.path);
            for (int i = 0; i < subAsset.Sub.Count; i++)
            {
                SetAssetPath(subAsset.Sub[i]);
            }
        }
    }
    [Serializable]
    public class SubAssetInfo
    {
        public bool isOpen;
        private bool selscted;
        public bool Selected { get { return selscted; } set { SetSelected(value); } }
        public string path;
        public string name;
        public FileType fileType;

        [NonSerialized]
        private Texture2D thumbnail;
        public Texture2D ThumbNail
        {
            get
            {
                if (thumbnail == null)
                {
                    if (fileType == FileType.Folder)
                        thumbnail = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
                    thumbnail = AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath<Object>(path));
                }
                return thumbnail;
            }
        }

        public List<SubAssetInfo> Sub = new List<SubAssetInfo>();

        public void SetSubAssets()
        {
            if (fileType == FileType.Folder)
            {
                path.GetSubDirs().ForEach((dirName) =>
                {
                    SubAssetInfo asset = new SubAssetInfo()
                    {
                        Selected = false,
                        path = path.CombinePath(dirName),
                        name = dirName,
                        fileType = FileType.Folder,
                    };
                    asset.SetSubAssets();
                    Sub.Add(asset);
                });
                path.GetSubFiles(false).ForEach((fileName) =>
                {
                    if (fileName.Contains(".meta")) return;
                    bool valid = fileName.EndsWith(".cs");
                    SubAssetInfo asset = new SubAssetInfo()
                    {
                        Selected = false,
                        path = fileName.ToAssetsPath(),
                        name = fileName.GetFileNameWithoutExtend(),
                        fileType = valid ? FileType.InValidFile : FileType.ValidFile,
                    };
                    asset.SetSubAssets();
                    Sub.Add(asset);
                });
            }
        }


        public void SetSelected(bool sel)
        {
            if (this.selscted == sel) return;
            if (fileType == FileType.InValidFile) return;

            this.selscted = sel;
            for (int i = 0; i < Sub.Count; i++)
            {
                Sub[i].SetSelected(sel);
            }
        }
    }
    public enum FileType
    {
        ValidFile,
        Folder,
        InValidFile
    }






}
