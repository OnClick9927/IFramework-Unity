/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IFramework.Lua
{

    public class LuaEditorTool
    {
        [UnityEditor.CustomEditor(typeof(LuaEnvConfig))]
        class LuaEnvConfigEditor : UnityEditor.Editor
        {
            LuaEnvConfig config { get { return target as LuaEnvConfig; } }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Build"))
                {
                    var files = new List<string>()
                    {
                        config.rootPath.CombinePath("FixCsharp.lua.txt"),
                        config.rootPath.CombinePath("GameLogic.lua.txt"),
                        config.rootPath.CombinePath("GlobalDefine.lua.txt"),
                    };
                    for (int index = 0; index < files.Count; index++)
                    {
                        string path = files[index];
                        if (File.Exists(path)) continue;
                        if (index == 0)
                            File.WriteAllText(path, "Log.L('Start Fix C# ')\n");
                        if (index == 1)
                            File.WriteAllText(path, "Log.L('Game Logic')\n");
                        if (index == 2)
                            File.WriteAllText(path, "Log.L('GlobalDefine')\n");
                    }
                    AssetDatabase.Refresh();
                }

            }
        }



        public class FilePostProcessor : AssetPostprocessor
        {
            private static Queue<string> _changed = new Queue<string>();
            private static Action<string> _reload;
            const string end = ".lua.txt";
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (!EditorApplication.isPlaying || !Application.isPlaying || !LuaHotFix.available) return;
                if (_reload == null)
                    _reload = LuaHotFix.Instance.gtable.Get<Action<string>>("UpdateFunctions");

                for (int index = 0; index < importedAssets.Length; index++)
                {
                    var path = importedAssets[index];
                    if (!path.EndsWith(end)) continue;
                    if (!path.Contains(LuaHotFix.config.rootPath)) continue;
                    path = path.Replace(LuaHotFix.config.rootPath, "");
                    path = path.Replace(end, "").Replace("/", ".").Remove(0, 1);
                    _changed.Enqueue(path);
                }

                if (_changed.Count == 0) return;
                EditorApplication.delayCall += () =>
                {
                    if (!EditorApplication.isPlaying || !Application.isPlaying || !LuaHotFix.available) return;
                    while (_changed.Count > 0)
                    {
                        string str = _changed.Dequeue();
                        if (_reload != null)
                        {
                            _reload(str);
                            Debug.Log($"<color=#00A0A0> 重载 Lua 代码 :{str}</color>");
                        }
                    }
                    _reload = null;
                };
            }
        }

    }
}

