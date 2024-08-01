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
    public partial class UIModuleWindow
    {
        public class UIGenCodeCS : UIGenCode<GameObject>
        {
            public enum ItemType
            {
                UIItem,
                GameObject,
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
            public override void GenPanelNames(PanelCollection collect, string scriptGenPath, string scriptName)
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
                    case ItemType.MVCView:
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
             $"//{FieldsStart}\n" +
             $"{Field}\n" +
             $"//{FieldsEnd}\n" +

            "\t\tprotected override void InitComponents()\n" +
            "\t\t{\n" +
             $"\t\t//{InitComponentsStart}\n" +
            $"{FindField}\n" +
            $"\t\t//{InitComponentsEnd}\n" +
            "\t\t}\n" +
            MoreMethod +
            "\t}\n" +
            "}";
                return target.Replace(MoreMethod, ViewTxt());
            }

            protected override string GetFieldCode(string fieldType, string fieldName)
            {
                return $"\t\tprivate {fieldType} {fieldName};";

            }
            protected override string GetFindFieldCode(string fieldType, string fieldName, string path)
            {
                if (fieldType == typeof(GameObject).FullName)
                    return $"\t\t\t{fieldName} = GetGameObject({path});";
                else if (fieldType == typeof(Transform).FullName)
                    return $"\t\t\t{fieldName} = GetTransform({path});";
                else
                    return $"\t\t\t{fieldName} = GetComponent<{fieldType}>({path});";
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


        }
    }
}
