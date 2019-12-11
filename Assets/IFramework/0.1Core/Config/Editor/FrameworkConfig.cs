/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
namespace IFramework
{
    class FrameworkConfig
    {
        private const string RelativeCorePath = "0.1Core";
        private const string RelativeCoreEditorPath = "0.1Core/Editor";
        private const string RelativeUtilPath = "UTil";
        private const string RelativeEditorPath = "UTil/Editor";
        private static string ConfigName = "FrameWorkInfo.asset";

        private static FrameworkConfigInfo info;
        private static FrameworkConfigInfo Info
        {
            get
            {
                if (info == null) SetConfig();
                return info;
            }
        }

        public static string FrameworkName { get { return Framework.FrameworkName; } }
        public static string Author { get { return Framework.Author; } }
        public static string Version { get { return Framework.Version; } }
        public static string Description { get { return Framework.Description; } }

        public static string EditorPath { get { return Info.EditorPath; } }
        public static string FrameworkPath { get { return Info.FrameWorkPath; } }
        public static string UtilPath { get { return Info.UtilPath; } }
        public static string CorePath { get { return Info.CorePath; } }
        public static string CoreEditorPath { get { return Info.CoreEditorPath; } }

        private static string ConfigPath;
        private static string GetRightConfigPath()
        {
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                if (assetPaths[i].Contains(RelativeCorePath))
                {
                    string tempPath = assetPaths[i];
                    int index = tempPath.IndexOf(RelativeCorePath);
                    string stoPath = tempPath.Remove(index)
                        .CombinePath(RelativeCorePath)
                        .CombinePath("Config/Resources").CombinePath(ConfigName).ToRegularPath();
                    return stoPath; 
                }
            }
            return string.Empty;
        }
        private static void SetConfig()
        {
            ConfigPath = GetRightConfigPath();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}",typeof(FrameworkConfigInfo)) , new string[] { @"Assets" });
            var stos=  guids.ToList() .ConvertAll((guid) => { return AssetDatabase.LoadAssetAtPath<FrameworkConfigInfo>(AssetDatabase.GUIDToAssetPath(guid)); });
            if (stos.Count == 0 || AssetDatabase.GetAssetPath(stos[0]).ToRegularPath()!= ConfigPath) CreateRightConfig(stos);
            else
            {
                for (int i = 1; i < stos.Count; i++) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(stos[i]));
                info = ScriptableObj.Load<FrameworkConfigInfo>(AssetDatabase.GetAssetPath(stos[0]).ToRegularPath());
                SetConfig(info);
            }
        }
        private static void CreateRightConfig(List<FrameworkConfigInfo> stos)
        {
            if (stos.Count!=0)
                stos.ReverseForEach((sto) => { AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sto)); });
            info = ScriptableObj.Create<FrameworkConfigInfo>(ConfigPath); 
            SetConfig(info);
        }
        private static void SetConfig(FrameworkConfigInfo config)
        {
            string FrameWorkPath= ConfigPath.Remove(ConfigPath.IndexOf(RelativeCorePath)).ToRegularPath(); 
            config.FrameWorkPath = FrameWorkPath;
            config.CorePath = FrameWorkPath.CombinePath(RelativeCorePath).ToRegularPath();
            config.CoreEditorPath = FrameWorkPath.CombinePath(RelativeCoreEditorPath).ToRegularPath();

            config.EditorPath = FrameWorkPath.CombinePath(RelativeEditorPath).ToRegularPath();
            config.UtilPath = FrameWorkPath.CombinePath(RelativeUtilPath).ToRegularPath();

            ScriptableObj.Update<FrameworkConfigInfo>(config);
        }

    }

}
