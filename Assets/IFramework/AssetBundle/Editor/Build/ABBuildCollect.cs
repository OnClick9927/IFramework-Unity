/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IFramework.AB
{
    class ABBuildCollect
	{
        public static event Func<List<ABBuildCollecter>> onLoadBuilders;
        private static Dictionary<string, List<string>> dps = new Dictionary<string, List<string>>();
        private static List<ABBuildCollecter> collecters = new List<ABBuildCollecter>();

        public static List<string> collectedAssets = new List<string>();
        public static List<AssetBundleBuild> Builds = new List<AssetBundleBuild>();

        public static List<AssetBundleBuild> Collect(string manifestPath)
        {
            if (!File.Exists(manifestPath)) File.Create(manifestPath);
            collectedAssets.Clear();
            Builds.Clear();
            collecters.Clear();
            dps.Clear();

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = ABTool.MainAssetBundleBuildName;
            build.assetNames = new string[] { manifestPath };
            Builds.Add(build);

            LoadBuilders();
            CollectDependences();
            for (int i = 0; i < collecters.Count; i++)
                collecters[i].Collect();
            BuildAtlas();
            EditorUtility.ClearProgressBar();
            return Builds;
        }
        private static void LoadBuilders()
        {
            collecters.AddRange(onLoadBuilders());
        }
        private static void CollectDependences()
        {
            for (int i = 0; i < collecters.Count; i++)
                CollectDependence(collecters[i].MeetFiles);
            Dictionary<string, List<string>> bundles = new Dictionary<string, List<string>>();
            foreach (var item in dps)
            {
                var assetPath = item.Key;
                if (!assetPath.EndsWith(".cs", StringComparison.CurrentCulture))
                {
                    if (collectedAssets.Contains(assetPath)) continue;
                    if (assetPath.EndsWith(".shader", StringComparison.CurrentCulture))
                    {
                        List<string> list = null;
                        if (!bundles.TryGetValue("shaders", out list))
                        {
                            list = new List<string>();
                            bundles.Add("shaders", list);
                        }
                        if (!list.Contains(assetPath))
                        {
                            list.Add(assetPath);
                            collectedAssets.Add(assetPath);
                        }
                    }
                    else
                    {
                        if (item.Value.Count > 1)
                        {
                            var name = "shared/" + GetAbsPathWithoutExtension(Path.GetDirectoryName(assetPath));
                            List<string> list = null;
                            if (!bundles.TryGetValue(name, out list))
                            {
                                list = new List<string>();
                                bundles.Add(name, list);
                            }
                            if (!list.Contains(assetPath))
                            {
                                list.Add(assetPath);
                                collectedAssets.Add(assetPath);
                            }
                        }
                    }
                }
            }
            foreach (var item in bundles)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = item.Key + "_" + item.Value.Count;
                build.assetNames = item.Value.ToArray();
                Builds.Add(build);
            }
        }
        private static void CollectDependence(List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++) 
            {
                string path = paths[i];
                string[] dependencies = AssetDatabase.GetDependencies(path);
                //string str = string.Format("Collecting... [{0}/{1}]", i == 0 ? 0 : i, paths.Count);
                //if (string.IsNullOrEmpty(str)|| EditorUtility.DisplayCancelableProgressBar(str, path, i==0?0:(i* 1f / paths.Count))) break;
                 
                foreach (string dpPath in dependencies)
                {
                    if (!dps.ContainsKey(dpPath))
                        dps[dpPath] = new List<string>();
                    if (!dps[dpPath].Contains(path))
                        dps[dpPath].Add(path);
                }
            }
        }


        private static void BuildAtlas()
        {
            foreach (var item in Builds)
            {
                var assets = item.assetNames;
                foreach (var asset in assets)
                {
                    var importer = AssetImporter.GetAtPath(asset);
                    if (importer is TextureImporter)
                    {
                        var ti = importer as TextureImporter;
                        if (ti.textureType == TextureImporterType.Sprite)
                        {
                            var tex = AssetDatabase.LoadAssetAtPath<Texture>(asset);
                            if (tex.texelSize.x >= 1024 || tex.texelSize.y >= 1024)
                            {
                                continue;
                            }

                            var tag = item.assetBundleName.Replace("/", "_");
                            if (!tag.Equals(ti.spritePackingTag))
                            {
                                var settings = ti.GetPlatformTextureSettings(ABTool.CurrentPlatformName);
                                settings.format = ti.GetAutomaticFormat(ABTool.CurrentPlatformName);
                                settings.overridden = true;
                                ti.SetPlatformTextureSettings(settings);
                                ti.spritePackingTag = tag;
                                ti.SaveAndReimport();
                            }
                        }
                    }
                }

            }
        }





        public static string GetAbsPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace('\\', '/').ToLower();
        }
        public static List<string> GetNonCollect(ABBuildCollecter collecter)
        {
            List<string> files = collecter.MeetFiles;
            int filesCount = files.Count;
            int removed = files.RemoveAll((string obj) =>
            {
                return collectedAssets.Contains(obj);
            });
            Debug.Log(string.Format("RemoveAll {0} size: {1}", removed, filesCount));
            return files;
        }
        public static List<string> GetDependenciesWithoutShared(string filePath)
        {
            string[] dpPaths = AssetDatabase.GetDependencies(filePath);
            List<string> assetNames = new List<string>();
            foreach (var assetPath in dpPaths)
            {
                if (assetPath.Contains(".prefab") || assetPath.Equals(filePath) ||
                    collectedAssets.Contains(assetPath) ||
                    assetPath.EndsWith(".cs", StringComparison.CurrentCulture) ||
                    assetPath.EndsWith(".shader", StringComparison.CurrentCulture))
                    continue;
                if (dps[assetPath].Count == 1)
                {
                    assetNames.Add(assetPath);
                }
            }
            return assetNames;
        }
    }
}
