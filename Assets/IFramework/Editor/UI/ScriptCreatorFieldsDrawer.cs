/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using static IFramework.EditorTools;
using static UnityEditor.GenericMenu;

namespace IFramework.UI
{
    class ScriptCreatorFieldsDrawer
    {
        public enum SearchType
        {
            Name,
            FieldName,
            FieldType
        }
        private class Tree : TreeView
        {
            public SearchType searchType;


            private GameObject go => context.gameObject;
            private ScriptCreator context;
            private SearchField search;

            public Tree(TreeViewState state, SearchType searchType, ScriptCreator sc) : base(state)
            {
                this.context = sc;
                this.searchType = searchType;
                search = new SearchField();
                this.showBorder = true;
                this.showAlternatingRowBackgrounds = true;
                this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[]
                {
                        new MultiColumnHeaderState.Column()
                        {
                            headerContent=new GUIContent("Name")
                        },
                        new MultiColumnHeaderState.Column()
                        {
                           headerContent=new GUIContent("Mark")
                        },
                         new MultiColumnHeaderState.Column()
                        {
                           headerContent=new GUIContent("FieldName")
                        },
        

                })); ;
                Reload();
                this.multiColumnHeader.ResizeToFit();
            }

            protected override TreeViewItem BuildRoot()
            {
                return new TreeViewItem { id = 0, depth = -1 };
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                var rows = GetRows() ?? new List<TreeViewItem>();
                rows.Clear();
                AddChildrenRecursive(this.go, root, rows);
                SetupDepthsFromParentsAndChildren(root);
                return rows;
            }
            static TreeViewItem CreateTreeViewItemForGameObject(GameObject gameObject)
            {
                return new TreeViewItem(gameObject.GetInstanceID(), -1, gameObject.name);
            }
            GameObject GetGameObject(int instanceID)
            {
                return (GameObject)EditorUtility.InstanceIDToObject(instanceID);
            }
            private bool CouldDrawChildren(GameObject go)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                     if (context.CouldMark(go.transform.GetChild(i).gameObject))
                        return true;
                return false;
            }
            private bool CouldDraw(GameObject go)
            {
                if (string.IsNullOrEmpty(this.searchString)) return true;
                string low = this.searchString.ToLower();
                var sm = context.GetMarks().Find(x => x.gameObject == go);
                switch (searchType)
                {
                    case SearchType.Name:
                        return go.name.ToLower().Contains(low);
                    case SearchType.FieldName:
                        return sm != null && sm.fieldName.ToLower().Contains(low);
                    case SearchType.FieldType:
                        return sm != null && sm.fieldType.ToLower().Contains(low);
                    default:
                        return false;
                }

            }
            void AddChildrenRecursive(GameObject go, TreeViewItem root, IList<TreeViewItem> rows)
            {
                if (go == null || !context.CouldMark(go)) return;
                TreeViewItem item = null;
                if (CouldDraw(go))
                {
                    item = CreateTreeViewItemForGameObject(go);
                    item.parent = root;
                    if (root.children == null)
                        root.children = new List<TreeViewItem>();
                    rows.Add(item);
                    root.children.Add(item);
                    item.depth = root.depth + 1;
                }
                if (!CouldDrawChildren(go)) return;
                int childCount = go.transform.childCount;
                if (string.IsNullOrEmpty(this.searchString))
                {
                    if (IsExpanded(item.id))
                        for (int i = 0; i < childCount; i++)
                            AddChildrenRecursive(go.transform.GetChild(i).gameObject, item, rows);
                    else
                        item.children = CreateChildListForCollapsedParent();
                }
                else
                {
                    for (int i = 0; i < childCount; i++)
                        AddChildrenRecursive(go.transform.GetChild(i).gameObject, root, rows);
                }
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                var go = GetGameObject(args.item.id);
                if (go == null)
                {
                    GUI.color = Color.red;
                    GUI.Label(args.rowRect, "GameObject Not Exist");
                    GUI.color = Color.white;

                    return;
                }
                float indet = this.GetContentIndent(args.item);
                var first = EditorTools.RectEx.Zoom(args.GetCellRect(0), TextAnchor.MiddleRight, new Vector2(-indet, 0));
                if (context.IsPrefabInstance(go))
                    GUI.color = new Color(0.1f, 0.7f, 1f, 1);

                //bool could = context.CouldMark(go);
                //var image = could ? "greenLight" : "d_redLight";
                GUI.Label(first, args.label);

                if (go != null)
                {
                    var sm = context.GetMark(go);
                    //if (!could)
                    //    sm = context.GetPrefabMark(go);
                    if (sm != null)
                    {
                        var rect = args.GetCellRect(1);
                        GUI.Label(rect, sm.fieldType);
                        rect = args.GetCellRect(2);
                        GUI.Label(rect, sm.fieldName);
                    }
                }
                GUI.color = Color.white;

                if (go == _ping)
                    GUI.Label(RectEx.Zoom(args.rowRect, TextAnchor.MiddleCenter, -8), "", "LightmapEditorSelectedHighlight");
            }
            public override void OnGUI(Rect rect)
            {
                var rs = EditorTools.RectEx.HorizontalSplit(rect, 18);
                var rs1 = EditorTools.RectEx.VerticalSplit(rs[0], 150, 10);
                var _tmp = (SearchType)EditorGUI.EnumPopup(rs1[0], searchType);
                if (_tmp != searchType)
                {
                    searchType = _tmp;
                    Reload();
                }
                var tmp = search.OnToolbarGUI(rs1[1], this.searchString);
                if (tmp != this.searchString)
                {
                    this.searchString = tmp;
                    Reload();
                }
                base.OnGUI(rs[1]);
            }


            protected override bool CanRename(TreeViewItem item)
            {
                var go = GetGameObject(item.id);
                return context.GetMark(go) != null;

            }

            protected override void RenameEnded(RenameEndedArgs args)
            {
                var id = args.itemID;
                var go = GetGameObject(id);
                var sm = context.GetMark(go);
                sm.fieldName = context.ToValidFiledName(args.newName);
                context.SaveContext();

            }
            protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
            {
                return this.multiColumnHeader.GetCellRect(2, rowRect);
            }

            private static void CreateMenu(GenericMenu menu, string content, bool disable, MenuFunction action)
            {
                if (disable)
                    menu.AddDisabledItem(new GUIContent(content));
                else
                    menu.AddItem(new GUIContent(content), false, action);

            }
            protected override void ContextClicked()
            {
                if (context.gameObject == null) return;
                GenericMenu menu = new GenericMenu();

                Dictionary<Type, int> help = new Dictionary<Type, int>();
                List<GameObject> gameobjects = new List<GameObject>();
                var marks = new List<GameObject>();
                var selection = this.GetSelection().Select(x => GetGameObject(x)).ToList();
                if (selection.Count == 0) return;
                var normal = selection.FindAll(y => context.CouldMark(y));
                var prefab = selection.FindAll(y => !context.CouldMark(y));

                for (int i = 0; i < normal.Count; i++)
                {
                    var go = normal[i];
                    gameobjects.Add(go);
                    var sm = context.GetMark(go);
                    if (sm != null) marks.Add(go);

                    Component[] components = go.GetComponents<Component>();
                    foreach (var component in components)
                    {
                        Type componentType = component.GetType();
                        if (help.ContainsKey(componentType)) help[componentType]++;
                        else help[componentType] = 1;
                    }

                }
                List<Type> types = new List<Type>() {
                typeof(GameObject),
                typeof(Transform)
                };
                if (help.Count > 0)
                {
                    int max = help.Values.ToList().Max();
                    foreach (var item in help)
                        if (item.Value == max)
                            types.Add(item.Key);
                }
                if (normal.Count > 0)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        var type = types[i];
                        menu.AddItem(new GUIContent($"Mark Component/{type.FullName}"), false, () =>
                        {
                            context.RemoveMarks(marks.ConvertAll(x => x.gameObject));
                            for (int i = 0; i < gameobjects.Count; i++)
                            {
                                context.AddMark(gameobjects[i], type);
                            }
                            Reload();
                        });
                        if (i == 1)
                            menu.AddSeparator("Mark Component/");
                    }
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Mark Component"), false);
                }

                CreateMenu(menu, "Remove Marks", types.Count == 0 || normal.Count == 0, () =>
                {
                    context.RemoveMarks(marks.ConvertAll(x => x.gameObject));
                    Reload();

                });
                CreateMenu(menu, "Destroy All Marks", false, () =>
                {
                    context.DestroyAllMarks();
                    Reload();

                });
  
                menu.AddSeparator("");
                CreateMenu(menu, "Fresh FieldNames", false, () =>
                {
                    var all = context.GetMarks();
                    all.RemoveAll(x => x == null);
                    var goes = all.ConvertAll(m => { return (m.gameObject, m.fieldType); });
                    context.RemoveMarks(all.ConvertAll(x => x.gameObject));
                    foreach (var item in goes)
                    {
                        context.AddMark(item.gameObject, item.fieldType);
                    }
                    context.SaveContext();
                    Reload();

                });
                CreateMenu(menu, "Check FiledNames", false, () =>
                {
                    string same;
                    if (context.HandleSameFieldName(out same))
                    {
                        same = "same FieldName\n" + same;
                        same += "\n err repair finish ";
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent(same));
                        context.SaveContext();
                        Reload();
                    }
                    else
                    {
                        same = "perfect!";
                    }
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent(same));
                });
                CreateMenu(menu, "Remove Useless Mark Flag", false, () =>
                {
                    context.RemoveUselessMarkFlag();
                    Reload();
                });

                menu.ShowAsContext();
            }
            GameObject _ping;
            protected async override void DoubleClickedItem(int id)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    var _ping = GetGameObject(id);
                    this._ping = _ping;
                    while (true)
                    {
                        this.SetExpanded(_ping.GetInstanceID(), true);
                        if (_ping.transform.parent == null) break;
                        _ping = _ping.transform.parent.gameObject;
                    }
                    this.FrameItem(id);
                    this.searchString = string.Empty;
                    Reload();
                    this.FrameItem(id);
                    await Task.Delay(1000);
                    this._ping = null;
                    Reload();
                }
            }
        }
        public void Reload()
        {
            _tree.Reload();
        }

        private Tree _tree;
        public ScriptCreatorFieldsDrawer(ScriptCreator creator, TreeViewState state, SearchType searchType)
        {
            if (state == null)
            {
                state = new TreeViewState();
            }
            _tree = new Tree(state, searchType, creator);
        }
        public void OnGUI()
        {
            var position = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
            _tree.OnGUI(position);
        }

        internal SearchType GetSearchType() => _tree.searchType;
    }

}
