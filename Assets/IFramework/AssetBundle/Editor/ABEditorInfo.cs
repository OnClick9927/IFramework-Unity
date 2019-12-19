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
    public class ABEditorInfo
	{
        public ABDirCollect DirCollect=new ABDirCollect();
        public ABBuiidInfo ABBuiidInfo = new ABBuiidInfo();
    }
    [Serializable]
    public class ABBuiidInfo
    {
        [SerializeField]
        public List<ABDeprndence> Dependences = new List<ABDeprndence>();
        public List<AssetBundleBuild_Class> AssetbundleBuilds = new List<AssetBundleBuild_Class>();

        private ABDeprndence ContainsAsset(string assetpath)
        {
            for (int i = 0; i < Dependences.Count; i++)
            {
                if (Dependences[i].AssetPath==assetpath)
                {
                    return Dependences[i];
                }
            }
            return default(ABDeprndence);
        }
        private AssetBundleBuild_Class ContainsBundle(string bundleName)
        {
            for (int i = 0; i < AssetbundleBuilds.Count; i++)
            {
                if (AssetbundleBuilds[i].assetBundleName == bundleName)
                {
                    return AssetbundleBuilds[i];
                }
            }
            return default(AssetBundleBuild_Class);
        }

        public void RemoveBundle(string bundleName)
        {
            AssetBundleBuild_Class item = ContainsBundle(bundleName);
            if (item==null)
                Debug.Log("no Bundle");
            else
            {
                AssetbundleBuilds.Remove(item);
                BuildsToDps();
            }
        }
        public void RemoveBundles(string[] bundleName)
        {
            for (int i = 0; i < bundleName.Length; i++)
            {
                AssetBundleBuild_Class item = ContainsBundle(bundleName[i]);
                if (item == null)
                    Debug.Log("no Bundle");
                else
                    AssetbundleBuilds.Remove(item);
            }
            BuildsToDps();
        }
        public void RemoveAsset(string bundleName, string AssetName)
        {
            AssetBundleBuild_Class item = ContainsBundle(bundleName);
            if (item == null)
                Debug.Log("no Bundle");
            else
                for (int i = item.assetNames.Count - 1; i >= 0; i--)
                    if (item.assetNames[i] == AssetName)
                        item.assetNames.RemoveAt(i);
            if (item.assetNames.Count==0)
                AssetbundleBuilds.Remove(item);
            BuildsToDps();
        }
        public void RemoveAssets(string bundleName, string[] AssetName)
        {
            AssetBundleBuild_Class item = ContainsBundle(bundleName);
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

        public List<AssetBundleBuild> GetAssetBundleBuilds()
        {
           return  AssetbundleBuilds.ConvertAll<AssetBundleBuild>((item) => { return item; });
        }
        public void ReadAssetbundleBuild(List<AssetBundleBuild> list)
        {
            AssetbundleBuilds.Clear();
            Dependences.Clear();
            list.ReverseForEach((item) =>
            {
                AssetbundleBuilds.Add(item);
            });
            BuildsToDps();
        }

        private void BuildsToDps()
        {
            Dependences.Clear();
            AssetbundleBuilds.ReverseForEach((buildItem) => {
                buildItem.CrossRefence = false;
                buildItem.FileLength = 0;
                for (int i = 0; i < buildItem.assetNames.Count; i++)
                {
                    string assetpath = buildItem.assetNames[i];
                    ABDeprndence info = ContainsAsset(assetpath);
                    if (info==null)
                    {
                        info = new ABDeprndence();
                        info.AssetPath = assetpath;
                        info.FileLength = IO.GetFileLength(assetpath);
                        info.Size = IO.GetFileSize(info.FileLength);

                        AssetImporter importer = AssetImporter.GetAtPath(assetpath);
                        info.BundleName = importer.assetBundleName + "." + importer.assetBundleVariant;

                        Dependences.Add(info);
                    }
                    else
                    {
                        buildItem.CrossRefence = true;
                    }
                    info.AssetBundles.Add(buildItem.assetBundleName);
                    buildItem.FileLength += info.FileLength;
                }
                buildItem.Size = IO.GetFileSize(buildItem.FileLength);


            });

        }
    }
    [Serializable]
    public class AssetBundleBuild_Class
    {
        public string assetBundleName;
        public string assetBundleVariant;
        public List<string> assetNames = new List<string>();
        public List<string> addressableNames = new List<string>();

        public bool CrossRefence;
        public long FileLength;
        public string Size;
        public static implicit operator AssetBundleBuild(AssetBundleBuild_Class _class)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = _class.assetBundleName;
            build.assetBundleVariant = _class.assetBundleVariant;
            build.assetNames = _class.assetNames.ToArray();
            return build;
        }
        public static implicit operator AssetBundleBuild_Class(AssetBundleBuild _struct)
        {
            return new AssetBundleBuild_Class()
            {
                assetBundleName = _struct.assetBundleName,
                assetBundleVariant = _struct.assetBundleVariant,
                assetNames = _struct.assetNames.ToList(),
                addressableNames = _struct.addressableNames != null ? _struct.addressableNames.ToList() : null
            };
        }
    }
    [Serializable]
    public class ABDeprndence
    {
        public string AssetPath;
        public string AssetName { get { return AssetPath.GetFileNameWithoutExtend(); } }
        public Texture2D ThumbNail { get { return AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath<Object>(AssetPath)); } }
        public string BundleName;

        public string Size;
        public long FileLength;
        [SerializeField]
        public List<string> AssetBundles = new List<string>();
    }




    [Serializable]
    public class ABDirCollect
    {
        public List<ABDirCollectItem> DirCollectItems = new List<ABDirCollectItem>();
        public void AddCollectItem(string path)
        {
            for (int i = 0; i < DirCollectItems.Count; i++)
            {
                if (path.Contains(DirCollectItems[i].SearchPath))
                    return;
            }
            ABDirCollectItem collect = new ABDirCollectItem(path);
            DirCollectItems.Add(collect);
        }
        public void RemoveCollectItem(ABDirCollectItem item)
        {
            if (DirCollectItems.Contains(item))
            {
                DirCollectItems.Remove(item);
            }
        }
    }
    [Serializable]
    public class ABDirCollectItem
    {
        [Serializable]
        public class ABSubFile
        {
            public enum FileType
            {
                ValidFile,
                Folder,
                InValidFile
            }

            public bool isOpen;

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
            [SerializeField]
            private bool selscted;
            public bool Selected { get { return selscted; } set { SetSelected(value); } }
            private void SetSelected(bool sel)
            {
                if (this.selscted == sel) return;
                if (fileType == FileType.InValidFile) return;

                this.selscted = sel;
                for (int i = 0; i < SubFiles.Count; i++)
                {
                    SubFiles[i].SetSelected(sel);
                }
            }


            public List<ABSubFile> SubFiles = new List<ABSubFile>();
            public ABSubFile() { }
            public ABSubFile(bool selected,string path,string name,FileType fileType)
            {
                this.selscted = selected;
                this.path = path;
                this.name = name;
                this.fileType = fileType;
            }
            public void SetSubAssetPaths()
            {
                if (fileType == FileType.Folder)
                {
                    path.GetSubDirs().ForEach((dirName) =>
                    {
                        ABSubFile asset = new ABSubFile(false,
                            path.CombinePath(dirName),
                            dirName,
                            FileType.Folder);
                        asset.SetSubAssetPaths();
                        SubFiles.Add(asset);
                    });
                    path.GetSubFiles(false).ForEach((fileName) =>
                    {
                        if (fileName.Contains(".meta")) return;
                        bool valid = fileName.EndsWith(".cs");
                        ABSubFile asset = new ABSubFile(false, 
                            fileName.ToAssetsPath(),
                            fileName.GetFileNameWithoutExtend(), 
                            valid ? FileType.InValidFile : FileType.ValidFile);
                        asset.SetSubAssetPaths();
                        SubFiles.Add(asset);
                    });
                }
            }
        }
        public enum ABDirCollectType
        {
            ABName,
            DirName,
            FileName,
            Scene
        }
        public ABDirCollectType CollectType;
        public string SearchPath;
        public string BundleName;
        public ABSubFile subAsset;
        public ABDirCollectItem() { }
        public ABDirCollectItem(string path)
        {
            this.SearchPath = path;
            SetSubAssetPaths();
        }
        private void SetSubAssetPaths()
        {
            subAsset = new ABSubFile(false, SearchPath, Path.GetFileName(SearchPath), ABSubFile.FileType.Folder);
            subAsset.SetSubAssetPaths();
        }

        public List<string> GetSubAssetPaths()
        {
            List<string> result = new List<string>();
            GetSubAssetPaths(subAsset, result);
            return result;
        }
        private void GetSubAssetPaths(ABSubFile subAsset,List<string> result)
        {
            if (subAsset.fileType != ABSubFile.FileType.Folder && subAsset.Selected)
                result.Add(subAsset.path);
            for (int i = 0; i < subAsset.SubFiles.Count; i++)
                GetSubAssetPaths(subAsset.SubFiles[i], result);
        }
    }
}
