/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [InitializeOnLoad]
    public partial class EditorTools
    {

        static EditorTools()
        {
            var directorys = new List<string>()
            {
                "Assets/Editor",
                EditorTools.projectMemoryPath,
            };
            CreateDirectories(directorys);

            AssetDatabase.Refresh();




            Log.logger = new UnityLogger();
            SetLogStatus();
        }
        public static void SetLogStatus()
        {
            Log.enable_F = ProjectConfig.enable_F;

            Log.enable_L = ProjectConfig.enable_L;
            Log.enable_W = ProjectConfig.enable_W;
            Log.enable_E = ProjectConfig.enable_E;
            Log.enable_A = ProjectConfig.enable_A;

            Log.enable = ProjectConfig.enable;
        }

        public const string projectMemoryPath = "Assets/Editor/IFramework";

        private static string GetFilePath() => AssetDatabase.GetAllAssetPaths().FirstOrDefault(x => x.Contains(nameof(IFramework))
                                                        && x.EndsWith($"{nameof(EditorTools)}.cs"));
        public static string pkgPath
        {
            get
            {
                string packagePath = Path.GetFullPath("Packages/com.woo.iframework");
                if (Directory.Exists(packagePath))
                {
                    return packagePath;
                }

                string path = GetFilePath();
                var index = path.LastIndexOf("IFramework");
                path = path.Substring(0, index + "IFramework".Length);
                return path;
            }
        }


        public static void SaveToPrefs<T>(T value, string key, bool unique = true) => Prefs.SetObject(value.GetType(), key, value, unique);
        public static T GetFromPrefs<T>(string key, bool unique = true) => Prefs.GetObject<T>(key, unique);
        public static object GetFromPrefs(Type type, string key, bool unique = true) => Prefs.GetObject(type, key, unique);



        public static void OpenFolder(string folder) => EditorUtility.OpenWithDefaultApp(folder);

        public static string ToAbsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            assetRootPath = assetRootPath.Substring(0, assetRootPath.Length - 6) + self;
            return assetRootPath.ToRegularPath();
        }
        public static string ToAssetsPath(this string self) => "Assets" + Path.GetFullPath(self).Substring(Path.GetFullPath(Application.dataPath).Length).Replace("\\", "/");
        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }
        public static string ToUnixLineEndings(this string self) => self.Replace("\r\n", "\n").Replace("\r", "\n");

        public static IEnumerable<Type> GetSubTypesInAssemblies(this Type self)
        {
            if (self.IsInterface)
                return AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(item => item.GetTypes())
                                .Where(item => item.GetInterfaces().Contains(self));
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(item => item.GetTypes())
                            .Where(item => item.IsSubclassOf(self));
        }

        public static string ToRegularPath(this string path) => path.Replace('\\', '/');

        public static string CombinePath(this string path, string toCombinePath) => Path.Combine(path, toCombinePath).ToRegularPath();
        public static void CreateDirectories(List<string> directories)
        {
            foreach (var path in directories)
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
        }
    }
}
