/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    partial class EditorTools
    {
        static class Prefs
        {
            const string dot = ",";
            private static string StringToPath(string key, bool unique)
            {
                if (unique)
                    return EditorTools.projectMemoryPath_unique.CombinePath("Prefs_" + key.Replace("/", "_") + ".txt").ToAssetsPath();
                return EditorTools.projectMemoryPath.CombinePath("Prefs_" + key.Replace("/", "_") + ".txt").ToAssetsPath();
            }
            private static string GetKey(Type type,string key)
            {
                return string.Format("{0}/{1}", type, key);
            }
            private static string GetString(Type type, string key, bool unique)
            {
                var path = StringToPath(GetKey(type,key), unique);
                if (File.Exists(path))
                {
                    var result = File.ReadAllText(path);
                    if (unique)
                    {
                        var index = result.IndexOf(dot);
                        if (result.Substring(0, index) == SystemInfo.deviceUniqueIdentifier)
                            result = result.Substring(index + 1);
                        else
                            result = string.Empty;
                    }
                    return result;
                }
                return string.Empty;
            }
     
            private static void SetString(Type type, string key, string value, bool unique)
            {
                var path = StringToPath(GetKey(type, key), unique);
                if (unique)
                {
                    value = SystemInfo.deviceUniqueIdentifier + dot + value;
                }
                File.WriteAllText(path, value);
                AssetDatabase.Refresh();
            }
            public static void SetObject(Type type, string key, object value, bool unique)
            {
                SetString(type,key, JsonUtility.ToJson(value, true), unique);
            }
            public static V GetObject<V>(Type type, string key, bool unique)
            {
                var str = GetString(type,key, unique);
                return JsonUtility.FromJson<V>(str);
            }
        }
    }
}
