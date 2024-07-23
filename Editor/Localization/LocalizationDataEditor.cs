/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;

namespace IFramework.Localization
{
    [CustomEditor(typeof(LocalizationData))]
    class LocalizationDataEditor : Editor
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
                new Column()
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
                return new TreeViewItem() { depth = -1, id = 1 };
            }
            private List<TreeViewItem> _rows = new List<TreeViewItem>();
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
                            id = i + 2,
                            displayName = keys[i],
                            depth = 0,
                            parent = root,
                        }); ;
                    }
                }

                SetupDepthsFromParentsAndChildren(root);
                _rows = rows;
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
            protected override bool CanMultiSelect(TreeViewItem item)
            {
                return true;
            }
            protected override void ContextClicked()
            {
                GenericMenu menu = new GenericMenu();

                var lanTypes = context.GetLocalizationTypes();
                for (int i = 0; i < lanTypes.Count; i++)
                {
                    var type = lanTypes[i];
                    menu.AddItem(new GUIContent($"Delete Localization Type/{type}"), false, () =>
                    {
                        context.ClearLan(type);
                        SaveContext();
                        Reload();
                    });
                }
                var select = this.GetSelection();
                if (search != null && select.Count > 0)
                {
                    var keys = select.Select(x => _rows.Find(y => y.id == x).displayName).ToList();
                    menu.AddItem(new GUIContent("Delete Select"), false, () =>
                    {
                        context.ClearKeys(keys);
                        SaveContext();
                        Reload();

                    });
                }
                menu.ShowAsContext();
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
        private static void SaveContext()
        {
            EditorUtility.SetDirty(context);
            AssetDatabase.SaveAssetIfDirty(context);
        }
        private static void WriteCSV()
        {
            var path = EditorUtility.SaveFilePanel("Save CSV",
                LocalizationEditorUserData.lastCSVPath, $"{context.name}.csv",
                "csv");
            if (string.IsNullOrEmpty(path)) return;
            LocalizationEditorUserData.lastCSVPath = path;
            var types = context.GetLocalizationTypes();
            var keys = context.GetLocalizationKeys();
            var header = new List<string>(types);
            header.Insert(0, string.Empty);
            List<string[]> result = new List<string[]>() { header.ToArray() };
            for (int i = 0; i < keys.Count; i++)
            {
                string[] _content = new string[types.Count + 1];
                var key = keys[i];
                _content[0] = key;
                for (int j = 0; j < types.Count; j++)
                {
                    var type = types[j];
                    var value = context.GetLocalization(type, key);
                    _content[j + 1] = value;
                }
                result.Add(_content);
            }

            CSVHelper.Write(path, result);
        }
        private static void ReadCSV()
        {
            var path = EditorUtility.OpenFilePanelWithFilters("Select CSV", LocalizationEditorUserData.lastCSVPath, new string[] { "CSV", "csv" });
            if (string.IsNullOrEmpty(path)) return;
            LocalizationEditorUserData.lastCSVPath = Path.GetDirectoryName(path);
            CSVHelper.BeginRead(path);
            int index = 0;
            string[] lanTypes = null;
            while (true)
            {
                var fields = CSVHelper.ReadFields();
                if (fields == null) break;
                if (index == 0)
                {
                    lanTypes = fields;
                }
                else
                {
                    var key = fields[0];
                    for (int j = 1; j < fields.Length; j++)
                    {
                        var value = fields[j];
                        var lan = lanTypes[j];
                        context.Add(lan, key, value);
                    }
                }

                index++;
            }
            SaveContext();
        }

        private static void Clear()
        {
            context.Clear();
            SaveContext();
        }
        private static void ReadLocalizationData()
        {

            var path = EditorUtility.OpenFilePanelWithFilters("Select LocalizationData", LocalizationEditorUserData.lastLocalizationDataPath, new string[] { "LocalizationData", "asset" });
            if (string.IsNullOrEmpty(path)) return; 
            path = path.ToAssetsPath();
            var src = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);
            if (src == null) return;
            LocalizationEditorUserData.lastLocalizationDataPath = path;
            var types = src.GetLocalizationTypes();
            var keys = src.GetLocalizationKeys();
            foreach (var key in keys)
            {
                foreach (var type in types)
                {
                    var value = src.GetLocalization(type, key);
                    context.Add(type, key, value);
                }
            }
            SaveContext();
        }



        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Clear Data"))
            {
                Clear();
                tree.Reload();
            }
            if (GUILayout.Button("Read From Asset"))
            {
                ReadLocalizationData();
                tree.Reload();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Read From CSV"))
            {
                ReadCSV();
                tree.Reload();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Write To CSV"))
            {
                WriteCSV();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();


            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            LanType = EditorGUILayout.TextField("LanType", LanType);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                context.Add(LanType);
                SaveContext();
                tree.Reload();
            }
            GUILayout.EndHorizontal();

            Key = EditorGUILayout.TextField("Key", Key);
            GUILayout.BeginHorizontal();
            VAL = EditorGUILayout.TextField("VAL", VAL);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {

                context.Add(LanType, Key, VAL);
                SaveContext();
                tree.Reload();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            tree.OnGUI(EditorGUILayout.GetControlRect(GUILayout.MaxHeight(600)));
        }
    }
}
