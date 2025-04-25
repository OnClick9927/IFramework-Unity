/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static IFramework.EditorTools;
namespace IFramework
{
    [EditorWindowCache("TimerWatcher")]
    class TimerWatcher : EditorWindow
    {
        private class Tree : TreeView
        {
            public Tree(TreeViewState state) : base(state)
            {
                this.showAlternatingRowBackgrounds = true;
                this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(
                    new MultiColumnHeaderState.Column[]
                    {
                        new MultiColumnHeaderState.Column{ headerContent=new GUIContent("ID"),allowToggleVisibility=false,canSort=false,width=80,minWidth=80},
                        new MultiColumnHeaderState.Column{ headerContent=new GUIContent("Time"), allowToggleVisibility=false,canSort=false ,width=50,minWidth=50},
                        new MultiColumnHeaderState.Column{ allowToggleVisibility=false,canSort=false},

                    }
                    ));
                Reload();
                this.multiColumnHeader.ResizeToFit();
            }
            protected override bool CanMultiSelect(TreeViewItem item)
            {
                return false;
            }
            protected override void SingleClickedItem(int id)
            {
                _selected = contexts[id];

                base.SingleClickedItem(id);
            }
            protected override TreeViewItem BuildRoot()
            {
                return new TreeViewItem()
                {
                    depth = -1,
                    id = -1
                };
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                IList<TreeViewItem> rows = GetRows() ?? new List<TreeViewItem>();
                rows.Clear();
                for (int i = 0; i < contexts.Count; i++)
                {
                    var context = contexts[i];
                    var item = new TreeViewItem()
                    {
                        depth = 0,
                        id = i,
                        parent = root,
                        displayName = $"{context.context.id}->{GetName(context.context)}"
                    };
                    rows.Add(item);
                }
                SetupDepthsFromParentsAndChildren(root);
                return rows;
            }
            protected override void RowGUI(RowGUIArgs args)
            {
                if (args.item.id < 0 || args.item.id >= contexts.Count)
                    return;
                var data = contexts[args.item.id];
                GUI.Label(args.GetCellRect(0), data.context.id);
                GUI.Label(args.GetCellRect(1), data.time.ToString("0.00"));
                GUI.Label(args.GetCellRect(2), GetName(data.context));

            }
        }

        class Temp
        {
            public ITimerContext context;
            public string stack;
            public float time;
        }
        static List<Temp> contexts = new List<Temp>();
        [InitializeOnLoadMethod]
        public static void GG()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnModeChange;
            UnityEditor.EditorApplication.playModeStateChanged += OnModeChange;
            TimerScheduler.onContextAllocate += Tween_onContextAllocate;
            TimerScheduler.onContextRecycle += Tween_onContextRecycle;
            _selected = null;
        }



        private static Temp _selected;
        private SplitView split = new SplitView()
        {

            split = 200,

        };
        private static Tree tree;
        TreeViewState state = new TreeViewState();
        private static void OnModeChange(UnityEditor.PlayModeStateChange mode)
        {
            if (mode == PlayModeStateChange.ExitingEditMode || mode == PlayModeStateChange.ExitingPlayMode)
            {
                contexts.Clear();
                _selected = null;
            }

        }
        static void ReloadWindow()
        {
            EditorApplication.delayCall += () =>
            {
                tree?.Reload();

            };
        }
        private static void Tween_onContextRecycle(ITimerContext obj)
        {
            if (_selected != null && _selected.context == obj)
            {
                _selected = null;
            }

            contexts.RemoveAll(x => x.context == obj);
            ReloadWindow();
        }



        private static void Tween_onContextAllocate(ITimerContext obj)
        {
            string trackStr = new System.Diagnostics.StackTrace(3, true).AddHyperLink();
            contexts.Add(new Temp()
            {
                context = obj,
                stack = trackStr,
                time = Time.time,
            });
            ReloadWindow();
        }

        private void OnEnable()
        {
            tree = new Tree(state);
        }
        private void OnDisable()
        {
            tree = null;
        }


        public static object DrawDefaultInspector(object obj)
        {
            var type = obj.GetType();
            MemberInfo[] fieldInfos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance).Where(x => x is FieldInfo || x is PropertyInfo).ToArray();

            GUILayout.BeginVertical();
            foreach (var field in fieldInfos)
            {
                FieldDefaultInspector(field, obj);
            }
            GUILayout.EndVertical();
            return obj;
        }
        private Vector2 scroll, scroll2;

        private static string GetName(ITimerContext context)
        {
            var type = context.GetType();
            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                return $"{nameof(ITimerContext)}<{args[0].Name},{args[1].Name}>";
            }
            else
            {
                return type.Name;
            }
        }
        private void OnGUI()
        {


            if (_selected != null)
            {
                split.OnGUI(new Rect(Vector2.zero, position.size));
                tree.OnGUI(split.rects[0]);

                var rect = split.rects[1];
                GUILayout.BeginArea(rect);

                var context = _selected;

                GUILayout.BeginHorizontal(GUILayout.Height(30));
                GUILayout.Label(GetName(context.context), EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("x", GUILayout.Width(30)))
                {
                    _selected = null;
                }
                GUILayout.EndHorizontal();
                scroll = GUILayout.BeginScrollView(scroll);
                GUI.enabled = false;

                DrawDefaultInspector(context.context);
                GUI.enabled = true;

                GUILayout.EndScrollView();




                GUILayout.Space(10);
                scroll2 = GUILayout.BeginScrollView(scroll2, EditorStyles.helpBox, GUILayout.Height(150));
                EditorTools.DrawStackTrace(context.stack);

                GUILayout.EndScrollView();


                GUILayout.EndArea();
            }
            else
            {
                tree.OnGUI(new Rect(Vector2.zero, position.size));

            }
        }
        private void OnInspectorUpdate()
        {
            Repaint();

        }

    }
}
