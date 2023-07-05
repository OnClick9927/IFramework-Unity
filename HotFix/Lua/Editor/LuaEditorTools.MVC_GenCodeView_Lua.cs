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
                CreateView(UIdir.CombinePath(viewScriptName));
            }

            private void CreateView(string path)
            {
                if (File.Exists(path))
                {
                    var target = string.Format("function {0}View:OnLoad()", panelName);
                    var txt = File.ReadAllText(path);
                    int start = txt.IndexOf(target);
                    start = txt.IndexOf("self.Controls", start);
                    int depth = 0;
                    int end = -1;
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
                    string fs = $"{{\n {Fields()}\n\t}}";
                    txt = txt.Insert(start, fs);
                    var flag = "---ViewUseFlag";
                    start = txt.IndexOf(flag);
                    end = txt.IndexOf(flag, start + flag.Length);
                    txt = txt.Remove(start, end - start);
                    File.WriteAllText(path, txt.Insert(start, $"{flag}\n {StaticUse()}\n"));
                }
                else
                {

                    string result = vSource.Replace("#PanelName#", panelName)
                        .Replace(ViewUseFlag, StaticUse())
                            .Replace(ViewFeildFlag, Fields());
                    File.WriteAllText(path, result.ToUnixLineEndings());
                }
            }
            private string StaticUse()
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
            private string Fields()
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

                        string path = marks[i].transform.GetPath();
                        path = path.Remove(0, root_path.Length + 1);
                        f += "\t\t---@type " + fieldType + "\n";
                        f += string.Format("\t\t{0} = self:GetComponent(\"{1}\", typeof({2})),{3}", fieldName, path, type, i == marks.Count - 1 ? "" : "\n");
                    }
                }

                return f;
            }

     

            public const string ViewUseFlag = "--using";
            public const string ViewFeildFlag = "--Find";
            public static string head = "--*********************************************************************************\n" +
              "--Author:         " + EditorTools.ProjectConfig.UserName + "\n" +
              "--Version:        " + EditorTools.ProjectConfig.Version + "\n" +
              "--UnityVersion:   " + Application.unityVersion + "\n" +
              "--Date:           " + DateTime.Now.ToString("yyyy-MM-dd") + "\n" +
              "--*********************************************************************************\n";
            static string vSource = head + "\n" +
             "---ViewUseFlag\n" +
             ViewUseFlag + "\n\n" +
             "---ViewUseFlag\n" +
              "---@class #PanelName#View : UIView_MVC" + "\n" +
             "local #PanelName#View = class(\"#PanelName#View\",UIView_MVC)\n\n" +
             "function #PanelName#View:OnLoad()\n" +
             "\tself.Controls = {\n" +
              ViewFeildFlag + "\n\t}\n" +
              "\t--BindUIEvent\n\n" +
             "end\n\n" +
             "function #PanelName#View:OnShow()\n\n" +
             "end\n\n" +
             "function #PanelName#View:OnHide()\n\n" +
             "end\n\n" +
             "function #PanelName#View:OnClose()\n" +
             "\tself.Controls = nil\n" +
             "end\n\n" +

             "return " + "#PanelName#View";




        }
    }


}

