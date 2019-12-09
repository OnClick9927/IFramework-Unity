/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.AB;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
	public class AutoBuildPlayer
	{
        [MenuItem("IFramework/AutoBuild/Transfer Android")]
        public static void AndroidPlatformBuild()
        {
            //SwitchPlatform
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
#if UNITY_2018
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            PlayerSettings.fullScreenMode =   FullScreenMode.FullScreenWindow;
#else
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
            //Resolution and Presentation
            PlayerSettings.defaultIsFullScreen = false;
#endif
            PlayerSettings.defaultScreenWidth = 1920; 
            PlayerSettings.defaultWebScreenHeight = 1080; 
            PlayerSettings.runInBackground = true;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            PlayerSettings.visibleInBackground = true;
            PlayerSettings.allowFullscreenSwitch = true;

            //SplashScreen

            //OtherSettings
            PlayerSettings.MTRendering = true;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
        }
        [MenuItem("IFramework/AutoBuild/Transfer StandaloneWindows")]
        private static void StandaloneWindowsPlatformBuild()
        {
            //SwitchPlatform
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

            //Resolution and Presentation
#if UNITY_2018
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
#else
            PlayerSettings.defaultIsFullScreen = false;
#endif
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultWebScreenHeight = 1080;
            PlayerSettings.runInBackground = true;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            PlayerSettings.visibleInBackground = true;
            PlayerSettings.allowFullscreenSwitch = true;

            //SplashScreen

            //OtherSettings
            PlayerSettings.MTRendering = true;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            QualitySettings.vSyncCount = 0;
            //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "DESKTOP");

        }

        [MenuItem("IFramework/AutoBuild/Append ScriptingDefineSymbols EnableLog")]
        private static void AppendScriptingDefineSymbols()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "EnableLog");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "EnableLog");
        }

        [MenuItem("IFramework/AutoBuild/Reduce ScriptingDefineSymbols EnableLog")]
        private static void ReduceScriptingDefineSymbols()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");
        }
        [MenuItem("IFramework/AutoBuild/Build Player With AssetBundle")]
        public static void BuildPlayerWithAB()
        {
            if (EditorApplication.isCompiling) return;
            BuildStandalonePlayer();
        }
        [MenuItem("IFramework/AutoBuild/Build Player Without AssetBundle")]
        public static void BuildPlayerWithOutAB()
        {
            if (EditorApplication.isCompiling) return;
            BuildPlayerWithoutAssetBundles();
        }
        public static void BuildStandalonePlayer()
        {
            var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0) return;
            string[] levels = EditorUtil.GetScenesInBuildSetting();
            if (levels.Length == 0) return;
            string targetName = "/" + EditorUtil.GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null) return;
            ABBuild.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, ABTool.AssetBundleOutPutPath));
            AssetDatabase.Refresh();
#if UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
#else
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = levels;
            buildPlayerOptions.locationPathName = outputPath + targetName;
            buildPlayerOptions.assetBundleManifestPath = ABBuild.GetAssetBundleManifestFilePath();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
            buildPlayerOptions.options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
#endif
        }
        public static void BuildPlayerWithoutAssetBundles()
        {
            var outputPath = UnityEditor.EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0) return;
            string[] levels = EditorUtil. GetScenesInBuildSetting();
            if (levels.Length == 0) return;
            string targetName = "/" + EditorUtil.GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null) return;
#if UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
#else
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = levels;
            buildPlayerOptions.locationPathName = outputPath + targetName;
            buildPlayerOptions.assetBundleManifestPath = ABBuild.GetAssetBundleManifestFilePath();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
            buildPlayerOptions.options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
#endif
        }
    }
}
