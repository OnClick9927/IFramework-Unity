/*********************************************************************************
 *Author:         Wulala
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-09-13
 *Description:    Description
 *History:        2022-09-13--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static IFramework.UI.PanelPathCollect;

namespace IFramework.UI
{
    public partial class UIMoudleWindow
    {
        private TreeViewState layer_state = new TreeViewState();
        public class UILayerEdit : UIMoudleWindowTab
        {
            private class LayerView : TreeView
            {
                private List<Data> datas { get { return collect.datas; } }

                string[] layerNames;
                IList<int> draggedItemIDs;
                public LayerView(TreeViewState state) : base(state)
                {
                    layerNames = Enum.GetNames(typeof(UILayer));

                    this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] {
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Name"),
                            minWidth=200,

                        },
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Res"),
                            minWidth=50,
                            maxWidth=50,
                            width=50,
                        },
                          new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Order"),
                            minWidth=50,
                            maxWidth=50,

                        },
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Path"),
                            minWidth=300
                        },

                    }));

                    this.showAlternatingRowBackgrounds = true;
                    this.multiColumnHeader.ResizeToFit();
                    Reload();

                }

                protected override TreeViewItem BuildRoot()
                {
                    return new TreeViewItem() { depth = -1 };
                }
                protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
                {
                    var rows = GetRows() ?? new List<TreeViewItem>();
                    rows.Clear();
                    for (int i = 0; i < layerNames.Length; i++)
                    {
                        TreeViewItem layer = new TreeViewItem()
                        {
                            depth = 0,
                            displayName = layerNames[i],
                            id = i,
                        };
                        layer.parent = root;
                        rows.Add(layer);
                        var findList = datas.FindAll(x => x.layer == (UILayer)Enum.Parse(typeof(UILayer), layerNames[i]));
                        if (findList.Count > 0)
                        {
                            if (IsExpanded(i))
                            {
                                findList.Sort((x, y) => { return x.order > y.order ? 1 : -1; });
                                for (int j = 0; j < findList.Count; j++)
                                {
                                    var find = findList[j];
                                    var index = datas.IndexOf(find);
                                    TreeViewItem item = new TreeViewItem()
                                    {
                                        depth = 1,
                                        displayName = find.path,
                                        id = layerNames.Length + index,
                                    };
                                    item.parent = layer;
                                    rows.Add(item);
                                }
                            }
                            else
                            {
                                layer.children = CreateChildListForCollapsedParent();
                            }

                        }


                    }
                    SetupParentsAndChildrenFromDepths(root, rows);
                    return rows;
                }

                protected override void RowGUI(RowGUIArgs args)
                {
                    if (args.item.depth == 0)
                    {
                        base.RowGUI(args);
                        return;
                    }
                    float indent = this.GetContentIndent(args.item);
                    var data = datas[args.item.id - layerNames.Length];
                    GUI.Label(EditorTools.RectEx.Zoom(args.GetCellRect(0), TextAnchor.MiddleRight, new Vector2(-indent, 0)), data.name);

                    GUI.Toggle(args.GetCellRect(1), data.isResourcePath, "");
                    GUI.Label(args.GetCellRect(2), data.order.ToString());
                    GUI.Label(args.GetCellRect(3), data.path);

                }

                protected override bool CanBeParent(TreeViewItem item)
                {
                    if (item.depth == 1)
                        return false;
                    var s_list = this.GetSelection();
                    if (s_list.ToList().Find(x => x < layerNames.Length) != default)
                    {
                        return false;
                    }
                    return true;
                }
                protected override bool CanStartDrag(CanStartDragArgs args)
                {
                    if (args.draggedItemIDs.ToList().Find(x => x < layerNames.Length) != default)
                    {
                        return false;
                    }
                    return true;
                }


                private void Set(UILayer layer, int index, Data data)
                {
                    List<Data> last = datas.FindAll(x => x.layer == layer);
                    int lastCount = last.Count;

                    last.Sort((x, y) => { return x.order > y.order ? 1 : -1; });
                    if (data.layer == layer)
                    {
                        last.Remove(data);
                    }
                    data.layer = layer;
                    int realIndex = -1;
                    for (int i = 0; i < last.Count; i++)
                    {
                        if (last[i].order >= index)
                        {
                            realIndex = i;
                            break;
                        }
                    }
                    if (realIndex == -1)
                        realIndex = Mathf.Clamp(lastCount - 1, 0, lastCount - 1);
                    last.Insert(realIndex, data);
                    for (int i = 0; i < last.Count; i++)
                    {
                        last[i].order = i;
                    }
                }
                protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
                {
                    if (args.performDrop)
                    {
                        var id = args.parentItem.id;
                        UILayer layer = (UILayer)Enum.Parse(typeof(UILayer), layerNames[id]);
                        if (args.dragAndDropPosition != DragAndDropPosition.OutsideItems)
                        {
                            var last = datas.FindAll(x => x.layer == layer);
                            var index = args.insertAtIndex;
                            for (int i = 0; i < draggedItemIDs.Count; i++)
                            {
                                Set(layer, index, datas[draggedItemIDs[i] - layerNames.Length]);
                            }

                        }
                        Save();
                        Reload();
                    }
                    return DragAndDropVisualMode.Move;
                }
                protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
                {
                    draggedItemIDs = args.draggedItemIDs;
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.StartDrag("");
                    base.SetupDragAndDrop(args);
                }
                private void Tool(Rect rect)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Fresh"))
                        {
                            Collect();
                            Reload();
                        }
                        if (GUILayout.Button("Save To File"))
                        {
                            Save();
                        }
                        if (GUILayout.Button("Ping File"))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(UICollectPath));
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                public override void OnGUI(Rect rect)
                {
                    var rs = EditorTools.RectEx.HorizontalSplit(rect,rect.height - 20);
                    base.OnGUI(rs[0]);
                    Tool(rs[1]);
                }
                protected override void DoubleClickedItem(int id)
                {
                    var item = FindItem(id, rootItem);
                    if (item != null && item.depth == 1)
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(item.displayName));

                    base.DoubleClickedItem(id);
                }

            }
            public override string name => "BuildUILayer";
            LayerView view;
            public override void OnEnable()
            {
                view = new LayerView(window.layer_state);
            }
            public override void OnGUI()
            {
                GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                view.OnGUI(GUILayoutUtility.GetLastRect());
            }

        }
    }
}
