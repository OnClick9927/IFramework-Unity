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
        static void CreateDirectories(List<string> directorys)
        {
            foreach (var path in directorys)
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
        }
        static EditorTools()
        {
            var directorys = new List<string>()
            {
                EditorTools.projectMemoryPath,
            };
            CreateDirectories(directorys);

            AssetDatabase.Refresh();
            Log.loger = new UnityLogger();
            Log.enable_L = ProjectConfig.enable_L;
            Log.enable_W = ProjectConfig.enable_W;
            Log.enable_E = ProjectConfig.enable_E;
            Log.enable = ProjectConfig.enable;
            directorys = new List<string>()
            {
                EditorTools.projectPath,
                EditorTools.projectConfigPath,
                EditorTools.projectScriptPath,
            };
            CreateDirectories(directorys);
            AssetDatabase.Refresh();

        }


        public const string projectMemoryPath = "Assets/Editor/IFramework";

        public static string projectPath => ProjectConfig.projectPath;
        public static string projectConfigPath { get { return projectPath.CombinePath("Configs"); } }
        public static string projectScriptPath { get { return projectPath.CombinePath("Scripts"); } }


        public static void SaveToPrefs<T>(T value, string key, bool unique = true)
        {
            Prefs.SetObject(value.GetType(), key, value, unique);
        }
        public static T GetFromPrefs<T>(string key, bool unique = true)
        {
            return Prefs.GetObject<T>(typeof(T), key, unique);
        }
        public static void OpenFolder(string folder)
        {
            EditorUtility.OpenWithDefaultApp(folder);
        }

        public static string ToAbsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            assetRootPath = assetRootPath.Substring(0, assetRootPath.Length - 6) + self;
            return assetRootPath.ToRegularPath();
        }
        public static string ToAssetsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            return "Assets" + Path.GetFullPath(self).Substring(assetRootPath.Length).Replace("\\", "/");
        }
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
        public static string ToUnixLineEndings(this string self)
        {
            return self.Replace("\r\n", "\n").Replace("\r", "\n");
        }
        /// <summary>
        /// 获取所有程序集中的类型的子类，3.5有问题
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubTypesInAssemblys(this Type self)
        {
            if (self.IsInterface)
                return AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(item => item.GetTypes())
                                .Where(item => item.GetInterfaces().Contains(self));
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(item => item.GetTypes())
                            .Where(item => item.IsSubclassOf(self));
        }
        /// <summary>
        /// 规范路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToRegularPath(this string path)
        {
            path = path.Replace('\\', '/');
            return path;
        }
        /// <summary>
        /// 拼接路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="toCombinePath"></param>
        /// <returns></returns>
        public static string CombinePath(this string path, string toCombinePath)
        {
            return Path.Combine(path, toCombinePath).ToRegularPath();
        }

    }
}
