/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using IFramework.Utility;

namespace IFramework
{
    public class EditorUtil
	{
        [MenuItem("IFramework/Tool/Copy Asset Path")]
        public static void CopyAssetPath()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            GUIUtility.systemCopyBuffer = path;
        }
        [MenuItem("IFramework/Editor/Quit2")]
        public static void Quit2()
        {
          //  Environment.Exit(0);
            EditorApplication.Exit(0);
        }
        [MenuItem("IFramework/Editor/Quit")]
        public static void Quit()
        {
            Process.GetCurrentProcess().Kill();
        }
        [MenuItem("IFramework/Editor/ReOpen")]
        public static void ReOpen()
        {
            string unityPath = Process.GetCurrentProcess().MainModule.FileName;
            string assetPath = Application.dataPath.GetDirPath();
            string batPath = FrameworkConfig.CoreEditorPath.CombinePath(@"ReStart.bat").ToAbsPath();
            int processId = Process.GetCurrentProcess().Id;
            CreatReStartBat(batPath);
            ProcessUtil.CreateProcess(batPath, string.Format("\"{0}\" \"{1}\" ", unityPath, assetPath, unityPath.Length));
            Process.GetCurrentProcess().Kill(); 
        }
        [MenuItem("IFramework/Editor/ReOpen2")]
        public static void ReOpen2()
        {
            EditorApplication.OpenProject(Application.dataPath.CombinePath("../"));
        }
        private static  void CreatReStartBat(string batPath)
        {
            if (File.Exists(batPath)) return;
            using (FileStream fs = new FileStream(batPath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.Lock(0, fs.Length);
                    sw.WriteLine("@echo off");

                    sw.WriteLine("set unityPath=%~1%");
                    sw.WriteLine("set projectPath=%~2%");
                    sw.WriteLine("for /f \"delims = \"\" tokens = 1\" %%v in ( %unityPath% ) do ( set unityPath=%%v )");
                    sw.WriteLine("for /f \"delims = \"\" tokens = 1\" %%v in ( %projectPath% ) do ( set projectPath=%%v )");
                    sw.WriteLine("start %unityPath% -projectPath %projectPath%");
                    //sw.WriteLine("pause");
                    fs.Unlock(0, fs.Length);
                    sw.Flush();
                    fs.Flush();

                    sw.Close();
                    fs.Close();
                }
            }
            AssetDatabase.Refresh();
        }

        public static EditorBuildSettingsScene[] ScenesInBuildSetting()
        {
            return EditorBuildSettings.scenes;
        }
        public static string[] GetScenesInBuildSetting()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            string name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            if (target == BuildTarget.Android)
            {
                return  name + PlayerSettings.Android.bundleVersionCode + ".apk";
            }
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return  name + PlayerSettings.Android.bundleVersionCode + ".exe";
            }
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
#endif
            {
                return  name + ".app";
            }
            if (target == BuildTarget.iOS)
            {
                return "iOS";
            }
            return null;
            //if (target == BuildTarget.WebGL)
            //{
            //    return "/web";
            //}

        }
        public static GameObject CreatePrefab(GameObject source, string savePath)
        {
            GameObject goClone =GameObject.Instantiate(source);
            GameObject prefab;
#if UNITY_2018_1_OR_NEWER
            prefab= PrefabUtility.SaveAsPrefabAssetAndConnect(goClone, savePath, InteractionMode.AutomatedAction);
          
#else
            prefab = PrefabUtility.CreatePrefab(savePath, goClone);
            prefab = PrefabUtility.ReplacePrefab(goClone, prefab, ReplacePrefabOptions.ConnectToPrefab);
#endif
            AssetDatabase.ImportAsset(savePath);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            GameObject.DestroyImmediate(goClone);
            return prefab;
        }

    }

}
