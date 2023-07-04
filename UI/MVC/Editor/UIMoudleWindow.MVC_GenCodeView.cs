/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/
using UnityEditor;
using IFramework;
using UnityEngine;
using System;
using System.IO;
using static IFramework.UI.UIMoudleWindow;
using static IFramework.EditorTools;
using System.Text;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace IFramework.UI.MVC
{
    public partial class UIMoudleWindow
    {
        [Serializable]
        public class MVC_GenCodeView : UIMoudleWindowTab
        {
            public override string name { get { return "CS/MVC_Gen_CS"; } }
            [SerializeField] private string UIdir = "";
            [SerializeField] private UIPanel panel;
            [SerializeField] private FloderField FloderField;
            [SerializeField] private TreeViewState state = new TreeViewState();
            private ScriptCreaterFieldsDrawer fields;
            private ScriptCreater creater = new ScriptCreater();
            private string panelName { get { return panel.name; } }
            private string viewName { get { return $"{panelName}View"; } }
            public override void OnEnable()
            {
                var last = EditorTools.GetFromPrefs<MVC_GenCodeView>(name);
                if (last != null)
                {
                    this.panel = last.panel;
                    this.UIdir = last.UIdir;
                    this.state = last.state;
                }
                this.FloderField = new FloderField(UIdir);
                fields = new ScriptCreaterFieldsDrawer(creater, state);
                SetViewData();
            }
            public override void OnDisable()
            {
                EditorTools.SaveToPrefs(this, name);
            }
            public override void OnHierarchyChanged()
            {
                creater.ColllectMarks();
            }
            public override void OnGUI()
            {
                if (EditorApplication.isCompiling)
                {
                    GUILayout.Label("Editor is Compiling");
                    GUILayout.Label("please wait");
                    return;
                }
                if (GUILayout.Button("Gen"))
                {
                    if (panel == null)
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Select UI Panel"));
                        return;
                    }
                    if (string.IsNullOrEmpty(UIdir))
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Map Gen Dir "));
                        return;
                    }
                    WriteView();
                    AssetDatabase.Refresh();
                }
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Panel Directory", GUIStyles.toolbar);
                    GUILayout.Space(20);

                    FloderField.OnGUI(EditorGUILayout.GetControlRect());
                    UIdir = FloderField.path;
                    GUILayout.EndHorizontal();
                }


                EditorGUI.BeginChangeCheck();

                panel = EditorGUILayout.ObjectField("UIPanel", panel, typeof(UIPanel), false) as UIPanel;
                if (EditorGUI.EndChangeCheck())
                {
                    SetViewData();
                }
                GUILayout.Space(10);
                fields.OnGUI();
            }
            private void SetViewData()
            {

                if (panel != null)
                {
                    creater.SetGameObject(panel.gameObject);
                    FindDir();
                }
                else
                {
                    creater.SetGameObject(null);
                    FloderField.SetPath(string.Empty);
                }
            }
            private void FindDir()
            {
                string total = $"{viewName}.cs";
                string find = AssetDatabase.GetAllAssetPaths().ToList().Find(x => x.EndsWith(total));
                if (string.IsNullOrEmpty(find))
                {
                    FloderField.SetPath(string.Empty);
                }
                else
                {
                    FloderField.SetPath(find.Replace(total, "").ToAssetsPath());
                }
            }



            private void WriteView()
            {
                string designPath = UIdir.CombinePath($"{viewName}.Design.cs");
                string path = UIdir.CombinePath($"{viewName}.cs");

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
