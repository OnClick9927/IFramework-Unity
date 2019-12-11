/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace IFramework.AB
{
    [EditorWindowCache("AssetBundle")]
    internal partial class ABWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            GetWindow<ABWindow>(false, "AssetBundle", true).minSize=new Vector2(200 + 700, 500);
        }
	}
    internal partial class ABWindow : EditorWindow
    {
        private static ABWindow instance;
        private ABWindow() { instance = this; }
        public static ABWindow Instance { get { return instance; } }

        private Rect windowRect;
        private Rect TopButtonRect;
        private Rect viewRect;

        private ABWindowTop Top;
        internal ABBuildWindow BuildWindow;
        internal ABCollectWindow CollectWindow;
        private string CollectInfoPath;

        public bool IsShowCollect=true;

        public ABInfo Info;

        private void OnEnable()
        {
            CollectInfoPath = FrameworkConfig.FrameworkPath.
                CombinePath("AssetBundle/Editor/Window/Sto/CollectInfo.asset");

            BuildWindow = new ABBuildWindow();
            Top =new ABWindowTop();
            CollectWindow = new ABCollectWindow();

            windowRect = this.position;
            TopButtonRect = new Rect(0, 0, windowRect.width, 50);
            viewRect = new Rect(0, 50, windowRect.width, windowRect.height - 50);
           
            LoadCollectInfo();


        }
        private void OnGUI()
        {
            windowRect = this.position;
            viewRect.width= TopButtonRect.width = windowRect.width;
            viewRect.height = windowRect.height - 50;
            Top.OnGUI(TopButtonRect);
            if (IsShowCollect)
                CollectWindow.OnGUI( viewRect);
            else
            {
                BuildWindow.OnGUI( viewRect);
            }
        }
        private void OnDisable()
        {
            ScriptableObj.Update<ABInfo>(Info);
        }


        private void LoadCollectInfo()
        {
            if (!File.Exists(CollectInfoPath))
                  ScriptableObj.Create<ABInfo>(CollectInfoPath);
            Info= ScriptableObj.Load<ABInfo>(CollectInfoPath);
           
        }


        public List<ABBuildCollecter> LoadCollecter()
        {
            var collecters= Info.CollectInfo.Collects.ConvertAll<ABBuildCollecter>((item) =>
            {
                if (string.IsNullOrEmpty(item.SearchPath)) return null;
                switch (item.CollectType)
                {
                    case ABCollectType.ABName:
                        if (!string.IsNullOrEmpty(item.BundleName))
                        {
                            return new CollectByABName()
                            {
                                bundleName = item.BundleName,
                                searchPath = item.SearchPath,
                                MeetFiles = item.GetSubAssets()
                            };
                        }
                        return null;
                    case ABCollectType.DirName:
                        return new CollectByDirName()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssets()
                        };
                    case ABCollectType.FileName:
                        return new CollectByFileName()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssets()
                        };
                    case ABCollectType.Scene:
                        return new CollectByScenes()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssets()
                        };
                    default:
                        return null;
                }

            });
            collecters= collecters.ReverseForEach((col) =>
            {
                if (col == null)
                    collecters.Remove(col);
            });
            return collecters;
        }

        public void ReFreashValue()
        {
            BuildWindow.ChoosedAsset = null;
            BuildWindow.ChossedAssetBundle = null;
            Info.TempInfo.ReadAssetbundleBuild(ABBuildCollect.Collect(ABTool.ManifestPath));
            ScriptableObj.Update(ABWindow.Instance.Info);
        }

        public void DeleteBundle(string bundleName)
        {
            Info.TempInfo.RemoveBundle(bundleName);
        }
        public void RemoveAsset(string assetPath, string bundleName)
        {
            Info.TempInfo.RemoveAsset(bundleName, assetPath);
    
        }


       
    }
}
