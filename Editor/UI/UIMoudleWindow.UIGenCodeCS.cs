﻿/*********************************************************************************
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
                UI
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
                    _type = ItemType.UI;
                }
                else
                {
                    if (_type == ItemType.UI)
                        _type = ItemType.GameObject;
                }
            }

            protected override void OnFindDirSuccess()
            {
                string txt = File.ReadAllText(scriptPath);
                if (txt.Contains($"{typeof(IFramework.UI.GameObjectView).FullName}"))
                    _type = ItemType.GameObject;
                if (txt.Contains($"{typeof(IFramework.UI.UIItemView).FullName}"))
                    _type = ItemType.UIItem;
                if (txt.Contains($"{typeof(IFramework.UI.MVC.UIView).FullName}"))
                    _type = ItemType.UI;
            }

            protected override void LoadLastData(UIGenCode<GameObject> _last)
            {
                var last = _last as UIGenCodeCS;
                this._type = last._type;
            }
            public override void GenPanelNames(PanelCollection collect, string scriptGenPath, string scriptName)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"public class {scriptName}");
                sb.AppendLine("{");
                var datas = collect.datas;
                foreach (var data in datas)
                {
                    sb.AppendLine($"\tpublic const string {data.name} = \"{data.path}\";");
                }
                sb.AppendLine("\tpublic static System.Collections.Generic.Dictionary<string, System.Type> map = new System.Collections.Generic.Dictionary<string, System.Type>()\n\t{");
                foreach (var data in datas)
                {
                    if (!System.IO.File.Exists(data.ScriptPath)) continue;
                    var lines = File.ReadAllLines(data.ScriptPath);
                    var line = lines.FirstOrDefault(x => x.Contains("namespace "));
                    var viewName = this.PanelToViewName(data.name);
                    if (line == null)
                    {
                        sb.AppendLine($"\t\t{{{data.name},typeof({viewName})}},");
                    }
                    else
                    {
                        line = line.Trim();
                        var ns = line.Split(" ")[1].Trim();
                        sb.AppendLine($"\t\t{{{data.name},typeof({ns}.{viewName})}},");
                    }
                }
                sb.AppendLine("\t};");
                sb.AppendLine("}");
                File.WriteAllText(scriptGenPath.CombinePath($"{scriptName}.cs"), sb.ToString().ToUnixLineEndings());
                AssetDatabase.Refresh();
            }
            protected override void Draw()
            {
                _type = (ItemType)EditorGUILayout.EnumPopup("Type", _type);
            }

            protected override string GetScriptTemplate()
            {
                string MoreMethod = "#MoreMethod#";
                Type pa = null;
                switch (_type)
                {
                    case ItemType.UIItem:
                        pa = typeof(UIItemView);
                        break;
                    case ItemType.GameObject:
                        pa = typeof(GameObjectView);
                        break;
                    case ItemType.UI:
                        pa = typeof(IFramework.UI.MVC.UIView);
                        break;
                    default:
                        break;
                }
                string target = "/*********************************************************************************\n" +
            $" *Author:         {Author}\n" +
            $" *Version:        {Version}\n" +
            $" *UnityVersion:   {UnityVersion}\n" +
            $" *Date:           {Date}\n" +
            "*********************************************************************************/\n" +
                "using static IFramework.UI.UnityEventHelper;\r\n" +
            $"namespace {ScriptNameSpace}\n" +
            "{\n" +
            $"\tpublic class {ScriptName} : {pa.FullName} \n" +
            "\t{\n" +

            "\t\tclass View {\n" +
             $"//{FieldsStart}\n" +
             $"{Field}\n" +
             $"//{FieldsEnd}\n" +
             "\t\tpublic View(IFramework.UI.GameObjectView context){\n" +
             $"//{InitComponentsStart}\n" +
            $"{FindField}\n" +
            $"//{InitComponentsEnd}\n" +
             "\t\t\t}\n" +
             "\t\t}\n" +

             "\t\tprivate View view;\n" +

            "\t\tprotected override void InitComponents()\n" +
            "\t\t{\n" +
            "\t\t\tview = new View(this);\n" +



            "\t\t}\n" +


            MoreMethod +
            "\t}\n" +
            "}";
                return target.Replace(MoreMethod, ViewTxt());
            }

            protected override string GetFieldCode(string source, string fieldType, string fieldName)
            {
                if (source.Contains("class View"))
                    return $"\t\tpublic {fieldType} {fieldName};";
                else
                    return $"\t\tprivate {fieldType} {fieldName};";

            }
            protected override string GetFindFieldCode(string source, string fieldType, string fieldName, string path)
            {
                if (source.Contains("class View"))
                {
                    if (fieldType == typeof(GameObject).FullName)
                        return $"\t\t\t{fieldName} = context.GetGameObject({path});";
                    else if (fieldType == typeof(Transform).FullName)
                        return $"\t\t\t{fieldName} = context.GetTransform({path});";
                    else
                        return $"\t\t\t{fieldName} = context.GetComponent<{fieldType}>({path});";
                }
                else
                {

                    if (fieldType == typeof(GameObject).FullName)
                        return $"\t\t\t{fieldName} = GetGameObject({path});";
                    else if (fieldType == typeof(Transform).FullName)
                        return $"\t\t\t{fieldName} = GetTransform({path});";
                    else
                        return $"\t\t\t{fieldName} = GetComponent<{fieldType}>({path});";
                }
            }







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
                if (_type == ItemType.UI)
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

            public override string GetScriptFitter() => "t:Script";
        }
    }
}