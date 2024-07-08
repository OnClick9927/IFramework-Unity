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
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace IFramework.UI
{
    public partial class UIModuleWindow
    {
        public class UIGenCodeCS : UIGenCode<GameObject>
        {
            public enum ItemType
            {
                UIItem,
                GameObject,
                //UIObject,
                MVCView
            }
            public override string name => "CS";
            protected override string GetViewScriptName(string viewName) => $"{viewName}.cs";


            [SerializeField] private ItemType _type;


            protected override GameObject gameObject => panel;

            protected override void OnFindDirFail()
            {
                UIPanel find = gameObject.GetComponent<UIPanel>();
                if (find != null)
                {
                    _type = ItemType.MVCView;
                }
                else
                {
                    if (_type == ItemType.MVCView)
                        _type = ItemType.GameObject;
                }
            }

            protected override void OnFindDirSuccess()
            {
                string txt = File.ReadAllText(scriptPath);
                //if (txt.Contains($": {typeof(UIObjectView)}"))
                //    _type = ItemType.UIObject;
                if (txt.Contains($"{typeof(IFramework.UI.GameObjectView).FullName}"))
                    _type = ItemType.GameObject;
                if (txt.Contains($"{typeof(IFramework.UI.UIItemView).FullName}"))
                    _type = ItemType.UIItem;
                if (txt.Contains($"{typeof(IFramework.UI.MVC.UIView).FullName}"))
                    _type = ItemType.MVCView;
            }

            protected override void LoadLastData(UIGenCode<GameObject> _last)
            {
                var last = _last as UIGenCodeCS;
                this._type = last._type;
            }
            public override void GenPanelNames(PanelPathCollect collect, string scriptGenPath, string scriptName)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"public class {scriptName}");
                sb.AppendLine("{");
                var datas = collect.datas;
                foreach (var data in datas)
                {
                    sb.AppendLine($"\t public static string {data.name} = \"{data.path}\";");
                }
                sb.AppendLine("}");
                File.WriteAllText(scriptGenPath.CombinePath($"{scriptName}.cs"), sb.ToString());
                AssetDatabase.Refresh();
            }
            protected override void Draw()
            {

                _type = (ItemType)EditorGUILayout.EnumPopup("Type", _type);
            }
       

            protected override void WriteView()
            {
                Write(creator, scriptPath, viewDesignScriptOrigin());
            }

            public static void Write(ScriptCreator creator, string path, string origin)
            {
                if (File.Exists(path))
                {
                    var all = File.ReadAllLines(path).ToList();
                    int cs, ce, fe, fs;
                    cs = ce = fs = fe = 0;
                    for (int i = 0; i < all.Count; i++)
                    {
                        if (all[i].Contains(FieldsStart))
                            fs = i;
                        if (all[i].Contains(FieldsEnd))
                            fe = i;
                        if (all[i].Contains(InitComponentsStart))
                            cs = i;
                        if (all[i].Contains(InitComponentsEnd))
                        {
                            ce = i; break;
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < all.Count; i++)
                    {
                        if (i < fs)
                            sb.AppendLine(all[i]);
                        else if (i == fs)
                        {
                            sb.AppendLine(all[i]);

                            sb.AppendLine(Field);
                        }
                        else if (i < fe) { }
                        else if (i < cs)
                            sb.AppendLine(all[i]);

                        else if (i == cs)
                        {
                            sb.AppendLine(all[i]);
                            sb.AppendLine(FindField);
                        }

                        else if (i < ce) { }
                        else
                            sb.AppendLine(all[i]);

                    }
                    origin = sb.ToString();
                }
                WriteTxt(path, origin.ToUnixLineEndings(),
                   (str) =>
                   {
                       string field;
                       string find;
                       Fields(creator, out field, out find);
                       return str
                       //.Replace("#PanelType#", panelName)
                       .Replace(Field, field)
                       .Replace(FindField, find);
                   });

            }

            private static void Fields(ScriptCreator creater, out string field, out string find)
            {
                var marks = creater.GetMarks();
                if (creater.containsChildren)
                    marks = creater.GetAllMarks();

                StringBuilder f = new StringBuilder();
                StringBuilder functionField = new StringBuilder();
                if (marks != null)
                {
                    string root_path = creater.gameObject.transform.GetPath();

                    for (int i = 0; i < marks.Count; i++)
                    {
                        string fieldType = marks[i].fieldType;
                        string fieldName = marks[i].fieldName;
                        if (marks[i].gameObject == creater.gameObject)
                        {
                            f.AppendLine($"\t\tprivate {fieldType} {fieldName};");
                            if (fieldType == typeof(GameObject).FullName)
                                functionField.AppendLine($"\t\t\t{fieldName} = gameObject;");
                            else if (fieldType == typeof(Transform).FullName)
                                functionField.AppendLine($"\t\t\t{fieldName} = transform;");
                            else
                                functionField.AppendLine($"\t\t\t{fieldName} = transform.GetComponent<{fieldType}>();");
                        }
                        else
                        {
                            string path = marks[i].gameObject.transform.GetPath();
                            if (creater.containsChildren)
                            {
                                if (creater.IsIgnorePath(path))
                                    continue;
                            }
                            path = path.Remove(0, root_path.Length + 1);
                            f.AppendLine($"\t\tprivate {fieldType} {fieldName};");
                            if (fieldType == typeof(GameObject).FullName)
                                functionField.AppendLine($"\t\t\t{fieldName} = transform.Find(\"{path}\").gameObject;");
                            else if (fieldType == typeof(Transform).FullName)
                                functionField.AppendLine($"\t\t\t{fieldName} = transform.Find(\"{path}\");");
                            else
                                functionField.AppendLine($"\t\t\t{fieldName} = transform.Find(\"{path}\").GetComponent<{fieldType}>();");
                        }

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
                           .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd"));
                if (func != null)
                    source = func.Invoke(source);
                File.WriteAllText(writePath, source.ToUnixLineEndings(), Encoding.UTF8);
            }


            public const string head = "/*********************************************************************************\n" +
            " *Author:         #User#\n" +
            " *Version:        #UserVERSION#\n" +
            " *UnityVersion:   #UserUNITYVERSION#\n" +
            " *Date:           #UserDATE#\n" +
            "*********************************************************************************/\n" +
                "using static IFramework.UI.UnityEventHelper;\r\n";

            public const string InitComponentsStart = "InitComponentsStart";
            public const string InitComponentsEnd = "InitComponentsEnd";
            public const string FieldsStart = "FieldsStart";
            public const string FieldsEnd = "FieldsEnd";
            public const string Field = "#field#";
            public const string FindField = "#findfield#";
            private const string UIItem = "#UIItem#";


            private string ViewTxt()
            {
                if (_type == ItemType.UIItem)
                {

                    return "\t\tprotected override void OnGet()\n" +
                     "\t\t{\n" +
                     "\t\t}\n" +
                    "\t\tpublic override void OnSet()\n" +
                    "\t\t{\n" +
                    "\t\t}\n";
                }
                if (_type == ItemType.MVCView)
                    return "\t\tprotected override void OnLoad()\n" +
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
            "\t\t}\n";
                return string.Empty;
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
                    //case ItemType.UIObject:
                    //    pa = typeof(UIObjectView);
                    //    break;
                    case ItemType.MVCView:
                        pa = typeof(IFramework.UI.MVC.UIView);
                        break;
                    default:
                        break;
                }
                string target = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            $"\tpublic class #UserSCRIPTNAME# : {pa.FullName} \n" +
            "\t{\n" +
             $"//{FieldsStart}\n" +
             $"{Field}\n" +
             $"//{FieldsEnd}\n" +

            "\t\tprotected override void InitComponents()\n" +
            "\t\t{\n" +
             $"\t\t//{InitComponentsStart}\n" +
            $"{FindField}\n" +
            $"\t\t//{InitComponentsEnd}\n" +
            "\t\t}\n" +
            UIItem +
            "\t}\n" +
            "}";
                return target.Replace(UIItem, ViewTxt());
            }

        }
    }
}
