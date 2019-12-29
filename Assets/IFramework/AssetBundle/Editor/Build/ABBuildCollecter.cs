/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace IFramework.AB
{
    abstract class ABBuildCollecter
    {
        public string searchPath;
        public string bundleName;
        public ABBuildCollecter() { }
        public ABBuildCollecter(string path)
        {
            searchPath = path;
        }
        public abstract void Collect();
        public abstract string GetAssetBundleName(string assetPath);
        public List<string> MeetFiles = new List<string>();
    }

    class CollectByABName : ABBuildCollecter
    {
        public CollectByABName() { }
        public CollectByABName(string path,string assetBundleName) : base(path)
        {
            bundleName = assetBundleName;
        }
        public override string GetAssetBundleName(string assetPath)
        {
            return bundleName;
        }
        public override void Collect()
        {
            var files = ABBuildCollect.GetNonCollect(this);
            List<string> list = new List<string>();
            foreach (var item in files)
            {
                list.AddRange(ABBuildCollect.GetDependenciesWithoutShared(item));
            }
            files.AddRange(list);
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = files.ToArray();
            ABBuildCollect.Builds.Add(build);
            ABBuildCollect.collectedAssets.AddRange(files);
        }
    }
    class CollectByScenes : ABBuildCollecter
    {
        public CollectByScenes() { }
        public CollectByScenes(string path) : base(path) { }

        public override string GetAssetBundleName(string assetPath)
        {
            return ABBuildCollect.GetAbsPathWithoutExtension(assetPath);
        }

        public override void Collect()
        {
            var files = ABBuildCollect.GetNonCollect(this);
            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ABBuildCollect.GetAbsPathWithoutExtension(item);
                build.assetNames = new string[] { item };
                ABBuildCollect.collectedAssets.AddRange(build.assetNames);
                ABBuildCollect.Builds.Add(build);
            }
        }
    }
    class CollectByDirName : ABBuildCollecter
    {
        public CollectByDirName() { }
        public CollectByDirName(string path) : base(path) { }
        public override string GetAssetBundleName(string assetPath)
        {
            return ABBuildCollect.GetAbsPathWithoutExtension(Path.GetDirectoryName(assetPath));
        }
        public override void Collect()
        {
            List<string> files = ABBuildCollect.GetNonCollect(this);
            Dictionary<string, List<string>> bundles = new Dictionary<string, List<string>>();
            for (int i = 0; i < files.Count; i++)
            {
                var filePath = files[i];
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Collecting... [{0}/{1}]", i, files.Count), filePath, i * 1f / files.Count)) break;
                var path = Path.GetDirectoryName(filePath);
                if (!bundles.ContainsKey(path))
                {
                    bundles[path] = new List<string>();
                }
                bundles[path].Add(filePath);
                bundles[path].AddRange(ABBuildCollect.GetDependenciesWithoutShared(filePath));
            }

            int count = 0;
            foreach (var item in bundles)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ABBuildCollect.GetAbsPathWithoutExtension(item.Key) + "_" + item.Value.Count;
                build.assetNames = item.Value.ToArray();
                ABBuildCollect.collectedAssets.AddRange(build.assetNames);
                ABBuildCollect.Builds.Add(build);
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Packing... [{0}/{1}]", count, bundles.Count), build.assetBundleName, count * 1f / bundles.Count)) break;
                count++;
            }
        }
    }
    class CollectByFileName : ABBuildCollecter
    {
        public CollectByFileName() { }
        public CollectByFileName(string path) : base(path) { }

        public override string GetAssetBundleName(string assetPath)
        {
            return ABBuildCollect.GetAbsPathWithoutExtension(assetPath);
        }
        public override void Collect()
        {
            var files = ABBuildCollect.GetNonCollect(this);
            for (int i = 0; i < files.Count; i++)
            {
                string filePath = files[i];
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Packing... [{0}/{1}]", i, files.Count), filePath, i * 1f / files.Count)) break;
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ABBuildCollect.GetAbsPathWithoutExtension(filePath);
                List<string> assetNames = ABBuildCollect.GetDependenciesWithoutShared(filePath);
                assetNames.Add(filePath);
                build.assetNames = assetNames.ToArray();
                ABBuildCollect.collectedAssets.AddRange(assetNames);
                ABBuildCollect.Builds.Add(build);
            }
        }
    }

}
