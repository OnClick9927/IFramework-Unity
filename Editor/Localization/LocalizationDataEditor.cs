/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;

namespace IFramework
{
    [CustomEditor(typeof(LocalizationData))]
    public class LocalizationDataEditor : Editor
    {
        private class Tree : TreeView
        {
            private SearchField search = new SearchField();
            public Tree(TreeViewState state) : base(state)
            {
                this.showBorder = true;
                this.showAlternatingRowBackgrounds = true;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {
                var lanTypes = context.GetLocalizationTypes();
                var columns = new List<Column>() {
                new MultiColumnHeaderState.Column()
                        {
                            autoResize = true,
                            headerContent = new GUIContent("Key")
                        }
                };
                for (int i = 0; lanTypes.Count > i; i++)
                {
                    columns.Add(new Column()
                    {
                        autoResize = true,
                        headerContent = new GUIContent(lanTypes[i])
                    });
                }
                this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(columns.ToArray()));
                return new TreeViewItem() { depth = -1 };
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {

                var keys = context.GetLocalizationKeys();

                var rows = new List<TreeViewItem>();
                rows.Clear();
                if (keys != null)
                {

                    for (int i = 0; i < keys.Count; i++)
                    {
                        bool build = string.IsNullOrEmpty(this.searchString) || keys[i].ToLower().Contains(this.searchString.ToLower());
                        if (!build) continue;
                        rows.Add(new TreeViewItem()
                        {
                            displayName = keys[i],
                            depth = 0,
                        }); ;
                    }
                }

                SetupDepthsFromParentsAndChildren(root);
                return rows;
            }
            protected override void RowGUI(RowGUIArgs args)
            {
                float indent = this.GetContentIndent(args.item);
                var key = args.item.displayName;
                GUI.Label(EditorTools.RectEx.Zoom(args.GetCellRect(0), TextAnchor.MiddleRight, new Vector2(-indent, 0)), key);
                var lanTypes = context.GetLocalizationTypes();
                for (int i = 0; lanTypes.Count > i; i++)
                {
                    var type = lanTypes[i];
                    GUI.Label(args.GetCellRect(i + 1), context.GetLocalization(type, key));
                }

            }
            public override void OnGUI(Rect rect)
            {
                var rs = EditorTools.RectEx.HorizontalSplit(rect, 20);

                var tmp = search.OnGUI(rs[0], this.searchString);
                if (tmp != this.searchString)
                {
                    this.searchString = tmp;
                    Reload();
                }
                base.OnGUI(rs[1]);
            }
        }
        private Tree tree;
        private TreeViewState state = new TreeViewState();
        private static LocalizationData context;
        private void OnEnable()
        {
            context = target as LocalizationData;
            tree = new Tree(state);
        }
        private string LanType = "";
        private string Key = "";
        private string VAL = "";

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            LanType = EditorGUILayout.TextField("LanType", LanType);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {

                context.Add(LanType);
                EditorUtility.SetDirty(context);
                AssetDatabase.SaveAssetIfDirty(context);
                tree.Reload();
            }
            GUILayout.EndHorizontal();

            Key = EditorGUILayout.TextField("Key", Key);
            GUILayout.BeginHorizontal();
            VAL = EditorGUILayout.TextField("VAL", VAL);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {

                context.Add(LanType,Key,VAL);
                EditorUtility.SetDirty(context);
                AssetDatabase.SaveAssetIfDirty(context);
                tree.Reload();
            }
            GUILayout.EndHorizontal();

            tree.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Height(600)));
        }
    }
}
