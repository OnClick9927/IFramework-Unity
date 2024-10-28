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
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static IFramework.EditorTools;
using static IFramework.UI.PanelCollection;
using static IFramework.UI.UIModuleWindow.UICollectData;

namespace IFramework.UI
{
    public partial class UIModuleWindow
    {
        [System.Serializable]
        public class UILayerEdit : UIModuleWindowTab
        {
            private static PanelCollection collect;
            private class LayerView : TreeView
            {
                private List<Data> datas { get { return collect.datas; } }
                private SearchField searchField;
                private UILayerEdit edit;
                string[] layerNames;
                IList<int> draggedItemIDs;
                public LayerView(UILayerEdit edit) : base(edit.layer_state)
                {
                    this.edit = edit;
                    layerNames = edit.layerObject.GetLayerNames().ToArray();
                    searchField = new SearchField();
                    this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] {
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Name"),
                            minWidth=200,
                            width=200,
                        },
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=EditorGUIUtility.IconContent("UnityEditor.GameView"),
                            minWidth=30,
                            maxWidth=30,
                            width=30
                        },
                          new MultiColumnHeaderState.Column()
                        {
                            headerContent=EditorGUIUtility.IconContent("Folder Icon"),
                            minWidth=30,
                            maxWidth=30,
                            width=30,
                        },
                               new MultiColumnHeaderState.Column()
                        {
                            headerContent=EditorGUIUtility.IconContent("Prefab Icon"),
                            minWidth=30,
                            maxWidth=30,
                            width=30,
                        },
                               new MultiColumnHeaderState.Column()
                        {
                            headerContent=EditorGUIUtility.IconContent("cs Script Icon"),
                            minWidth=30,
                            maxWidth=30,
                            width=30,
                        },
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Res"),
                            minWidth=30,
                            maxWidth=30,
                            width=30,
                        },

                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("FullScreen"),
                            width=100,
                                      minWidth=100,
                            maxWidth=100,
                        },
                             new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("ScriptType"),
                            width=100,
                                      minWidth=100,
                        },
                                new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Order"),
                            minWidth=40,
                            maxWidth=40,
                            width=40
                        },
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Path"),
                            minWidth=400
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
                        var findList = datas.FindAll(x => x.layer == edit.layerObject.LayerNameToIndex(layerNames[i]));
                        if (string.IsNullOrEmpty(searchString))
                        {
                            TreeViewItem layer = new TreeViewItem()
                            {
                                depth = 0,
                                displayName = layerNames[i],
                                id = i,
                            };
                            layer.parent = root;
                            rows.Add(layer);
                            if (findList.Count > 0)
                            {
                                if (IsExpanded(i))
                                {
                                    findList.Sort((x, y) => { return x.order >= y.order ? 1 : -1; });
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
                        else
                        {
                            if (findList.Count > 0)
                            {
                                findList.Sort((x, y) => { return x.order >= y.order ? 1 : -1; });
                                for (int j = 0; j < findList.Count; j++)
                                {
                                    var find = findList[j];
                                    if (!find.name.ToLower().Contains(searchString.ToLower()))
                                        continue;

                                    var index = datas.IndexOf(find);
                                    TreeViewItem item = new TreeViewItem()
                                    {
                                        depth = 1,
                                        displayName = find.path,
                                        id = layerNames.Length + index,
                                    };
                                    item.parent = root;
                                    rows.Add(item);
                                }

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
                    if (data.isResourcePath)
                        GUI.Label(args.GetCellRect(5), EditorGUIUtility.IconContent("d_P4_CheckOutRemote"));
                    //GUI.Toggle(args.GetCellRect(5), data.isResourcePath, "");
                    data.fullScreen = GUI.Toggle(args.GetCellRect(6), data.fullScreen, "");
                    GUI.Label(args.GetCellRect(8), data.order.ToString());
                    GUI.Label(args.GetCellRect(9), data.path);
                    var rect_7 = args.GetCellRect(7);
                    var list = UICollectData.GetFitScriptPaths(data.name);
                    if (list != null)
                    {

                        if (list.Count == 0)
                        {
                            GUI.color = Color.red;
                            GUI.Label(rect_7, "None Script");
                            GUI.color = Color.white;
                        }
                        else if (list.Count == 1)
                        {
                            GUI.Label(rect_7, data.ScriptPath);
                        }
                        else
                        {
                            var tmp = data.ScriptPath;
                            var index = list.IndexOf(tmp);
                            if (index < 0) index = 0;
                            var _index = EditorGUI.Popup(rect_7, index, list.Select(x => x.Replace("/", "_")).ToArray());
                            if (_index != index)
                                data.ScriptPath = list[_index];
                        }
                    }




                    if (GUI.Button(args.GetCellRect(1), EditorGUIUtility.IconContent("Search Icon"), EditorStyles.iconButton))
                    {
                        var p = Resources.FindObjectsOfTypeAll(typeof(UIPanel)).Select(x => x as UIPanel).FirstOrDefault(x => x.name == data.name && !AssetDatabase.Contains(x));
                        if (p != null)
                        {
                            EditorGUIUtility.PingObject(p);
                        }
                    }
                    if (GUI.Button(args.GetCellRect(2), EditorGUIUtility.IconContent("Search Icon"), EditorStyles.iconButton))
                    {
                        PingProject(data);
                    }
                    if (GUI.Button(args.GetCellRect(3), EditorGUIUtility.IconContent("d_editicon.sml"), EditorStyles.iconButton))
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(data.path));
                    }
                    if (GUI.Button(args.GetCellRect(4), EditorGUIUtility.IconContent("d_editicon.sml"), EditorStyles.iconButton))
                    {
                        if (!string.IsNullOrEmpty(data.ScriptPath))
                        {
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(data.ScriptPath, 0);
                        }
                        else
                        {
                            window.ShowNotification(new GUIContent($"None Script for {data.name}"));
                        }
                    }
                }
                private void PingProject(Data data)
                {
                    string path = data.path;
                    if (data.isResourcePath)
                    {
                        EditorGUIUtility.PingObject(Resources.Load<UnityEngine.Object>(path));
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));

                    }
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
                protected override void ContextClicked()
                {
                    var _select = this.GetSelection();
                    if (_select == null || _select.Count == 0) return;
                    var select = _select.ToList();
                    select.RemoveAll(x => x < layerNames.Length);
                    if (select == null || select.Count == 0) return;

                    GenericMenu menu = new GenericMenu();

                    for (int i = 0; i < layerNames.Length; i++)
                    {
                        var name = layerNames[i];
                        menu.AddItem(new GUIContent($"MoveTo/{name}"), false, () =>
                        {
                            foreach (var id in select)
                            {
                                var data = datas[id - layerNames.Length];
                                if (data.layer.ToString() == name) continue;
                                Set(edit.layerObject.LayerNameToIndex(name), int.MaxValue, data);
                            }
                            Reload();
                            SetExpanded(layerNames.ToList().IndexOf(name), true);
                        });
                    }
                    if (select.Count == 1) {
                        var data = datas[select[0] - layerNames.Length];
                        menu.AddItem(new GUIContent($"CopyPath"), false, () =>
                        {
                            GUIUtility.systemCopyBuffer = data.path;
                        });
                    }
                    menu.ShowAsContext();
                }
                private void Set(int layer, int index, Data data)
                {
                    List<Data> last = datas.FindAll(x => x.layer == layer);
                    int lastCount = last.Count;

                    last.Sort((x, y) => { return x.order >= y.order ? 1 : -1; });
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
                        int layer = edit.layerObject.LayerNameToIndex(layerNames[id]);
                        if (args.dragAndDropPosition != DragAndDropPosition.OutsideItems)
                        {
                            var last = datas.FindAll(x => x.layer == layer);
                            var index = args.insertAtIndex;
                            for (int i = 0; i < draggedItemIDs.Count; i++)
                            {
                                Set(layer, index, datas[draggedItemIDs[i] - layerNames.Length]);
                            }

                        }
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

                protected override void DoubleClickedItem(int id)
                {
                    var item = FindItem(id, rootItem);
                    var _data = datas.Find(x => x.path == item.displayName);
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        this.searchString = string.Empty;
                        Reload();
                        SetExpanded(layerNames.ToList().IndexOf(_data.layer.ToString()), true);

                    }
                    else
                    {
                        PingProject(_data);

                    }

                    base.DoubleClickedItem(id);
                }
                public override void OnGUI(Rect rect)
                {
                    var rs = EditorTools.RectEx.HorizontalSplit(rect, 20);
                    this.searchString = searchField.OnGUI(rs[0], this.searchString);
                    base.OnGUI(rs[1]);
                }
                protected override void SearchChanged(string newSearch)
                {
                    base.SearchChanged(newSearch);
                    Reload();
                }
            }
            public override string name => "BuildUILayer";
            LayerView view;
            FolderField GenF = new FolderField();
            FolderField CollectF = new FolderField();
            FolderField ScriptGenF = new FolderField();

            [UnityEngine.SerializeField] private TreeViewState layer_state = new TreeViewState();
            [UnityEngine.SerializeField] private string layerObjectPath;
            private UILayerData layerObject;

            public override void OnEnable()
            {
                var last = EditorTools.GetFromPrefs<UILayerEdit>(name, false);
                if (last != null)
                {
                    layer_state = last.layer_state;
                    layerObjectPath = last.layerObjectPath;
                    layerObject = AssetDatabase.LoadAssetAtPath<UILayerData>(layerObjectPath);
                }

                Fresh();
            }
            public override void OnDisable()
            {
                layerObjectPath = string.Empty;
                if (layerObject != null)
                    layerObjectPath = AssetDatabase.GetAssetPath(layerObject);

                EditorTools.SaveToPrefs<UILayerEdit>(this, name, false);
            }
            public override void OnGUI()
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                var sps = EditorTools.RectEx.HorizontalSplit(rect, 20);
                var last = layerObject;
                layerObject = EditorGUI.ObjectField(sps[0], "Layer Object", layerObject, typeof(UILayerData), false) as UILayerData;

                if (last != layerObject)
                {
                    Fresh();
                }
                if (layerObject == null) return;
                else
                {

                    rect = sps[1];
                    var rs = EditorTools.RectEx.HorizontalSplit(rect, rect.height - 10);
                    view.OnGUI(rs[0]);
                    Tool(rs[1]);
                }
            }


            private void Fresh()
            {
                if (layerObject != null)
                {
                    collect = UICollectData.Collect();
                    view = new LayerView(this);
                }
                else
                {
                    view = null;
                }
                view?.Reload();
            }
            private void Tool(Rect rect)
            {

                var index = UICollectData.planIndex;
                GUILayout.BeginHorizontal();
                var plans = UICollectData.plans;
                index = EditorGUILayout.Popup(index, plans.ConvertAll(x => x.name).ToArray(), GUILayout.Width(150));
                if (index != UICollectData.planIndex)
                {
                    UICollectData.SetPlanIndex(index);
                    Fresh();
                }
                var plan = UICollectData.plan;
                var _name = GUILayout.TextField(plan.name);
                if (GUILayout.Button(nameof(UICollectData.DeletePlan), GUILayout.Width(80)))
                {
                    UICollectData.DeletePlan();
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button(nameof(UICollectData.NewPlan), GUILayout.Width(70)))
                {
                    UICollectData.NewPlan();
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndHorizontal();


                GenF.SetPath(plan.ConfigGenPath);
                CollectF.SetPath(plan.PanelCollectPath);
                ScriptGenF.SetPath(plan.ScriptGenPath);
                GUILayout.BeginHorizontal();
                GUILayout.Label(nameof(UICollectData.Plan.ConfigGenPath), GUILayout.Width(150));
                GenF.OnGUI(EditorGUILayout.GetControlRect());
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label(nameof(UICollectData.Plan.PanelCollectPath), GUILayout.Width(150));
                CollectF.OnGUI(EditorGUILayout.GetControlRect());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(nameof(UICollectData.Plan.ScriptGenPath), GUILayout.Width(150));
                ScriptGenF.OnGUI(EditorGUILayout.GetControlRect());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();


                string scriptName = EditorGUILayout.TextField(nameof(plan.ScriptName), plan.ScriptName);
                var typeIndex = EditorGUILayout.Popup(plan.typeIndex, Plan.shortTypes, GUILayout.Width(150));
                GUILayout.EndHorizontal();
                string configName = EditorGUILayout.TextField(nameof(plan.ConfigName), plan.ConfigName);


                UICollectData.SavePlan(_name, GenF.path, CollectF.path, ScriptGenF.path, scriptName, configName, typeIndex);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(nameof(Fresh))) Fresh();

                    if (GUILayout.Button(nameof(UICollectData.SavePlan)))
                        UICollectData.SavePlan(collect);
                    if (GUILayout.Button(nameof(UICollectData.SavePlans)))
                    {
                        UICollectData.SavePlans();
                        GUIUtility.ExitGUI();
                    }
                }
                GUILayout.EndHorizontal();
            }



        }


    }
}
