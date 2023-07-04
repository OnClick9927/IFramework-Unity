/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System.Text;
using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace IFramework.UI
{
    public partial class UIMoudleWindow
    {
        public class GenItemCodeCS : UIMoudleWindowTab
        {
            public enum ItemType
            {
                UIItem,
                GameObject,
                UIObject,
            }
            public override string name => "CS/Gen_item_Code_CS";
             private FloderField FloderField;

            [SerializeField] private ItemType _type;
            [SerializeField] private string UIdir = "";
            [SerializeField] private GameObject gameobject;
            [SerializeField] private TreeViewState state = new TreeViewState();
            private ScriptCreaterFieldsDrawer fields;
            private ScriptCreater creater = new ScriptCreater();
            private string panelName { get { return gameobject.name; } }
            private string viewName { get { return $"{panelName}View"; } }
            public override void OnEnable()
            {
                var last = EditorTools.GetFromPrefs<GenItemCodeCS>(name);
                if (last != null)
                {
                    this.gameobject = last.gameobject;
                    this.UIdir = last.UIdir;
                    this.state = last.state;
                    this._type = last._type;
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
                    if (gameobject == null)
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
                _type = (ItemType)EditorGUILayout.EnumPopup("Type", _type);


                EditorGUI.BeginChangeCheck();

                gameobject = EditorGUILayout.ObjectField("GameObject", gameobject, typeof(GameObject), false) as GameObject;
                if (EditorGUI.EndChangeCheck())
                {
                    SetViewData();
                }
                GUILayout.Space(10);
                fields.OnGUI();
            }
            private void SetViewData()
            {

                if (gameobject != null)
                {
                    creater.SetGameObject(gameobject.gameObject);
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

                    string designPath = FloderField.path.CombinePath($"{viewName}.Design.cs");
                    string txt = File.ReadAllText(designPath);
                    if (txt.Contains($": {typeof(UIObjectView)}"))
                        _type = ItemType.UIObject;
                    if (txt.Contains($": {typeof(GameObjectView)}"))
                        _type = ItemType.GameObject;
                    if (txt.Contains($": {typeof(UIItemView)}"))
                        _type = ItemType.UIItem;
                }
            }



            private void WriteView()
            {
                string designPath = UIdir.CombinePath($"{viewName}.Design.cs");
                string path = UIdir.CombinePath($"{viewName}.cs");

                WriteTxt(designPath, viewDesignScriptOrigin().ToUnixLineEndings(),
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
                    WriteTxt(path, ViewTxt(), null);
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


            private string ViewTxt()
            {
                string add = "";
                if (_type == ItemType.UIItem)
                {
                    add = "" +
 "\t\tprotected override void OnGet()\n" +
 "\t\t{\n" +
 "\t\t}\n" +
"\t\tpublic override void OnSet()\n" +
"\t\t{\n" +
"\t\t}\n";
                }
                return head +
 "namespace #UserNameSpace#\n" +
 "{\n" +
 $"\tpublic partial class #UserSCRIPTNAME# \n" +
 "\t{\n" + add +

 "\t}\n" +
 "}";
            }
            private string viewDesignScriptOrigin()
            {
                Type pa = null;
                switch (_type)
                {
                    case ItemType.UIItem:
                        pa = typeof(UIItemView);
                        break;
                    case ItemType.GameObject:
                        pa = typeof(GameObjectView);
                        break;
                    case ItemType.UIObject:
                        pa = typeof(UIObjectView);
                        break;
                    default:
                        break;
                }
                return head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            $"\tpublic partial class #UserSCRIPTNAME# : {pa.FullName} \n" +
            "\t{\n" +
             "#field#\n" +
            "\t\tprotected override void InitComponents()\n" +
            "\t\t{\n" +
            "#findfield#\n" +
            "\t\t}\n" +
            "\t}\n" +
            "}";
            }
        }
    }
}
