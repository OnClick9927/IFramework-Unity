/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using IFramework.UI;

namespace IFramework.Hotfix.Lua
{

    static partial class LuaEditorTools
    {
        [Serializable]
        class MVC_GenCodeView_Lua : UI.UIMoudleWindow.UIGenCode<UIPanel>
        {
            public override string name => "Lua/MVC_Gen_Lua";

            protected override GameObject gameobject => panel.gameObject;

            protected override string viewScriptName => $"{viewName}.lua.txt";
            protected override void OnFindDirSuccess() { }

            protected override void LoadLastData()
            {
                var last = EditorTools.GetFromPrefs<MVC_GenCodeView_Lua>(name);
                if (last != null)
                {
                    this.UIdir = last.UIdir;
                    this.panel = last.panel;
                    this.state = last.state;
                }
            }

            protected override void WriteView()
            {
                string path = UIdir.CombinePath(viewScriptName);
                if (File.Exists(path))
                    UpdateLua(creater, $"function {viewName}:OnLoad()", path);
                else
                    WriteLua(creater, path, viewName, vSource);

            }

            static string StaticUse(ScriptCreater creater)
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
            static string Fields(ScriptCreater creater)
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

            public static void WriteLua(ScriptCreater creater, string path, string viewName, string source)
            {
                string result = source.Replace(ViewNameFlag, viewName)
                .Replace(ViewUseFlag, StaticUse(creater))
                .Replace(ViewFeildFlag, Fields(creater));
                File.WriteAllText(path, result.ToUnixLineEndings());
            }
            public static void UpdateLua(ScriptCreater creater, string target, string path)
            {
                string txt = File.ReadAllText(path);
                int start = txt.IndexOf(target);
                int end = -1;
                start = txt.IndexOf("self.Controls", start);
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


            public const string ViewUseFlagFT = "---ViewUseFlag";
            public const string ViewUseFlag = "--using";
            public const string ViewFeildFlag = "--Find";
            public const string ViewNameFlag = "#ViewName#";

            public static string head = "--*********************************************************************************\n" +
              "--Author:         " + EditorTools.ProjectConfig.UserName + "\n" +
              "--Version:        " + EditorTools.ProjectConfig.Version + "\n" +
              "--UnityVersion:   " + Application.unityVersion + "\n" +
              "--Date:           " + DateTime.Now.ToString("yyyy-MM-dd") + "\n" +
              "--*********************************************************************************\n";
            static string vSource = head + "\n" +
             $"{ViewUseFlagFT}\n" +
             $"{ViewUseFlag}\n\n" +
             $"{ViewUseFlagFT}\n" +
              $"---@class {ViewNameFlag} : UIView_MVC" + "\n" +
             $"local {ViewNameFlag} = class(\"{ViewNameFlag}\",UIView_MVC)\n\n" +
             $"function {ViewNameFlag}:OnLoad()\n" +
             "\tself.Controls = {\n" +
              ViewFeildFlag + "\n\t}\n\n" +
             "end\n\n" +
             $"function {ViewNameFlag}:OnShow()\n\n" +
             "end\n\n" +
             $"function {ViewNameFlag}:OnHide()\n\n" +
             "end\n\n" +
             $"function {ViewNameFlag}:OnClose()\n" +
             "\tself.Controls = nil\n" +
             "end\n\n" +

             $"return {ViewNameFlag}";




        }
    }


}

