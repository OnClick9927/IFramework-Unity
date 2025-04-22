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
using UnityEditor.IMGUI.Controls;
using static IFramework.EditorTools;

#pragma warning disable
namespace IFramework
{
    partial class RootWindow
    {
        class WindowCollection : UserOptionTab
        {
            private class SelectTree : TreeView
            {
                private struct Index
                {
                    public int id;
                    public EditorTools.EditorWindowTool.Entity value;
                }
                private List<Index> _show;
                public SelectTree(TreeViewState state) : base(state)
                {
                    this.rowHeight = 22;
                    EditorWindowTool.windows.FindAll((w) => { return w.searchName.ToLower().Contains(search); }).ToArray();
                    _show = new List<Index>();
                    EditorWindowTool.windows
                        .ForEach((entity) =>
                        {
                            _show.Add(new Index() { value = entity, id = EditorWindowTool.windows.IndexOf(entity) });
                        });
                    showAlternatingRowBackgrounds = true;

                    Reload();
                }
                protected override void DoubleClickedItem(int id)
                {
                    var type = _show.Find((index) => { return index.id == id; }).value.type;
                    RootWindow.OpenWindow(type);
                }
                protected override TreeViewItem BuildRoot()
                {
                    var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
                    return root;
                }
                protected override void SearchChanged(string newSearch)
                {
                    _show.Clear();
                    EditorWindowTool.windows.FindAll((w) => { return w.searchName.ToLower().Contains(search); })
                        .ForEach((entity) =>
                        {
                            _show.Add(new Index() { value = entity, id = EditorWindowTool.windows.IndexOf(entity) });

                        });
                    Reload();
                }
                protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
                {
                    List<TreeViewItem> list = new List<TreeViewItem>();
                    for (int i = 0; i < _show.Count; i++)
                    {
                        list.Add(new TreeViewItem() { depth = 1, id = _show[i].id, displayName = _show[i].value.searchName });
                    }
                    return list;
                }
                protected override void RowGUI(RowGUIArgs args)
                {
                    var window = EditorWindowTool.windows[args.item.id];
                    if (!string.IsNullOrEmpty(window.type.Namespace) && window.type.Namespace.Contains("UnityEditor"))
                        GUI.Label(args.rowRect, new GUIContent(window.searchName, tx));
                    else
                        GUI.Label(args.rowRect, window.searchName);
                }

            }

            public static Texture tx = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
            private SelectTree _tree;

            public override string Name => "WindowCollection";

            public WindowCollection()
            {
                _tree = new SelectTree(new TreeViewState());
            }

            public override void OnGUI(Rect position)
            {
                position.position = Vector2.zero;
                position = EditorTools.RectEx.Zoom(position, TextAnchor.MiddleCenter, -5);
                _tree.searchString = search;
                _tree.OnGUI(position);
            }


        }
    }

}
