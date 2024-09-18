/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace IFramework.Hotfix.Lua
{
    [InitializeOnLoad]
    class LuaEditorPaths
    {
        public static string lua_ui_path { get { return hotFixScriptPath.CombinePath("UI"); } }
        public static string lua_panel_names_path { get { return lua_ui_path.CombinePath("PanelNames.lua.txt"); } }

        public static string hotFixScriptPath { get { return EditorTools.projectPath.CombinePath("Lua"); } }
        static LuaEditorPaths()
        {
            var directorys = new List<string>()
            {
                LuaEditorPaths.hotFixScriptPath,
                LuaEditorPaths.lua_ui_path
            };
            var files = new List<string>()
            {
                LuaEditorPaths.hotFixScriptPath.CombinePath("FixCsharp.lua.txt"),
                LuaEditorPaths.hotFixScriptPath.CombinePath("GameLogic.lua.txt"),
                LuaEditorPaths.hotFixScriptPath.CombinePath("GlobalDefine.lua.txt"),
            };
            foreach (var path in directorys)
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            for (int index = 0; index < files.Count; index++)
            {
                string path = files[1];
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

