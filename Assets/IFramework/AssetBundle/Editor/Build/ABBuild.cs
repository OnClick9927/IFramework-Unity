/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using System.Collections.Generic; 
using System.IO;
using UnityEditor;

namespace IFramework.AB
{
    class ABBuild
    {

        public static void BuildManifest(string path, List<AssetBundleBuild> builds)
        {
            if (File.Exists(path)) File.Delete(path);
            List<ManifestXmlContent> contents = new List<ManifestXmlContent>();
            foreach (var item in builds)
            {
                contents.Add(new ManifestXmlContent(item.assetBundleName, item.assetNames));
            }
            string txt = Xml.ToXmlString<List<ManifestXmlContent>>(contents);
            File.WriteAllText(path, txt);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        public static void BuildAssetBundles(List<AssetBundleBuild> builds, BuildTarget buildTarget)
        {
            if (!Directory.Exists(ABTool.AssetBundleOutPutPath))
                Directory.CreateDirectory(ABTool.AssetBundleOutPutPath);
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#if UNITY_TVOS
			shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
#endif
            if (shouldCheckODR)
            {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
				if (PlayerSettings.iOS.useOnDemandResources)
				options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
#if ENABLE_IOS_APP_SLICING
				options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
            }

            if (builds == null || builds.Count == 0)
                BuildPipeline.BuildAssetBundles(ABTool.AssetBundleOutPutPath, options, buildTarget);
            else
            {
                BuildPipeline.BuildAssetBundles(ABTool.AssetBundleOutPutPath, builds.ToArray(), options, buildTarget);
            }
        }

        public static string GetAssetBundleManifestFilePath()
        {
            var relativeAssetBundlesOutputPathForPlatform = Path.Combine(ABTool.AssetBundleOutPutPath, ABTool.CurrentPlatformName);
            return Path.Combine(relativeAssetBundlesOutputPathForPlatform, ABTool.CurrentPlatformName) + ".manifest";
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            string name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            if (target == BuildTarget.Android)
            {
                return "/" + name + PlayerSettings.Android.bundleVersionCode + ".apk";
            }
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return "/" + name + PlayerSettings.Android.bundleVersionCode + ".exe";
            }
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
#endif
            {
                return "/" + name + ".app";
            }
            if (target == BuildTarget.iOS)
            {
                return "/iOS";
            }
            return null;


        }

        public static void CopyAssetBundlesTo(string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            var destination = Path.Combine(outputPath, ABTool.AssetBundlesOutputDirName);
            if (Directory.Exists(destination))
                FileUtil.DeleteFileOrDirectory(outputPath);

            FileUtil.CopyFileOrDirectory(ABTool.AssetBundlesOutputDirName, destination);
            AssetDatabase.Refresh();
        }
        public static void DeleteBundleFile()
        {
            Log.L(ABTool.AssetBundleOutPutPath);
            Directory.Delete(ABTool.AssetBundleOutPutPath, true);
        }
    }
}
