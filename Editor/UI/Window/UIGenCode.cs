/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text;
using System;
using static IFramework.EditorTools;

namespace IFramework.UI
{
    public abstract class UIGenCode : UIModuleWindowTab
    {
        public abstract void GenPanelNames(PanelCollection collect, string scriptGenPath, string scriptName);
        public abstract string GetScriptFitter();
        protected abstract string GetScriptFileName(string viewName);

        [SerializeField] private string GenPath = "";
        [SerializeField] private string panelPath;

        protected virtual string viewName => PanelToViewName(panelName);
        protected string panelName => PanelCollection.Data.ValidName(panel.name.Replace("@sm", ""));

        protected string scriptFileName => GetScriptFileName(viewName);
        protected virtual string scriptPath { get { return GenPath.CombinePath(scriptFileName); } }
        public string PanelToViewName(string panelName) => $"{panelName}View";
        public string GetPanelScriptName(string panelName) => GetScriptFileName(PanelToViewName(panelName));



        private GameObject _panel;

        protected GameObject panel
        {
            get
            {
                if (_panel == null && !string.IsNullOrEmpty(panelPath) && File.Exists(panelPath))
                {
                    _panel = AssetDatabase.LoadAssetAtPath<GameObject>(panelPath);
                }
                return _panel;
            }
            set
            {
                if (_panel != value)
                {
                    if (value == null)
                    {
                        panelPath = string.Empty;
                        _panel = null;
                    }
                    else
                    {
                        var _path = AssetDatabase.GetAssetPath(value);
                        if (string.IsNullOrEmpty(_path)) return;
                        if (!_path.EndsWith(".prefab")) return;
                        _panel = value;
                        panelPath = _path;
                    }
                }


            }

        }
        public void SetGameObject(GameObject go, string scriptPath)
        {
            GenPath = Path.GetDirectoryName(scriptPath);
            this.panel = go;
            SetViewData();
            //GUI.changed = true;
        }


        [SerializeField] private TreeViewState state = new TreeViewState();
        [SerializeField] private ScriptCreatorFieldsDrawer.SearchType _searchType;
        private ScriptCreatorFieldsDrawer fields;
        private FolderField FloderField;
        private ScriptCreator creator = new ScriptCreator();



        public override void OnEnable()
        {
            var last = EditorTools.GetFromPrefs(this.GetType(), name) as UIGenCode;
            if (last != null)
            {
                this._searchType = last._searchType;
                if (File.Exists(last.panelPath))
                    this.panelPath = last.panelPath;
                if (Directory.Exists(last.GenPath))
                    this.GenPath = last.GenPath;
                this.state = last.state;
                LoadLastData(last);
            }
            this.FloderField = new FolderField(GenPath);

            fields = new ScriptCreatorFieldsDrawer(creator, state, _searchType);
            SetViewData();
        }
        protected abstract void OnFindDirFail();

        protected virtual void OnFindDirSuccess() { }
        protected abstract void LoadLastData(UIGenCode last);


        public override void OnDisable()
        {
            _searchType = fields.GetSearchType();
            EditorTools.SaveToPrefs(this, name);
        }
        private void SetViewData()
        {
            BeforeSetViewData();

            if (panel != null)
            {
                creator.SetGameObject(panel);
                FindDir();
            }
            else
            {
                creator.SetGameObject(null);
                FloderField.SetPath(string.Empty);
            }
            fields.Reload();

        }

        protected virtual void BeforeSetViewData()
        {

        }

        private void FindDir()
        {
            if (!string.IsNullOrEmpty(GenPath))
            {

                var target = Path.Combine(GenPath, scriptFileName);
                if (File.Exists(target))
                {
                    FloderField.SetPath(GenPath);
                    OnFindDirSuccess();
                    return;
                }

            }
            string find = AssetDatabase.GetAllAssetPaths().ToList().Find(x => x.EndsWith(scriptFileName));
            if (string.IsNullOrEmpty(find))
            {
                OnFindDirFail();
                //FloderField.SetPath(string.Empty);
            }
            else
            {
                FloderField.SetPath(find.Replace(scriptFileName, "").ToAssetsPath());
                GenPath = FloderField.path;
                OnFindDirSuccess();
            }
        }


        protected virtual void Draw() { }
        public override void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                GUILayout.Label("Editor is Compiling");
                GUILayout.Label("please wait");
                return;
            }
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Panel Directory", EditorStyles.label, GUILayout.Width(150));
                FloderField.OnGUI(EditorGUILayout.GetControlRect());
                GenPath = FloderField.path;
                GUILayout.EndHorizontal();
            }
            Draw();
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            panel = EditorGUILayout.ObjectField("GameObject", panel, typeof(GameObject), false) as GameObject;
            if (GUILayout.Button("Open", GUILayout.Width(60)))
            {
                AssetDatabase.OpenAsset(panel);
            }
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                SetViewData();
            }
            if (panel && !string.IsNullOrEmpty(GenPath))
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Script", scriptPath);
                GUILayout.Space(5);

                GUI.enabled = File.Exists(scriptPath);
                if (GUILayout.Button("Edit", GUILayout.Width(60)))
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(scriptPath, 10);

                if (GUILayout.Button("Ping", GUILayout.Width(60)))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scriptPath));
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            fields.OnGUI();
            GUILayout.Space(5);
            if (GUILayout.Button("Gen"))
            {
                if (panel == null)
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Select UI Panel"));
                    return;
                }
                if (string.IsNullOrEmpty(GenPath))
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Map Gen Dir "));
                    return;
                }
                WriteView();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(5);

        }




        public const string ScriptName = "#ScriptName#";
        public const string ScriptNameSpace = "#ScriptNameSpace#";


        //public const string Version = "#UserVERSION#";
        //public const string UnityVersion = "#UserUNITYVERSION#";
        public const string Date = "#Date#";


        public const string Author = "#Author#";
        public const string InitComponentsStart = "InitComponentsStart";
        public const string InitComponentsEnd = "InitComponentsEnd";
        public const string FieldsStart = "FieldsStart";
        public const string FieldsEnd = "FieldsEnd";
        public const string Field = "#field#";
        public const string FindField = "#findfield#";

        protected abstract string GetFindPrefabCode(string source, string name, string fieldName);

        protected abstract string GetFieldCode(string source, string fieldType, string fieldName);
        protected abstract string GetFindFieldCode(string source, string fieldType, string fieldName, string path);

        private void Fields(string source, ScriptCreator creater, out string field, out string find)
        {
            var marks = creater.GetMarks();
            //if (creater.executeSubContext)
            //    marks = creater.GetAllMarks();

            StringBuilder sb_field = new StringBuilder();
            StringBuilder sb_find = new StringBuilder();
            if (marks != null)
            {
                string root_path = creater.rootPath;

                for (int i = 0; i < marks.Count; i++)
                {
                    var mark = marks[i];

                    string fieldType = mark.fieldType;
                    string fieldName = mark.fieldName;

                    string path = mark.gameObject.transform.GetPath();
                    if (path == root_path)
                        path = string.Empty;
                    else
                        path = path.Remove(0, root_path.Length + 1);
                    path = $"\"{path}\"";
                    sb_field.AppendLine(GetFieldCode(source, fieldType, fieldName));
                    sb_find.AppendLine(GetFindFieldCode(source, fieldType, fieldName, path));
                }
            }

            var prefabs = creater.GetPrefabs();
            for (int i = 0; i < prefabs.Count; i++)
            {
                string prefabs_name = prefabs[i].name;
                string fieldName = $"Prefab_{prefabs_name}";
                sb_field.AppendLine(GetFieldCode(source, typeof(GameObject).FullName, fieldName));
                sb_find.AppendLine(GetFindPrefabCode(source, prefabs_name, fieldName));

            }


            field = sb_field.ToString();
            find = sb_find.ToString();
        }

        protected virtual string OverwriteWriteFile(ScriptCreator context, string source)
        {
            return source;
        }

        protected virtual string GetNameSpace() => EditorTools.ProjectConfig.NameSpace;

        protected void WriteView()
        {
            creator.RemoveEmptyMarks();
            var file = ReadFromFile(scriptPath);
            var source = string.IsNullOrEmpty(file) ? GetScriptTemplate() : file;
            string field;
            string find;
            Fields(source, creator, out field, out find);
            source = source.Replace(Author, EditorTools.ProjectConfig.UserName)
            .Replace(ScriptName, Path.GetFileNameWithoutExtension(scriptPath))
              .Replace(ScriptNameSpace, GetNameSpace())
              .Replace(Date, DateTime.Now.ToString("yyyy-MM-dd")).Replace(Field, field)
                   .Replace(FindField, find);
            if (string.IsNullOrEmpty(file))
                source = source.ToUnixLineEndings();
            File.WriteAllText(scriptPath, OverwriteWriteFile(creator, source));
        }
        protected abstract string GetScriptTemplate();
        private string ReadFromFile(string path)
        {
            if (!File.Exists(path)) return string.Empty;
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
            return sb.ToString();
        }

    }
}
