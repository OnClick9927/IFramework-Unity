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
using static IFramework.UI.UIEditorPrefs;
using static IFramework.EditorTools.ScriptCreator;

namespace IFramework.UI
{
    class UIGenCodeCS : UIGenCode
    {



        private UIGenCodeCS_PUB pubsave => UIEditorPrefs.context.pubsave;


        public override string name => "CS";
        protected override string viewName => _type == ViewType.View ? base.viewName : PanelToWidgetName(base.panelName);

        protected override string GetScriptFileName(string viewName) => $"{viewName}.cs";
        private string PanelToWidgetName(string panelName) => $"{panelName}Widget";

        [SerializeField] private ViewType _type;


        //string old_version_widget_file_path;

        List<Type> viewTypes, widgetTypes;
        List<string> viewTypes_str, widgetTypes_str;

        protected override void BeforeSetViewData()
        {
            //old_version_widget_file_path = string.Empty;
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
        protected override void OnFindDirSuccess()
        {
            var lines = File.ReadAllLines(scriptPath);
            var flag_1 = $"namespace {EditorTools.ProjectConfig.NameSpace}";
            var flag_2 = $"public class {viewName}";
            foreach (var line in lines)
            {
                if (line.StartsWith(flag_1))
                {
                    var temp = line.Replace(flag_1, "");
                    if (temp.StartsWith("."))
                        temp = temp.Substring(1);
                    pubsave.NameSpace = temp;
                }
                if (line.Trim().StartsWith(flag_2))
                {
                    if (_type == ViewType.View)
                    {
                        for (int i = 0; i < viewTypes_str.Count; i++)
                            if (line.Contains(viewTypes[i].Name))
                                pubsave.viewBaseIndex = i;

                    }
                    else
                    {
                        for (int i = 0; i < widgetTypes_str.Count; i++)
                            if (line.Contains(widgetTypes[i].Name))
                                pubsave.widgetBaseIndex = i;
                    }
                    break;
                }

            }


        }
        protected override void OnFindDirFail()
        {
            //if (_type == ViewType.Widget)
            //{
            //    old_version_widget_file_path = AssetDatabase.GetAllAssetPaths().ToList().Find(x => x.EndsWith(GetScriptFileName(base.viewName)));
            //}
        }
        public override void OnEnable()
        {
            FindBase();
            base.OnEnable();
        }
        private void FindBase()
        {
            viewTypes = typeof(UIView).GetSubTypesInAssemblies().Where(x => x.IsAbstract).ToList();
            widgetTypes = typeof(WidgetView).GetSubTypesInAssemblies().Where(x => x.IsAbstract && !x.IsSubclassOf(typeof(UIView)) && x != typeof(UIView)).ToList();

            viewTypes.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.FullName, y.FullName));
            widgetTypes.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.FullName, y.FullName));

            viewTypes.Insert(0, typeof(UIView));
            widgetTypes.Insert(0, typeof(WidgetView));
            viewTypes_str = viewTypes.ConvertAll(x => x.FullName);
            widgetTypes_str = widgetTypes.ConvertAll(x => x.FullName);

        }

        protected override void LoadLastData(UIGenCode _last)
        {
            var last = _last as UIGenCodeCS;
            this._type = last._type;
            //pubsave = EditorTools.GetFromPrefs(typeof(UIGenCodeCS_PUB), name, false) as UIGenCodeCS_PUB;
            //if (pubsave == null) pubsave = new UIGenCodeCS_PUB();
        }
        public override void OnDisable()
        {
            base.OnDisable();
            //if (pubsave == null) return;
            UIEditorPrefs.Save();
            //EditorTools.SaveToPrefs(pubsave, name, false);
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
            File.WriteAllText(GetScriptFilePath(scriptGenPath, scriptName), sb.ToString().ToUnixLineEndings());
            AssetDatabase.Refresh();
        }

        //private void Fix()
        //{
        //    string path = old_version_widget_file_path;
        //    var old_name = Path.GetFileNameWithoutExtension(path);
        //    var txt = File.ReadAllText(path);
        //    txt = txt.Replace(old_name, viewName);
        //    File.WriteAllText(path.Replace(old_name, viewName), txt);
        //    File.Delete(path);
        //    AssetDatabase.Refresh();
        //}



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
            //GUI.enabled = !string.IsNullOrEmpty(old_version_widget_file_path);
            //if (GUILayout.Button("Fix Widget", GUILayout.Width(80)))
            //{
            //    Fix();
            //}
            //GUI.enabled = true;

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
            if (_type == ViewType.Widget)
                return Resources.Load<TextAsset>("Widget").text.Replace(ParentClass, widgetTypes[pubsave.widgetBaseIndex].FullName);
            return Resources.Load<TextAsset>("View").text.Replace(ParentClass, viewTypes[pubsave.viewBaseIndex].FullName);

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


        public override string GetScriptFitter() => "t:Script";
    }

}
