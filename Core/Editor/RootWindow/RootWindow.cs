/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.51
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using IFramework;

#pragma warning disable
namespace IFramework
{
    partial class RootWindow : UnityEditor.EditorWindow
    {
        private MenuTree menu = new MenuTree();
        private SplitView _split;
        private UnityEditor.IMGUI.Controls.SearchField _searchField;
        private static string search = "";
        private Dictionary<string, UserOptionTab> tabs;
        private Dictionary<string, Type> windowtabs = new Dictionary<string, Type>();
        private string showkey = "";

        [MenuItem("Tools/IFramework %#i")]
        static void ShowWindow()
        {
            GetWindow<RootWindow>();
        }
        private void OnEnable()
        {

            this.titleContent = new GUIContent("RootWindow");
            this.minSize = new Vector2(700, 400);
            _searchField = new UnityEditor.IMGUI.Controls.SearchField();
            _split = new SplitView();
            _split.fistPan += menu.OnGUI;
            _split.secondPan += ContentGUI;
            var list = typeof(UserOptionTab).GetSubTypesInAssemblys().ToList();
            list.RemoveAll(t => t.IsAbstract);

            tabs = list.ConvertAll(t => Activator.CreateInstance(t) as UserOptionTab)
                        .ToDictionary(tab => tab.Name);
            var keys = tabs.Keys.ToList();
            keys.Sort();
            var _list = EditorTools.EditorWindowTool.user_windows;
            foreach (var item in _list)
            {
                Type type = item.type;
                string name = item.searchName;
                windowtabs.Add(name, type);
                keys.Add(name);
            }
            menu.ReadTree(keys, false);
            menu.onCurrentChange += (obj) =>
            {
                showkey = obj;
            };
            foreach (var item in tabs.Values)
            {
                item.OnEnable();
            }

        }


        private void OnDisable()
        {

            if (tabs != null)
            {
                foreach (var item in tabs.Values)
                {
                    item.OnDisable();
                }
            }

        }
        private void OnGUI()
        {
            var rs = EditorTools.RectEx.HorizontalSplit(new Rect(Vector2.zero, position.size), 20);
            Tool(rs[0]);
            var r2 = EditorTools.RectEx.Zoom(rs[1], TextAnchor.UpperCenter, -10);
            _split.OnGUI(r2);
        }

        private void Tool(Rect position)
        {
            GUILayout.BeginArea(position);
            GUILayout.BeginHorizontal("toolbar");
            GUILayout.Label("", GUILayout.Width(100));

            Rect r = GUILayoutUtility.GetLastRect();
            if (GUI.Button(r, "Tools", GUIStyles.ToolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Doc", Application.persistentDataPath);
                dic.Add("Streaming", Application.streamingAssetsPath);
                dic.Add("DataPath", Application.dataPath);
                dic.Add("Temporary", Application.temporaryCachePath);
#if UNITY_2018_1_OR_NEWER
                dic.Add("Console", Application.consoleLogPath);
#endif
                foreach (var item in dic)
                {
                    menu.AddItem(new GUIContent($"Open Folder/{item.Key}"), false, () =>
                    {
                        EditorTools.OpenFolder(item.Value);
                    });
                }
                menu.AddItem(new GUIContent("Github"), false, () => { Application.OpenURL("https://github.com/OnClick9927/IFramework-Unity"); });
                menu.AddItem(new GUIContent("Join us"), false, () => { Application.OpenURL("https://jq.qq.com/?_wv=1027&k=TTSfAM1P"); });
                menu.DropDown(r);
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("", GUILayout.Width(200));

            search = _searchField.OnGUI(GUILayoutUtility.GetLastRect(), search);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        private void ContentGUI(Rect position)
        {
            if (tabs == null && windowtabs == null) return;
            GUILayout.BeginArea(position);
            if (tabs.ContainsKey(showkey))
            {
                tabs[showkey].OnGUI(position);
            }
            else
            {
                if (!string.IsNullOrEmpty(showkey))
                {

                    position.position = Vector2.zero;

                    GUIStyle style = new GUIStyle(GUIStyles.button)
                    {
                        richText = true,
                    };
                    GUILayout.Space(10);
                    GUILayout.Label(showkey, new GUIStyle(GUIStyles.largeLabel)
                    {
                        alignment = UnityEngine.TextAnchor.MiddleCenter,
                        fontSize = 20,
                        fontStyle = FontStyle.Bold,
                    });
                    var pos = new Rect(0, 0, 300, 200);
                    pos.center = position.center;
                    var rs = EditorTools.RectEx.HorizontalSplit(pos, pos.height / 2, 10, false);
                    //string type_string = showkey.Replace("/", ".");
                    if (windowtabs.ContainsKey(showkey))
                    {
                        if (GUI.Button(rs[0], $"<size=20><b><color=#00AABB>Open</color></b></size>", style))
                        {
                            var type = windowtabs[showkey];
                            OpenWindow(type);
                        }
                        if (GUI.Button(rs[1], $"<size=20><b><color=#00AABB>Close</color></b></size>", style))
                        {
                            var type = windowtabs[showkey];
                            var find = EditorTools.EditorWindowTool.Find(type);
                            if (find != null)
                            {
                                find.Close();
                            }
                        }
                    }


                }
            }
            GUILayout.EndArea();

        }


        public static void OpenWindow(Type type)
        {
            if (EditorTools.ProjectConfig.dockWindow)
            {
                var m = typeof(EditorWindow).GetMethod(nameof(GetWindow), new Type[] { typeof(Type).MakeArrayType() });
                m = m.MakeGenericMethod(type);
                var _win = m.Invoke(null, new object[] { new Type[] { typeof(RootWindow) } }) as EditorWindow;
                _win.Focus();
            }
            else
            {
                EditorWindow.GetWindow(type);
            }

        }
    }

}
