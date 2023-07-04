/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using UnityEditor;

namespace IFramework.UI
{
    public partial class UIMoudleWindow
    {
        [System.Serializable]
        class MenuTree
        {
            private class InnerTree : TreeView
            {
                public event Action<string> onCurrentChange;

                private IList<string> _paths;
                private class Item : TreeViewItem
                {
                    public string path;
                }
                private List<Item> _items;

                public void ReadTree(List<string> paths, bool sort = true)
                {
                    if (sort) paths.Sort();
                    _paths = paths;
                    this.Reload();
                }
                public InnerTree(TreeViewState state) : base(state)
                {
                    this.rowHeight = 30;
                    //showAlternatingRowBackgrounds = true;
                }

                protected override TreeViewItem BuildRoot()
                {
                    var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
                    return root;
                }
                private string ToString(string[] strs, int count)
                {
                    string tmp = "";
                    for (int i = 0; i < count; i++)
                    {
                        tmp += "/" + strs[i];
                    }
                    return tmp.Substring(1);
                }
                protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
                {
                    if (_paths == null) return new List<TreeViewItem>();
                    _items = new List<Item>();
                    for (int i = 0; i < _paths.Count; i++)
                    {
                        string path = _paths[i];
                        var items = path.Split('/');
                        string last = items.Last();
                        bool ok = true;
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            if (!last.ToLower().Contains(this.searchString.ToLower())) ok = false;
                        }
                        if (ok)
                        {
                            var list2 = new List<Item>();
                            int index = 0;
                            for (int j = 0; j < items.Length; j++)
                            {
                                int depth = j;
                                string name = items[j];
                                string __path = ToString(items, j + 1);
                                var exist = _items.Find((it) =>
                                {
                                    return it.displayName == name && it.depth == depth && it.path == __path;
                                });
                                if (exist == null)
                                {
                                    index++;
                                    exist = new Item() { id = $"{name}{depth}".GetHashCode(), depth = depth, displayName = name, path = __path };
                                }
                                list2.Add(exist);
                            }
                            for (int j = 0; j < list2.Count; j++)
                            {
                                Item item = list2[j];
                                Item next = null;
                                if (j + 1 < list2.Count)
                                {
                                    next = list2[j + 1];
                                }
                                if (!_items.Contains(item))
                                {
                                    _items.Add(item);
                                }
                                if (next == null)
                                {
                                    item.children = next == null ? null : CreateChildListForCollapsedParent();
                                    break;
                                }
                                else
                                {
                                    item.AddChild(next);
                                }
                            }
                        }
                    }
                    return _items.ConvertAll(it =>
                    {
                        return new TreeViewItem()
                        {
                            id = it.id,
                            depth = it.depth,
                            displayName = it.displayName,
                            children = it.children,
                            icon = it.icon,
                            parent = it.parent
                        };
                    });
                }
                protected override bool CanChangeExpandedState(TreeViewItem item)
                {
                    return false;
                }
                protected override bool CanMultiSelect(TreeViewItem item)
                {
                    return false;
                }
                protected override void SelectionChanged(IList<int> selectedIds)
                {
                    var first = selectedIds.First();
                    string name = _items.Find(x => x.id == first).path;
                    onCurrentChange?.Invoke(name);
                    base.SelectionChanged(selectedIds);
                }
                protected override void SearchChanged(string newSearch)
                {
                    Reload();
                }

                public void Clear()
                {
                    _paths = null;
                    // _items.Clear();
                    Reload();
                }

                protected override void RowGUI(RowGUIArgs args)
                {
                    Rect rowRect = args.rowRect;
                    rowRect.y += rowRect.height;
                    rowRect.height = 1;
                    EditorGUI.DrawRect(rowRect, new Color(0.5f, 0.5f, 0.5f, 1));

                    var item = args.item;

                    Rect labelRect = EditorTools.RectEx.Zoom(args.rowRect, TextAnchor.MiddleCenter, -10);
                    if (hasSearch)
                    {
                        labelRect.x += depthIndentWidth;
                        labelRect.width -= labelRect.x;
                    }
                    else
                    {
                        labelRect.x += item.depth * depthIndentWidth + depthIndentWidth;
                        labelRect.width -= labelRect.x;
                    }
                    GUI.enabled = item.children == null || item.children.Count == 0;
                    GUI.Label(labelRect, item.displayName, "BoldLabel");
                    GUI.enabled = true;
                }
                public void Select(string path)
                {
                    var fit = _items.Find((item) => { return item.path == path; });
                    if (fit != null)
                    {
                        this.SetSelection(new List<int>()
                    {
                        fit.id
                    }, TreeViewSelectionOptions.FireSelectionChanged);
                    }
                    else
                    {
                        this.SetSelection(new List<int>());
                    }
                }
            }

            [SerializeField] private string search = "";
            [SerializeField] private TreeViewState State = new TreeViewState();
            public event Action<string> onCurrentChange;

            private UnityEditor.IMGUI.Controls.SearchField _searchField;
            private UnityEditor.IMGUI.Controls.SearchField _searchField2
            {
                get
                {
                    if (_searchField == null)
                    {
                        _searchField = new UnityEditor.IMGUI.Controls.SearchField();

                    }
                    return _searchField;
                }
            }
            private InnerTree _tree;
            private InnerTree tree
            {
                get
                {
                    if (_tree == null)
                    {
                        _tree = new InnerTree(State);
                        _tree.onCurrentChange += _tree_onCurrentChange;
                    }
                    return _tree;
                }
            }

            private void _tree_onCurrentChange(string obj)
            {
                onCurrentChange?.Invoke(obj);
            }

            public void ReadTree(List<string> paths, bool sort = true)
            {
                tree.ReadTree(paths, sort);
            }

            public void OnGUI(Rect position)
            {
                var rs = EditorTools.RectEx.HorizontalSplit(position, 20);
                search = _searchField2.OnGUI(rs[0], search);
                tree.searchString = search;
                tree.OnGUI(rs[1]);
            }


            public void Select(string path)
            {
                tree.Select(path);
            }

        }

    }
}
