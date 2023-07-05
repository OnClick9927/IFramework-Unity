/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/
using UnityEngine;
using System;
using System.IO;
using static IFramework.UI.UIMoudleWindow;
using System.Text;

namespace IFramework.UI.MVC
{
    public partial class UIMoudleWindow
    {
        [Serializable]
        public class MVC_GenCodeView : UIGenCode<UIPanel>
        {
            public override string name { get { return "CS/MVC_Gen_CS"; } }

            protected override GameObject gameobject => panel.gameObject;
            protected string designScriptName { get { return $"{viewName}.Design.cs"; } }
            protected override string viewScriptName { get { return $"{viewName}.cs"; } }

            protected override void OnFindDirSuccess()
            {
                throw new NotImplementedException();
            }

            protected override void LoadLastData()
            {
                var last = EditorTools.GetFromPrefs<MVC_GenCodeView>(name);
                if (last != null)
                {
                    this.panel = last.panel;
                    this.UIdir = last.UIdir;
                    this.state = last.state;
                }
            }
    
            protected override void WriteView()
            {
                string designPath = UIdir.CombinePath(designScriptName);
                string path = UIdir.CombinePath(viewScriptName);

                WriteTxt(designPath, viewDesignScriptOrigin,
                (str) =>
                {
                    string field;
                    string find;
                    Fields(out field, out find);
                    return str.Replace("#PanelType#", panelName)
                    .Replace("#field#", field)
                    .Replace("#findfield#", find)
                    .Replace(".Design", "");
                });

                if (!File.Exists(path))
                {
                    WriteTxt(path, viewScriptOrigin, null);
                }
            }



            private void Fields(out string field, out string find)
            {
                var marks = creater.GetMarks();


                StringBuilder f = new StringBuilder();
                StringBuilder functionField = new StringBuilder();
                if (marks != null)
                {
                    string root_path = creater.gameObject.transform.GetPath();

                    for (int i = 0; i < marks.Count; i++)
                    {
                        string fieldType = marks[i].fieldType;
                        string fieldName = marks[i].fieldName;

                        string path = marks[i].transform.GetPath();

                        path = path.Remove(0, root_path.Length + 1);
                        f.AppendLine($"\t\tprivate {fieldType} {fieldName};");
                        functionField.AppendLine($"\t\t\t{fieldName} = transform.Find(\"{path}\").GetComponent<{fieldType}>();");
                    }
                }
                field = f.ToString();
                find = functionField.ToString();

            }


            private static void WriteTxt(string writePath, string source, Func<string, string> func)
            {
                source = source.Replace("#User#", EditorTools.ProjectConfig.UserName)
                         .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(writePath))
                           .Replace("#UserNameSpace#", EditorTools.ProjectConfig.NameSpace)
                           .Replace("#UserVERSION#", EditorTools.ProjectConfig.Version)
                           .Replace("#UserUNITYVERSION#", Application.unityVersion)
                           .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd")).ToUnixLineEndings();
                if (func != null)
                    source = func.Invoke(source);
                File.WriteAllText(writePath, source, System.Text.Encoding.UTF8);
            }

   

        

            private const string head = "/*********************************************************************************\n" +
            " *Author:         #User#\n" +
            " *Version:        #UserVERSION#\n" +
            " *UnityVersion:   #UserUNITYVERSION#\n" +
            " *Date:           #UserDATE#\n" +
            "*********************************************************************************/\n";



            private const string viewDesignScriptOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME# : IFramework.UI.MVC.UIView \n" +
            "\t{\n" +
             "#field#\n" +
            "\t\tprotected override void InitComponents()\n" +
            "\t\t{\n" +
            "#findfield#\n" +
            "\t\t}\n" +
            "\t}\n" +
            "}";
            private const string viewScriptOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME#\n" +
            "\t{\n" +
            "\t\tprotected override void OnLoad()\n" +
            "\t\t{\n" +

            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnShow()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
             "\t\tprotected override void OnHide()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnClose()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t}\n" +
            "}";


        }
    }
}
