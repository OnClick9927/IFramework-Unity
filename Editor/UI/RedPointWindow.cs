/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-07-25
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using static IFramework.EditorTools;

namespace IFramework
{
    [EditorWindowCache("RedPoint")]
    class RedPointWindow : EditorWindow
    {
        private class ViewTree : TreeView
        {
            public RedPointWindow window;
            private List<RedPoint> root;
            private SearchField field = new SearchField();
            private string _ping;
            internal RedTreeService red;

            public ViewTree(TreeViewState state, MultiColumnHeaderState headerState) : base(state)
            {
                this.multiColumnHeader = new MultiColumnHeader(headerState);
                Reload();
                this.showAlternatingRowBackgrounds = true;
                this.showBorder = true;
                this.multiColumnHeader.ResizeToFit();
            }

            public void FreshView(List<RedPoint> root)
            {
                this.root = root;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {
                return new TreeViewItem()
                {
                    depth = -1,
                    id = -100
                };
            }

            private void LoopBuild(TreeViewItem parent, RedPoint point, IList<TreeViewItem> list)
            {



                if (string.IsNullOrEmpty(searchString))
                {

                    TreeViewItem item = new TreeViewItem()
                    {

                        depth = parent.depth + 1,
                        displayName = point.key,
                        parent = parent,
                        id = point.key.GetHashCode()

                    };
                    list.Add(item);
                    if (point.children != null && point.children.Count > 0)
                    {
                        if (!IsExpanded(item.id))
                        {

                            item.children = CreateChildListForCollapsedParent();
                        }
                        else
                            foreach (var child in point.children.Values)
                            {

                                LoopBuild(item, child, list);
                            }
                    }
                }
                else
                {

                    if (point.key.ToLower().Contains(searchString.ToLower()))
                    {

                        TreeViewItem item = new TreeViewItem()
                        {

                            depth = parent.depth + 1,
                            displayName = point.key,
                            parent = parent,
                            id = point.key.GetHashCode()

                        };
                        list.Add(item);
                    }


                    if (point.children != null && point.children.Count > 0)
                    {
                        foreach (var child in point.children.Values)
                        {

                            LoopBuild(parent, child, list);
                        }
                    }

                }

            }
            protected async override void DoubleClickedItem(int id)
            {
                if (string.IsNullOrEmpty(searchString)) return;
                var _ping = FindItem(id, rootItem).displayName;
                char separator = RedTreeService.separator;
                var columns = _ping.Split(separator);
                for (int j = 0; j < columns.Length; j++)
                {
                    var _pkey = string.Join(separator.ToString(), columns, 0, j);
                    var _key = string.Join(separator.ToString(), columns, 0, j + 1);
                    this.SetExpanded(_key.GetHashCode(), true);
                    this.SetExpanded(_pkey.GetHashCode(), true);
                }
                this._ping = _ping;

                this.FrameItem(id);
                this.searchString = string.Empty;
                Reload();
                this.FrameItem(id);
                await Task.Delay(1000);
                this._ping = null;
                Reload();
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                var list = this.GetRows() ?? new List<TreeViewItem>();

                list.Clear();

                if (this.root != null)
                {
                    for (int i = 0; i < this.root.Count; i++)
                    {
                        LoopBuild(root, this.root[i], list);
                    }
                }
                SetupParentsAndChildrenFromDepths(root, list);
                return list;
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                float indent = this.GetContentIndent(args.item);
                GUI.Label(EditorTools.RectEx.Zoom(args.GetCellRect(0), TextAnchor.MiddleRight, new Vector2(-indent, 0)), args.item.displayName);
                GUI.Label(args.GetCellRect(1), red.GetCount(args.item.displayName).ToString());
                GUI.Label(args.GetCellRect(2), red.GetDotCount(args.item.displayName).ToString());


                if (args.item.displayName == _ping)
                    GUI.Label(RectEx.Zoom(args.rowRect, TextAnchor.MiddleCenter, -8), "", "LightmapEditorSelectedHighlight");
            }

            public override void OnGUI(Rect rect)
            {
                var rs = EditorTools.RectEx.HorizontalSplit(rect, 20);

                var rss = RectEx.VerticalSplit(rs[0], rs[0].width - 100);
                var tmp = field.OnGUI(rss[0], searchString);
                if (GUI.Button(rss[1], "Fresh"))
                {
                    window.Fresh();
                }
                if (tmp != searchString)
                {
                    searchString = tmp;
                    Reload();
                }
                base.OnGUI(rs[1]);
            }
        }
        ViewTree _tree;
        TreeViewState state;
        MultiColumnHeaderState headerState;
        private void OnEnable()
        {
            if (state == null)
                state = new TreeViewState();
            if (headerState == null)
                headerState = new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] {

                    new MultiColumnHeaderState.Column()
                    {
                        headerContent=new GUIContent("Path"),
                        width=position.width-80,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent=new GUIContent("Count"),
                        width=80,
                        maxWidth=80,
                        minWidth=80,

                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent=new GUIContent("Dot"),
                        width=80,
                        maxWidth=80,
                        minWidth=80,

                    }
                });

            _tree = new ViewTree(state, headerState);
            _tree.window = this;
            //RedTree.SetViewer(_tree);
            Fresh();
        }

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying) return;
            var tree = Game.Current.GetService<RedTreeService>();
            if (tree == null) return;

            tree.OnFresh -= Tree_OnFresh;
            tree.OnFresh += Tree_OnFresh;


            _tree.OnGUI(new UnityEngine.Rect(Vector2.zero, this.position.size));
        }
        public void Fresh()
        {
            if (!EditorApplication.isPlaying) return;

            Tree_OnFresh(Game.Current.GetService<RedTreeService>());

        }
        private void Tree_OnFresh(RedTreeService tree)
        {
            if (tree == null) return;
            _tree.red = tree;

            _tree.FreshView(tree.key_map.Values.ToList().Where(x => string.IsNullOrEmpty(x.parent_key)).ToList());

        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
