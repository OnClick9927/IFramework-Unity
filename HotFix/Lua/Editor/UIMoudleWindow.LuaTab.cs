/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.IO;
using IFramework.Hotfix.Lua;
using UnityEngine;

namespace IFramework.UI
{
    partial class UIMoudleWindow
    {
        public class LuaTab : UIMoudleWindowTab
        {
            public static void Lua_BuildPanelNames()
            {
                string s = "local M = \n" +
                    "{\n";
                foreach (var data in collect.datas)
                {
                    s += $"\t {data.name} = \"{data.path}\";\n";
                }
                s += "}\n" +
                    "return M";
                File.WriteAllText(LuaEditorPaths.lua_panel_names_path, s);
                AssetDatabase.Refresh();
            }

            public override string name => "Lua";

            public override void OnGUI()
            {
                if (GUILayout.Button("Build Panel Names"))
                {
                    Lua_BuildPanelNames();
                }
            }
        }

    }
}

