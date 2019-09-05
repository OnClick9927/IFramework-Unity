/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
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
    public class FrameworkConfig
	{
        private static FrameworkConfigInfo info;
        private static FrameworkConfigInfo Info
        {
            get
            {
                if (info == null) SetConfig();
                return info; 
            }
        }
        public static string EditorPath { get { return Info.EditorPath; } }
        public static string FrameworkPath { get { return Info.FrameWorkPath; } }
        public static string UtilPath { get { return Info.UtilPath; } }
        public static string UnityCorePath { get { return Info.UnityCorePath; } }
        public static string EditorCorePath { get { return Info.EditorCorePath; } }

        public static string Version { get { return Info.Version; } }
        public static string Description { get { return Info.Description; } }


        private static string ConfigPath;
        private static string GetRightConfigPath()
        {
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                if (assetPaths[i].Contains(RelativeUnityCorePath))
                {
                    string tempPath = assetPaths[i];
                    int index = tempPath.IndexOf(RelativeUnityCorePath);
                    string stoPath = tempPath.Remove(index)
                        .CombinePath(RelativeUnityCorePath)
                        .CombinePath("Config/Resources").CombinePath(ConfigName).ToRegularPath();
                    return stoPath; 
                }
            }
            return string.Empty;
        }
        private static void SetConfig()
        {
            ConfigPath = GetRightConfigPath();
            string[] guids = AssetDatabase.FindAssets("t:FrameworkConfigInfo", new string[] { @"Assets" });
            List<FrameworkConfigInfo> stos=  guids.ToList()
                .ConvertAll((guid) => { return AssetDatabase.LoadAssetAtPath<FrameworkConfigInfo>(AssetDatabase.GUIDToAssetPath(guid)); });
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
            string FrameWorkPath= ConfigPath.Remove(ConfigPath.IndexOf(RelativeUnityCorePath)).ToRegularPath(); 
            config.FrameWorkPath = FrameWorkPath;
            config.UnityCorePath = FrameWorkPath.CombinePath(RelativeUnityCorePath).ToRegularPath();
            config.EditorCorePath = FrameWorkPath.CombinePath(RelativeEditorCorePath).ToRegularPath();

            config.EditorPath = FrameWorkPath.CombinePath(RelativeEditorPath).ToRegularPath();
            config.UtilPath = FrameWorkPath.CombinePath(RelativeUtilPath).ToRegularPath();
            config.FrameWorkName = FrameworkName;
            config.Author = Author;
            ScriptableObj.Update<FrameworkConfigInfo>(config);
        }
        private const string RelativeUnityCorePath = "0.1Core/Unity";
        private const string RelativeEditorCorePath = "0.1Core/Unity/Editor";
        private const string RelativeUtilPath = "UTil";
        private const string RelativeEditorPath = "UTil/Editor";

        private static string ConfigName = "FrameWorkInfo.asset";
        public const string FrameworkName = "IFramework";
        public const string Author = "OnClick"; 

    }

}
