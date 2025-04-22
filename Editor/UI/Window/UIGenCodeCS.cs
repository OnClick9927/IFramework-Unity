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
using System.Collections.Generic;

namespace IFramework.UI
{
    class UIGenCodeCS : UIGenCode
    {
        [System.Serializable]
        class UIGenCodeCS_PUB
        {
            public string NameSpace;
            public int viewBaseIndex;
            public int widgetBaseIndex;
        }


        private UIGenCodeCS_PUB pubsave = new UIGenCodeCS_PUB();


        public override string name => "CS";
        protected override string viewName => _type == ViewType.View ? base.viewName : PanelToWidgetName(base.panelName);

        protected override string GetScriptFileName(string viewName) => $"{viewName}.cs";
        private string PanelToWidgetName(string panelName) => $"{panelName}Widget";

        [SerializeField] private ViewType _type;


        string old_version_widget_file_path;

        List<Type> viewTypes, widgetTypes;
        List<string> viewTypes_str, widgetTypes_str;

        protected override void BeforeSetViewData()
        {
            old_version_widget_file_path = string.Empty;
            if (panel == null)
            {

            }
            else
            {
                if (panel.GetComponent<UIPanel>() != null)
                {
                    _type = ViewType.View;
                }
                else
                    _type = ViewType.Widget;

            }
        }
        protected override void OnFindDirFail()
        {
            UIPanel find = panel.GetComponent<UIPanel>();
            if (_type == ViewType.Widget)
            {
                old_version_widget_file_path = AssetDatabase.GetAllAssetPaths().ToList().Find(x => x.EndsWith(GetScriptFileName(base.viewName)));
            }
        }
        public override void OnEnable()
        {
            base.OnEnable();
            FindBase();
        }
        private void FindBase()
        {
            viewTypes = typeof(UIView).GetSubTypesInAssemblies().Where(x => x.IsAbstract).ToList();
            widgetTypes = typeof(GameObjectView).GetSubTypesInAssemblies().Where(x => x.IsAbstract && !x.IsSubclassOf(typeof(UIView)) && x != typeof(UIView)).ToList();

            viewTypes.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.FullName, y.FullName));
            widgetTypes.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.FullName, y.FullName));

            viewTypes.Insert(0, typeof(UIView));
            widgetTypes.Insert(0, typeof(GameObjectView));
            viewTypes_str = viewTypes.ConvertAll(x => x.FullName);
            widgetTypes_str = widgetTypes.ConvertAll(x => x.FullName);

        }

        protected override void LoadLastData(UIGenCode _last)
        {
            var last = _last as UIGenCodeCS;
            this._type = last._type;
            pubsave = EditorTools.GetFromPrefs(typeof(UIGenCodeCS_PUB), name, false) as UIGenCodeCS_PUB;
            //if (pubsave == null) pubsave = new UIGenCodeCS_PUB();
        }
        public override void OnDisable()
        {
            base.OnDisable();
            //if (pubsave == null) return;
            EditorTools.SaveToPrefs(pubsave, name, false);
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
                var seg = ScriptPathCollection.GetSeg(data);

                if (!System.IO.File.Exists(seg.ScriptPath)) continue;
                var lines = File.ReadAllLines(seg.ScriptPath);
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

        private void Fix()
        {
            string path = old_version_widget_file_path;
            var old_name = Path.GetFileNameWithoutExtension(path);
            var txt = File.ReadAllText(path);
            txt = txt.Replace(old_name, viewName);
            File.WriteAllText(path.Replace(old_name, viewName), txt);
            File.Delete(path);
            AssetDatabase.Refresh();
        }



        protected override void Draw()
        {
            if (_type == ViewType.Widget)
                pubsave.widgetBaseIndex = EditorGUILayout.Popup("BaseType", pubsave.widgetBaseIndex, widgetTypes_str.ToArray());

            else
                pubsave.viewBaseIndex = EditorGUILayout.Popup("BaseType", pubsave.viewBaseIndex, viewTypes_str.ToArray());

            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent($"NameSpace\t\t  {EditorTools.ProjectConfig.NameSpace}.");
            var size = GUI.skin.label.CalcSize(content);
            GUILayout.Label(content, GUILayout.Width(size.x));
            pubsave.NameSpace = EditorGUILayout.TextField(pubsave.NameSpace);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            _type = (ViewType)EditorGUILayout.EnumPopup("Type", _type);
            GUI.enabled = !string.IsNullOrEmpty(old_version_widget_file_path);
            if (GUILayout.Button("Fix Widget", GUILayout.Width(80)))
            {
                Fix();
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();

        }
        protected override string GetNameSpace()
        {
            var ns = base.GetNameSpace();
            if (string.IsNullOrEmpty(pubsave.NameSpace))
                return ns;
            return $"{ns}.{pubsave.NameSpace}";
        }
        protected override string GetScriptTemplate()
        {
            string MoreMethod = "#MoreMethod#";
            Type pa = null;
            switch (_type)
            {
                case ViewType.Widget:
                    pa = widgetTypes[pubsave.widgetBaseIndex];
                    break;
                case ViewType.View:
                    pa = viewTypes[pubsave.viewBaseIndex];
                    break;
                default:
                    break;
            }
            string target = "/*********************************************************************************\n" +
        $" *Author:         {Author}\n" +
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
         $"\t\tpublic View({ScriptName} context){{\n" +
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
        protected override string GetFindPrefabCode(string source, string name, string fieldName)
        {
            if (source.Contains("class View"))
            {
                return $"\t\t\t{fieldName} = context.FindPrefab(\"{name}\");";
            }
            else
            {

                return $"\t\t\t{fieldName} = FindPrefab(\"{name}\");";
            }
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
            if (_type == ViewType.View)
                return "\t\tprotected override void OnLoad(){}\n" +
                        "\t\tprotected override void OnShow(){}\n" +
                        "\t\tprotected override void OnHide(){}\n" +
                        "\t\tprotected override void OnClose(){}\n" +
                        "\t\tprotected override void OnBecameInvisible(){}\n" +
                        "\t\tprotected override void OnBecameVisible(){}\n";
            ;
            return string.Empty;
        }

        public override string GetScriptFitter() => "t:Script";
    }

}
