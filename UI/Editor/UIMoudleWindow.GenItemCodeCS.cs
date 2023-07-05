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

namespace IFramework.UI
{
    public partial class UIMoudleWindow
    {
        public class GenItemCodeCS : UIGenCode<GameObject>
        {
            public enum ItemType
            {
                UIItem,
                GameObject,
                UIObject,
            }
            public override string name => "CS/Gen_item_Code_CS";
            protected string designScriptName { get { return $"{viewName}.Design.cs"; } }
            protected override string viewScriptName { get { return $"{viewName}.cs"; } }


            [SerializeField] private ItemType _type;
            protected override GameObject gameobject => panel;


            protected override void OnFindDirSuccess()
            {
                string txt = File.ReadAllText(UIdir.CombinePath(designScriptName));
                if (txt.Contains($": {typeof(UIObjectView)}"))
                    _type = ItemType.UIObject;
                if (txt.Contains($": {typeof(GameObjectView)}"))
                    _type = ItemType.GameObject;
                if (txt.Contains($": {typeof(UIItemView)}"))
                    _type = ItemType.UIItem;
            }

            protected override void LoadLastData()
            {
                var last = EditorTools.GetFromPrefs<GenItemCodeCS>(name);
                if (last != null)
                {
                    this.panel = last.gameobject;
                    this.UIdir = last.UIdir;
                    this.state = last.state;
                    this._type = last._type;
                }
            }
            protected override void Draw()
            {
                _type = (ItemType)EditorGUILayout.EnumPopup("Type", _type);
            }
          

            protected override void WriteView()
            {
                string designPath = UIdir.CombinePath(designScriptName);
                string path = UIdir.CombinePath(viewScriptName);

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
