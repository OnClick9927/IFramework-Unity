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

namespace IFramework.UI
{
    public partial class UIModuleWindow
    {
        public abstract class UIGenCode : UIModuleWindowTab
        {
            public abstract void GenPanelNames(PanelPathCollect collect, string scriptGenPath, string scriptName);
        }
        public abstract class UIGenCode<T> : UIGenCode where T : UnityEngine.Object
        {
            [SerializeField] protected string GenPath = "";
            [SerializeField] protected T panel;

            [SerializeField] protected TreeViewState state = new TreeViewState();
            [SerializeField] private ScriptCreatorFieldsDrawer.SearchType _searchType;
            private ScriptCreatorFieldsDrawer fields;
            private FolderField FloderField;
            protected abstract GameObject gameObject { get; }

            protected string panelName { get { return panel.name; } }
            private string PanelToViewName(string panelName) => $"{panelName}View";

            protected string viewName => PanelToViewName(panelName);
            public sealed override string GetPanelScriptName(string panelName) => GetViewScriptName(PanelToViewName(panelName));
            protected abstract string GetViewScriptName(string viewName);
            protected string viewScriptName => GetViewScriptName(viewName);
            protected virtual string scriptPath { get { return GenPath.CombinePath(viewScriptName); } }


            protected ScriptCreator creator = new ScriptCreator();
            public override void OnEnable()
            {
                var last = EditorTools.GetFromPrefs(this.GetType(), name) as UIGenCode<T>;
                if (last != null)
                {
                    this._searchType = last._searchType;
                    this.panel = last.panel;
                    var path = AssetDatabase.GetAssetPath(this.gameObject);
                    if (!path.EndsWith(".prefab"))
                        this.panel = null;
                    this.GenPath = last.GenPath;
                    this.state = last.state;
                    LoadLastData(last);
                }
                this.FloderField = new FolderField(GenPath);

                fields = new ScriptCreatorFieldsDrawer(creator, state, _searchType);
                SetViewData();
            }
            protected abstract void OnFindDirSuccess();
            protected abstract void LoadLastData(UIGenCode<T> last);
            protected abstract void WriteView();
            public override void OnDisable()
            {
                _searchType = fields.GetSearchType();
                EditorTools.SaveToPrefs(this, name);
            }
            private void SetViewData()
            {

                if (panel != null)
                {
                    creator.SetGameObject(gameObject);
                    FindDir();
                }
                else
                {
                    creator.SetGameObject(null);
                    FloderField.SetPath(string.Empty);
                }
            }
            private void FindDir()
            {
                string find = AssetDatabase.GetAllAssetPaths().ToList().Find(x => x.EndsWith(viewScriptName));
                if (string.IsNullOrEmpty(find))
                {
                    FloderField.SetPath(string.Empty);
                }
                else
                {
                    FloderField.SetPath(find.Replace(viewScriptName, "").ToAssetsPath());
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
                    GUILayout.Label("Panel Directory", GUIStyles.toolbar);
                    GUILayout.Space(20);

                    FloderField.OnGUI(EditorGUILayout.GetControlRect());
                    GenPath = FloderField.path;
                    GUILayout.EndHorizontal();
                }
                Draw();
                EditorGUI.BeginChangeCheck();
                panel = EditorGUILayout.ObjectField("GameObject", panel, typeof(T), false) as T;
                if (EditorGUI.EndChangeCheck())
                {
                    SetViewData();
                }
                GUILayout.Space(5);
                fields.OnGUI();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Gen"))
                    {
                        if (gameObject == null)
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

                    GUI.enabled = gameObject && File.Exists(scriptPath);
                    if (GUILayout.Button("Edit Script"))
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(scriptPath, 10);

                    if (GUILayout.Button("Ping Script"))
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scriptPath));
                    GUI.enabled = true;


                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

            }


        }
    }
}
