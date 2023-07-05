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

namespace IFramework.UI
{
    public partial class UIMoudleWindow
    {
        public abstract class UIGenCode<T> : UIMoudleWindowTab where T : UnityEngine.Object
        {
            [SerializeField] protected string UIdir = "";
            [SerializeField] protected T panel;

            [SerializeField] protected TreeViewState state = new TreeViewState();
            private ScriptCreaterFieldsDrawer fields;
            private FloderField FloderField;
            protected abstract GameObject gameobject { get; }

            protected string panelName { get { return panel.name; } }
            protected string viewName { get { return $"{panelName}View"; } }
            protected abstract string viewScriptName { get; }

            protected ScriptCreater creater = new ScriptCreater();
            public override void OnEnable()
            {
                LoadLastData();
                this.FloderField = new FloderField(UIdir);
                fields = new ScriptCreaterFieldsDrawer(creater, state);
                SetViewData();
            }
            protected abstract void OnFindDirSuccess();
            protected abstract void LoadLastData();
            protected abstract void WriteView();
            public override void OnDisable()
            {
                EditorTools.SaveToPrefs(this, name);
            }
            private void SetViewData()
            {

                if (panel != null)
                {
                    creater.SetGameObject(gameobject);
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
                string find = AssetDatabase.GetAllAssetPaths().ToList().Find(x => x.EndsWith(viewScriptName));
                if (string.IsNullOrEmpty(find))
                {
                    FloderField.SetPath(string.Empty);
                }
                else
                {
                    FloderField.SetPath(find.Replace(viewScriptName, "").ToAssetsPath());
                    UIdir = FloderField.path;
                    OnFindDirSuccess();
                }
            }

            public override void OnHierarchyChanged()
            {
                creater.ColllectMarks();
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
                Draw();
                EditorGUI.BeginChangeCheck();
                panel = EditorGUILayout.ObjectField("GameObject", panel, typeof(T), false) as T;
                if (EditorGUI.EndChangeCheck())
                {
                    SetViewData();
                }
                GUILayout.Space(10);
                fields.OnGUI();
            }
        }
    }
}
