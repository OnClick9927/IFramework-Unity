/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.51
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static IFramework.EditorTools;
using static IFramework.EditorTools.ScriptCreator;

#pragma warning disable
namespace IFramework
{

    partial class RootWindow
    {
        [System.Serializable]
        class GameObjectViewTab : UserOptionTab
        {
            [SerializeField] private TreeViewState state = new TreeViewState();
            [SerializeField] private ScriptCreatorFieldsDrawer.SearchType _searchType;
            [SerializeField] private string GenPath;
            [SerializeField] private string panelPath;
            [SerializeField] private string NameSpace;

            protected string GetScriptFileName(string viewName) => $"{viewName}.cs";
            protected virtual string viewName => PanelToViewName(panelName);
            protected string panelName => ScriptCreatorContext.ToValidFiledName(panel.name.Replace("@sm", ""));

            public string PanelToViewName(string panelName) => $"{panelName}View";
            protected virtual string scriptPath { get { return GenPath.CombinePath(scriptFileName); } }

            protected string scriptFileName => GetScriptFileName(viewName);

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

            private ScriptCreatorFieldsDrawer fields;
            private FolderField FloderField;
            private ScriptCreator creator = new ScriptCreator();

            public override void OnEnable()
            {
                FloderField = new FolderField();
                var last = EditorTools.GetFromPrefs<GameObjectViewTab>(nameof(GameObjectViewTab), true);
                if (last != null)
                {
                    _searchType = last._searchType;
                    state = last.state;
                    if (File.Exists(last.panelPath))
                        this.panelPath = last.panelPath;
                    if (Directory.Exists(last.GenPath))
                        this.GenPath = last.GenPath;
                    FloderField.SetPath(GenPath);
                    NameSpace = last.NameSpace;
                }
                fields = new ScriptCreatorFieldsDrawer(creator, state, _searchType);
                SetViewData();

            }
            public override void OnDisable()
            {
                EditorTools.SaveToPrefs<GameObjectViewTab>(this, nameof(GameObjectViewTab), true);
            }
            private void OnFindDirSuccess()
            {
                var lines = File.ReadAllLines(scriptPath);
                var flag_1 = $"namespace {EditorTools.ProjectConfig.NameSpace}";
                foreach (var line in lines)
                {
                    if (line.StartsWith(flag_1))
                    {
                        var temp = line.Replace(flag_1, "");
                        if (temp.StartsWith("."))
                            temp = temp.Substring(1);
                        NameSpace = temp;
                        break;
                    }

                }
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
                    //OnFindDirFail();
                    //FloderField.SetPath(string.Empty);
                }
                else
                {
                    FloderField.SetPath(find.Replace(scriptFileName, "").ToAssetsPath());
                    GenPath = FloderField.path;
                    OnFindDirSuccess();
                }
            }

            private void SetViewData()
            {
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

            public override string Name => "3、GenObjectView";
            public override void OnGUI(Rect position)
            {
                if (EditorApplication.isCompiling)
                {
                    GUILayout.Label("Editor is Compiling");
                    GUILayout.Label("please wait");
                    return;
                }
                position.position = Vector2.zero;
                GUILayout.BeginArea(position);
                GUILayout.BeginVertical();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Panel Directory", EditorStyles.label, GUILayout.Width(150));
                    FloderField.OnGUI(EditorGUILayout.GetControlRect());
                    GenPath = FloderField.path;
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUIContent content = new GUIContent($"NameSpace\t\t  {EditorTools.ProjectConfig.NameSpace}.");
                var size = GUI.skin.label.CalcSize(content);
                GUILayout.Label(content, GUILayout.Width(size.x));
                NameSpace = EditorGUILayout.TextField(NameSpace);
                GUILayout.EndHorizontal();


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
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Gen Dir "));
                        return;
                    }
                    EditorApplication.delayCall += () =>
                    {

                        WriteView();

                        AssetDatabase.Refresh();
                    };
                }

                GUILayout.Space(5);
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

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
                  .Replace(ScriptNameSpace, EditorTools.ProjectConfig.NameSpace + (string.IsNullOrEmpty(NameSpace) ? "" : $".{NameSpace}"))
                  .Replace(Date, DateTime.Now.ToString("yyyy-MM-dd")).Replace(Field, field)
                       .Replace(FindField, find);
                if (string.IsNullOrEmpty(file))
                    source = source.ToUnixLineEndings();
                File.WriteAllText(scriptPath, source);
            }
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
            protected string GetFieldCode(string source, string fieldType, string fieldName)
            {
                if (source.Contains("class View"))
                    return $"\t\tpublic {fieldType} {fieldName};";
                else
                    return $"\t\tprivate {fieldType} {fieldName};";

            }
            protected string GetFindPrefabCode(string source, string name, string fieldName)
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

            protected string GetFindFieldCode(string source, string fieldType, string fieldName, string path)
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

            protected string GetScriptTemplate() => Resources.Load<TextAsset>("GameObjectView").text;





      

        }

    }

}
