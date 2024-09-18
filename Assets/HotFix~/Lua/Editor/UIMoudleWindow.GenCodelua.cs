/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using IFramework.UI;
using System.Collections.Generic;
using System.Linq;
using static IFramework.UI.UIModuleWindow;
using IFramework.Hotfix.Lua;

namespace IFramework.UI
{
    partial class UIModuleWindow
    {
        [Serializable]

        class GenCodelua : UIGenCode<GameObject>
        {
            public enum ItemType
            {
                UIItem,
                LuaObject,
                UIObject,
                UIView_MVC,
            }
            public override string name => "Lua";
            protected override GameObject gameObject => panel;

            protected override string viewScriptName => $"{panelName}.lua.txt";
            [SerializeField] private ItemType _type;


            protected override void OnFindDirSuccess()
            {
                string txt = File.ReadAllText(scriptPath);
                var names = Enum.GetNames(typeof(ItemType));
                foreach (var item in names)
                {
                    if (txt.Contains($": {item}"))
                    {
                        _type = (ItemType)Enum.Parse(typeof(ItemType), item);
                        break;
                    }
                }
            }

            protected override void LoadLastData()
            {
                var last = EditorTools.GetFromPrefs<GenCodelua>(name);
                if (last != null)
                {
                    this.UIdir = last.UIdir;
                    this.panel = last.panel;
                    this._type = last._type;
                    this.state = last.state;
                }
            }

            protected override void WriteView()
            {
                string path = scriptPath;

                if (File.Exists(path))
                {
                    if (_type == ItemType.UIView_MVC)
                        UpdateLua(creator, $"function {viewName}:OnLoad()", path);
                    else
                        UpdateLua(creator, $"function {viewName}:ctor(gameObject)", path);
                }
                else
                {
                    string source = vSource;
                    if (_type == ItemType.UIItem)
                        source = vSource;
                    if (_type == ItemType.LuaObject)
                        source = ObjSource;
                    if (_type == ItemType.UIObject)
                        source = UISource;
                    if (_type == ItemType.UIView_MVC)
                        source = mvcSource;
                    WriteLua(creator, path, viewName, source);
                }
            }


            protected override void Draw()
            {
                _type = (ItemType)EditorGUILayout.EnumPopup("Type", _type);

            }
            public static void Lua_BuildPanelNames()
            {
                var datas = UICollectData.collect.datas;

                string s = "local M = \n" +
                    "{\n";
                foreach (var data in datas)
                {
                    s += $"\t {data.name} = \"{data.path}\";\n";
                }
                s += "}\n" +
                    "return M";
                File.WriteAllText(LuaEditorPaths.lua_panel_names_path, s);
                AssetDatabase.Refresh();
            }
            public override void OnGUI()
            {
                base.OnGUI();
                if (GUILayout.Button("Build Panel Names"))
                {
                    Lua_BuildPanelNames();
                }
            }


            private static string Get_base(ItemType type)
            {
                string _base = type.ToString();
                string add = type != ItemType.UIItem ? "\tself:SetGameObject(gameObject)\n" : "";
                return
                head + "\n" +
                $"{ViewUseFlagFT}\n" +
                ViewUseFlag + "\n\n" +
                $"{ViewUseFlagFT}\n" +
                $"---@class {ViewNameFlag} : {_base}\n" +
                $"local {ViewNameFlag} = class(\"{ViewNameFlag}\",{_base})\n\n" +
                $"function {ViewNameFlag}:ctor(gameObject)\n" +
                 add +
                $"\t{ctrls} " + "= {\n" +
                ViewFeildFlag + "\n" +
                "\t}\n" +
                "end\n\n";
            }


            static string vSource
            {
                get
                {
                    return Get_base(ItemType.UIItem) +
    $"function {ViewNameFlag}:OnGet()\n" +
    "end\n\n" +
    $"function {ViewNameFlag}:OnSet()\n\n" +
    "end\n\n" +
    $"return {ViewNameFlag}";
                }
            }



            static string ObjSource
            {
                get
                {
                    return Get_base(ItemType.LuaObject) + "return " + ViewNameFlag;
                }
            }
            static string UISource
            {
                get
                {
                    return Get_base(ItemType.UIObject) + "return " + ViewNameFlag;

                }
            }
            static string mvcSource = head + "\n" +
            $"{ViewUseFlagFT}\n" +
            $"{ViewUseFlag}\n\n" +
            $"{ViewUseFlagFT}\n" +
             $"---@class {ViewNameFlag} : UIView_MVC" + "\n" +
            $"local {ViewNameFlag} = class(\"{ViewNameFlag}\",UIView_MVC)\n\n" +
            $"function {ViewNameFlag}:OnLoad()\n" +
            $"\t{ctrls} " + "= {\n" +
             ViewFeildFlag + "\n\t}\n\n" +
            "end\n\n" +
            $"function {ViewNameFlag}:OnShow()\n\n" +
            "end\n\n" +
            $"function {ViewNameFlag}:OnHide()\n\n" +
            "end\n\n" +
            $"function {ViewNameFlag}:OnClose()\n" +
            $"\t{ctrls} = nil\n" +
            "end\n\n" +

            $"return {ViewNameFlag}";

            public static void WriteLua(ScriptCreator creater, string path, string viewName, string source)
            {
                string result = source.Replace(ViewNameFlag, viewName)
                .Replace(ViewUseFlag, StaticUse(creater))
                .Replace(ViewFeildFlag, Fields(creater));
                File.WriteAllText(path, result.ToUnixLineEndings());
            }
            public static void UpdateLua(ScriptCreator creater, string target, string path)
            {
                string txt = File.ReadAllText(path);
                int start = txt.IndexOf(target);
                int end = -1;
                start = txt.IndexOf(ctrls, start);
                int depth = 0;
                for (int i = start + 1; i < txt.Length; i++)
                {
                    char data = txt[i];
                    if (data == '{')
                    {
                        if (depth == 0)
                            start = i;
                        depth++;
                    }
                    else if (data == '}')
                        depth--;
                    else
                    {
                        continue;
                    }
                    if (depth == 0)
                    {
                        end = i;
                        break;
                    }
                }

                if (end == -1) return;
                txt = txt.Remove(start, end - start + 1);
                string fs = $"{{\n {Fields(creater)}\n\t}}";
                txt = txt.Insert(start, fs);
                start = txt.IndexOf(ViewUseFlagFT);
                end = txt.IndexOf(ViewUseFlagFT, start + ViewUseFlagFT.Length);
                txt = txt.Remove(start, end - start);
                File.WriteAllText(path, txt.Insert(start, $"{ViewUseFlagFT}\n{StaticUse(creater)}\n"));
            }
            static string StaticUse(ScriptCreator creater)
            {
                List<string> usings = new List<string>();
                var marks = creater.GetMarks();
                if (marks != null)
                {
                    for (int i = 0; i < marks.Count; i++)
                    {
                        string ns = marks[i].fieldType;
                        if (!usings.Contains(ns))
                        {
                            usings.Add(ns);
                        }
                    }
                }
                string use = "";
                for (int i = 0; i < usings.Count; i++)
                {
                    var one = i == usings.Count - 1 ? "" : "\n";
                    use += $"local {usings[i].Split('.').Last()} = StaticUsing(\"{usings[i]}\"){one}";
                }
                return use;
            }
            static string Fields(ScriptCreator creater)
            {
                var marks = creater.GetMarks();

                string f = "";

                string root_path = creater.gameObject.transform.GetPath();
                if (marks != null)
                {
                    for (int i = 0; i < marks.Count; i++)
                    {
                        string fieldType = marks[i].fieldType;
                        string fieldName = marks[i].fieldName;
                        string type = marks[i].fieldType.Split('.').Last();
                        if (marks[i].gameObject == creater.gameObject)
                        {
                            var end = i == marks.Count - 1 ? "" : "\n";
                            f += $"\t\t---@type {fieldType}\n";
                            f += $"\t\t{fieldName} = self:GetComponent(\"\", typeof({type})),{end}";
                        }
                        else
                        {

                            string path = marks[i].transform.GetPath();
                            path = path.Remove(0, root_path.Length + 1);
                            var end = i == marks.Count - 1 ? "" : "\n";
                            f += $"\t\t---@type {fieldType}\n";
                            f += $"\t\t{fieldName} = self:GetComponent(\"{path}\", typeof({type})),{end}";
                        }
                    }
                }

                return f;
            }

            public const string ViewUseFlagFT = "---ViewUseFlag";
            public const string ViewUseFlag = "--using";
            public const string ViewFeildFlag = "--Find";
            public const string ViewNameFlag = "#ViewName#";
            public const string ctrls = "self.Views";

            public static string head = "--*********************************************************************************\n" +
              "--Author:         " + EditorTools.ProjectConfig.UserName + "\n" +
              "--Version:        " + EditorTools.ProjectConfig.Version + "\n" +
              "--UnityVersion:   " + Application.unityVersion + "\n" +
              "--Date:           " + DateTime.Now.ToString("yyyy-MM-dd") + "\n" +
              "--*********************************************************************************\n";


        }
    }


}

