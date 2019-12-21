/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using UnityEngine;

namespace IFramework.AB
{
	public class ABTool
	{
        public const string ManifestPath = "Assets/Manifest.Xml";
        public const string AssetBundlesOutputDirName = "AssetBundles";
        public const string MainAssetBundleBuildName = "help";
        public static string AssetBundleOutPutPath = AssetBundlesOutputDirName.CombinePath(CurrentPlatformName);
        public static string CurrentPlatformName
        {
            get
            {
#if UNITY_EDITOR
                return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
            }
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            if (platform == RuntimePlatform.Android) return "Android";
            if (platform == RuntimePlatform.IPhonePlayer) return "iOS";
            if (platform == RuntimePlatform.tvOS) return "tvOS";
            if (platform == RuntimePlatform.WebGLPlayer) return "WebGL";
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor) return "Windows";
            if (platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.OSXEditor) return "OSX";
            return null;
        }
#if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            if (target == BuildTarget.Android) return "Android";
            if (target == BuildTarget.tvOS) return "tvOS";
            if (target == BuildTarget.iOS) return "iOS";
            if (target == BuildTarget.WebGL) return "WebGL";
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64) return "Windows";
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target ==  BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
#endif
                return "OSX";
            return null;
        }
        private static int activeBundleMode = -1;
        public static bool ActiveBundleMode
        {
            get
            {
                if (activeBundleMode == -1)
                    activeBundleMode = EditorPrefs.GetBool("ActiveBundleMode", true) ? 1 : 0;
                return activeBundleMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != activeBundleMode)
                {
                    activeBundleMode = newValue;
                    EditorPrefs.SetBool("ActiveBundleMode", value);
                }
            }
        }
#endif
    }
}
