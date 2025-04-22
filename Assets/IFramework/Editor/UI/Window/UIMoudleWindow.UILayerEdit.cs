/*********************************************************************************
 *Author:         Wulala
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-09-13
 *Description:    Description
 *History:        2022-09-13--
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static IFramework.EditorTools;
using static IFramework.UI.PanelCollection;

namespace IFramework.UI
{
    partial class UIModuleWindow
    {
        [System.Serializable]
        class UILayerEdit : UIModuleWindowTab
        {
            private static PanelCollection collect;
            private class LayerView : TreeView
            {
                private List<Data> datas { get { return collect.datas; } }
                private SearchField searchField;
                private UILayerEdit edit;
                string[] layerNames;
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
                            headerContent=new GUIContent("Layer"),
                            width=100,
                            maxWidth=100,
                        },
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("FS","FullScreen"),
                            width=30,
                            minWidth=30,
                            maxWidth=30,
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
                            headerContent=new GUIContent("ScriptType"),
                            width=100,
                                      minWidth=100,
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

                protected override TreeViewItem BuildRoot() => new TreeViewItem() { depth = -1 };


                private void BuildToLayer(TreeViewItem layer, List<Data> findList, IList<TreeViewItem> rows)
                {
                    if (findList.Count > 0)
                    {
                        if (IsExpanded(layer.id))
                        {
                            for (int j = 0; j < findList.Count; j++)
                            {
                                var find = findList[j];

                                var index = datas.IndexOf(find);
                                TreeViewItem item = new TreeViewItem()
                                {
                                    depth = 1,
                                    displayName = find.path,
                                    id = index,
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

                private void BuildToRoot(TreeViewItem root, List<Data> findList, IList<TreeViewItem> rows)
                {
                    if (findList.Count > 0)
                    {
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
                                id = index,
                            };
                            item.parent = root;
                            rows.Add(item);
                        }

                    }

                }
                List<string> dirs = new List<string>();
                protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
                {
                    var rows = GetRows() ?? new List<TreeViewItem>();
                    rows.Clear();
                    dirs.Clear();
                    if (edit.mode == Mode.Layer)
                    {
                        for (int i = 0; i < layerNames.Length; i++)
                        {
                            var findList = datas.FindAll(x => x.layer == edit.layerObject.LayerNameToIndex(layerNames[i]));
                            if (string.IsNullOrEmpty(searchString))
                            {
                                TreeViewItem layer = new TreeViewItem()
                                {
                                    depth = 0,
                                    displayName = $"{layerNames[i]}\t\t {findList.Count} : {datas.Count}",
                                    id = -i - layerStartIndex,
                                };
                                layer.parent = root;
                                rows.Add(layer);
                                BuildToLayer(layer, findList, rows);
                            }
                            else
                            {
                                BuildToRoot(root, findList, rows);
                            }


                        }
                    }
                    else
                    {
                        Dictionary<string, List<Data>> map = new Dictionary<string, List<Data>>();

                        var result = datas.Select(x => new { dir = Path.GetDirectoryName(x.path), data = x });

                        foreach (var item in result)
                        {
                            List<Data> list = null;
                            if (!map.TryGetValue(item.dir, out list))
                            {
                                list = new List<Data>();
                                map.Add(item.dir, list);
                            }
                            list.Add(item.data);
                        }

                        int index = -dirStartIndex;
                        foreach (var dir in map.Keys)
                        {
                            var findList = map[dir];
                            if (string.IsNullOrEmpty(searchString))
                            {
                                TreeViewItem layer = new TreeViewItem()
                                {
                                    depth = 0,
                                    displayName = $"{dir}\t\t {findList.Count} : {datas.Count}",
                                    id = index,
                                };
                                layer.parent = root;
                                rows.Add(layer);
                                index--;
                                dirs.Add(dir);
                                BuildToLayer(layer, findList, rows);

                            }
                            else
                            {
                                BuildToRoot(root, findList, rows);

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
                    var data = datas[args.item.id];

                    GUI.Label(EditorTools.RectEx.Zoom(args.GetCellRect(0), TextAnchor.MiddleRight, new Vector2(-indent, 0)), data.name);
                    var temp = EditorGUI.Popup(args.GetCellRect(1), data.layer, layerNames);
                    if (temp != data.layer)
                    {
                        data.layer = temp;
                        Reload();
                    }
                    data.fullScreen = GUI.Toggle(args.GetCellRect(2), data.fullScreen, "");
                    if (data.isResourcePath)
                        GUI.Label(args.GetCellRect(7), EditorGUIUtility.IconContent("d_P4_CheckOutRemote"));
                    GUI.Label(args.GetCellRect(9), data.path);
                    var rect_7 = args.GetCellRect(8);
                    var seg = ScriptPathCollection.GetSeg(data);
                    var list = seg.Paths;

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
                            GUI.Label(rect_7, seg.ScriptPath);
                        }
                        else
                        {
                            var tmp = seg.ScriptPath;
                            var index = list.IndexOf(tmp);
                            if (index < 0) index = 0;
                            var _index = EditorGUI.Popup(rect_7, index, list.Select(x => x.Replace("/", "_")).ToArray());
                            if (_index != index)
                            {
                                seg.ScriptPath = list[_index];
                                ScriptPathCollection.SaveScriptsData();
                            }

                        }
                    }




                    if (GUI.Button(args.GetCellRect(3), EditorGUIUtility.IconContent("Search Icon"), EditorStyles.iconButton))
                    {
                        var p = Resources.FindObjectsOfTypeAll(typeof(UIPanel)).Select(x => x as UIPanel).FirstOrDefault(x => x.name == data.name && !AssetDatabase.Contains(x));
                        if (p != null)
                        {
                            EditorGUIUtility.PingObject(p);
                        }
                    }
                    if (GUI.Button(args.GetCellRect(4), EditorGUIUtility.IconContent("Search Icon"), EditorStyles.iconButton))
                    {
                        PingProject(data);
                    }
                    if (GUI.Button(args.GetCellRect(5), EditorGUIUtility.IconContent("d_editicon.sml"), EditorStyles.iconButton))
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(data.path));
                    }
                    if (GUI.Button(args.GetCellRect(6), EditorGUIUtility.IconContent("d_editicon.sml"), EditorStyles.iconButton))
                    {
                        GameObject go = data.isResourcePath ? Resources.Load<GameObject>(data.path) : AssetDatabase.LoadAssetAtPath<GameObject>(data.path);
                        window.SwitchToGenCode(go, seg.ScriptPath);
                        //return;
                        //if (!string.IsNullOrEmpty(seg.ScriptPath))
                        //{
                        //    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(seg.ScriptPath, 0);
                        //}
                        //else
                        //{
                        //    window.ShowNotification(new GUIContent($"None Script for {data.name}"));
                        //}
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


                protected override void ContextClicked()
                {
                    var _select = this.GetSelection();
                    if (_select == null || _select.Count == 0) return;
                    var select = _select.ToList();
                    if (select == null || select.Count == 0) return;

                    GenericMenu menu = new GenericMenu();

                    for (int i = 0; i < layerNames.Length; i++)
                    {
                        var name = layerNames[i];
                        menu.AddItem(new GUIContent($"MoveTo/{name}"), false, () =>
                        {
                            foreach (var id in select)
                            {
                                var data = datas[id];
                                if (data.layer.ToString() == name) continue;
                                data.layer = edit.layerObject.LayerNameToIndex(name);
                            }
                            Reload();
                            SetExpanded(layerNames.ToList().IndexOf(name), true);
                        });
                    }
                    if (select.Count == 1)
                    {
                        var data = datas[select[0]];
                        menu.AddItem(new GUIContent($"CopyPath"), false, () =>
                        {
                            GUIUtility.systemCopyBuffer = data.path;
                        });
                    }
                    menu.ShowAsContext();
                }
                static int dirStartIndex { get { return 10000; } }
                static int layerStartIndex { get { return 100; } }

                private void ExpandedParent(Data data)
                {
                    if (edit.mode == Mode.Layer)
                    {

                        var index = data.layer;
                        SetExpanded(-index - layerStartIndex, true);
                    }
                    else
                    {
                        var _dir = Path.GetDirectoryName(data.path);
                        var index = dirs.IndexOf(_dir);
                        SetExpanded(-index - dirStartIndex, true);

                    }
                }
                protected override void DoubleClickedItem(int id)
                {
                    if (id < 0) return;

                    //var item = FindItem(id, rootItem);
                    var _data = datas[id];
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        this.searchString = string.Empty;
                        Reload();
                        ExpandedParent(_data);


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
                    var rss = RectEx.VerticalSplit(rs[0], 100, 10);
                    var _mode = (Mode)EditorGUI.EnumPopup(rss[0], edit.mode);
                    if (_mode != edit.mode)
                    {
                        edit.mode = _mode;
                        Reload();
                    }
                    this.searchString = searchField.OnGUI(rss[1], this.searchString);
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
            private enum Mode
            {
                Layer,
                Directory,
            }
            [UnityEngine.SerializeField] private Mode mode;
            /*[UnityEngine.SerializeField] */
            private TreeViewState layer_state = new TreeViewState();
            [UnityEngine.SerializeField] private string layerObjectPath;
            private UILayerData layerObject;

            public override void OnEnable()
            {
                var last = EditorTools.GetFromPrefs<UILayerEdit>(name, false);
                if (last != null)
                {
                    mode = last.mode;
                    layer_state = EditorTools.GetFromPrefs<TreeViewState>(name, true);
                    layerObjectPath = last.layerObjectPath;
                    layerObject = AssetDatabase.LoadAssetAtPath<UILayerData>(layerObjectPath);
                }
                if (layer_state == null) layer_state = new TreeViewState();
                Fresh();
            }
            public override void OnDisable()
            {
                layerObjectPath = string.Empty;
                if (layerObject != null)
                    layerObjectPath = AssetDatabase.GetAssetPath(layerObject);
                EditorTools.SaveToPrefs<TreeViewState>(layer_state, name, true);
                EditorTools.SaveToPrefs<UILayerEdit>(this, name, false);
            }
            public override void OnGUI()
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                var sps = EditorTools.RectEx.HorizontalSplit(rect, 20, 8);
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
                    var rs = EditorTools.RectEx.HorizontalSplit(rect, rect.height - 10, 0);
                    view.OnGUI(rs[0]);
                    Tool(rs[1]);
                }
            }


            private void Fresh()
            {
                if (layerObject != null)
                {
                    collect = EditorPanelCollectionPlans.Collect(EditorPanelCollectionPlans.plan_current);
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
                var index = EditorPanelCollectionPlans.planIndex;
                GUILayout.BeginHorizontal();
                var plans = EditorPanelCollectionPlans.plans;
                index = EditorGUILayout.Popup(index, plans.ConvertAll(x => x.name).ToArray(), GUILayout.Width(150));
                if (index != EditorPanelCollectionPlans.planIndex)
                {
                    EditorPanelCollectionPlans.planIndex = index;
                    Fresh();
                }
                var plan = EditorPanelCollectionPlans.plan_current;
                var _name = GUILayout.TextField(plan.name);
                if (GUILayout.Button(nameof(EditorPanelCollectionPlans.DeletePlan), GUILayout.Width(80)))
                {
                    EditorPanelCollectionPlans.DeletePlan();
                    Fresh();
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button(nameof(EditorPanelCollectionPlans.NewPlan), GUILayout.Width(70)))
                {
                    EditorPanelCollectionPlans.NewPlan();
                    Fresh();

                    GUIUtility.ExitGUI();
                }
                GUILayout.EndHorizontal();


                GenF.SetPath(plan.ConfigGenPath);
                CollectF.SetPath(plan.PanelCollectPath);
                ScriptGenF.SetPath(plan.ScriptGenPath);
                GUILayout.BeginHorizontal();
                GUILayout.Label(nameof(EditorPanelCollectionPlan.ConfigGenPath), GUILayout.Width(150));
                GenF.OnGUI(EditorGUILayout.GetControlRect());
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label(nameof(EditorPanelCollectionPlan.PanelCollectPath), GUILayout.Width(150));
                CollectF.OnGUI(EditorGUILayout.GetControlRect());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(nameof(EditorPanelCollectionPlan.ScriptGenPath), GUILayout.Width(150));
                ScriptGenF.OnGUI(EditorGUILayout.GetControlRect());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();


                string scriptName = EditorGUILayout.TextField(nameof(plan.ScriptName), plan.ScriptName);
                var typeIndex = EditorGUILayout.Popup(plan.typeIndex, EditorPanelCollectionPlan.shortTypes, GUILayout.Width(150));
                GUILayout.EndHorizontal();
                string configName = EditorGUILayout.TextField(nameof(plan.ConfigName), plan.ConfigName);


                EditorPanelCollectionPlans.SaveCurrentPlan(_name, GenF.path, CollectF.path, ScriptGenF.path, scriptName, configName, typeIndex);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(nameof(Fresh))) Fresh();

                    if (GUILayout.Button(nameof(EditorPanelCollectionPlans.GenPlan)))
                        EditorPanelCollectionPlans.GenPlan(plan, collect);
                    if (GUILayout.Button(nameof(EditorPanelCollectionPlans.GenPlans)))
                    {
                        EditorPanelCollectionPlans.GenPlans();
                        GUIUtility.ExitGUI();
                    }
                }
                GUILayout.EndHorizontal();
            }

        }


    }
}
