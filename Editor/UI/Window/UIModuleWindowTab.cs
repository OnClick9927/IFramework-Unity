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
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace IFramework.UI
{
    public abstract class UIModuleWindowTab
    {
        public abstract string name { get; }
        public abstract void OnGUI();
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnHierarchyChanged() { }
    }

    class RunTimeTab : UIModuleWindowTab
    {
        public override string name => "Runtime";

        int ui_name_index;
        public enum Mode
        {
            Hierarchy,
            Param
        }
        public Mode mode;
        class ShowParams
        {
            public Canvas canvas;
            public UIPanel top;
            public List<UIPanel> Visible = new List<UIPanel>();
            [System.Serializable]
            public class LayerData
            {
                public string layer;
                public UIPanel top;
                public UIPanel top_show;
            }
            public List<LayerData> layers = new List<LayerData>();
        }
        private ShowParams show = new ShowParams();
        private class Tree : TreeView
        {
            private Canvas canvas;

            public Tree(TreeViewState state) : base(state)
            {

            }

            protected override TreeViewItem BuildRoot()
            {
                return new TreeViewItem()
                {
                    depth = -1,
                    id = -1,
                };
            }
            protected override bool CanMultiSelect(TreeViewItem item) => false;

            private void Build(GameObject go, TreeViewItem parent, IList<TreeViewItem> result)
            {
                var item = new TreeViewItem()
                {
                    depth = parent.depth + 1,
                    id = go.GetInstanceID(),
                    parent = parent,
                    displayName = go.name,
                };

                result.Add(item);
                if (!go.GetComponent<UIPanel>())
                    for (int i = 0; i < go.transform.childCount; i++)
                    {

                        Build(go.transform.GetChild(i).gameObject, item, result);

                    }
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                var rows = GetRows() ?? new List<TreeViewItem>();
                rows.Clear();
                if (canvas != null)
                {
                    Build(canvas.gameObject, root, rows);
                }
                SetupParentsAndChildrenFromDepths(root, rows);

                return rows;
            }
            public void Reload(Canvas canvas)
            {
                this.canvas = canvas;
                Reload();
            }
            protected override void DoubleClickedItem(int id) => EditorGUIUtility.PingObject(id);
        }
        private Tree tree;
        public override void OnHierarchyChanged()
        {
            if (tree == null)
                tree = new Tree(new TreeViewState());
            tree.Reload(show?.canvas);
        }
        public override void OnGUI()
        {
            if (!EditorApplication.isPlaying) return;
            mode = (Mode)GUILayout.Toolbar((int)mode, Enum.GetNames(typeof(Mode)));
            var moudules = Game.Current.modules.FindModules(typeof(UIModule));
            if (moudules == null) return;
            var names = moudules.Select(m => m.name).ToArray();
            ui_name_index = EditorGUILayout.Popup("Module", ui_name_index, names);
            UIModule module = moudules.FirstOrDefault(x => x.name == names[ui_name_index]) as UIModule;
            show.Visible.Clear();
            show.Visible.AddRange(module.GetVisibleList().Select(x => module.FindPanel(x)));

            show.canvas = module.canvas;
            show.top = module.GetTopShow();
            show.layers.Clear();
            show.layers.AddRange(module.GetLayerNames().ConvertAll(x => new ShowParams.LayerData()
            {
                layer = x,
                top_show = module.GetLayerTopShow(module.LayerNameToIndex(x)),
                top = module.GetLayerTop(module.LayerNameToIndex(x)),
            }));



            scroll = GUILayout.BeginScrollView(scroll);
            if (mode == Mode.Param)
            {
                GUI.enabled = false;
                EditorTools.DrawDefaultInspector(show);
                GUI.enabled = true;
            }
            else if (mode == Mode.Hierarchy)
            {
                if (tree == null)
                    OnHierarchyChanged();
                tree?.OnGUI(EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true)));
            }
            GUILayout.EndScrollView();

        }
        Vector2 scroll;
    }
}
