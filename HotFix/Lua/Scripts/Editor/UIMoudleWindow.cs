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

namespace IFramework.UI
{
    partial class UIMoudleWindow
    {

        public static void Lua_BuildPanelNames()
        {
            Collect();
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
    }
}

