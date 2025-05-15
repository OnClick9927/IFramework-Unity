/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

        class ShowParams
        {
            public Canvas canvas;
            public UIPanel top;
            public List<UIPanel> Visible = new List<UIPanel>();
            public class LayerData
            {
                public string layer;
                public UIPanel top;
                public UIPanel top_show;
            }
            public List<LayerData> layers = new List<LayerData>();
        }
        private ShowParams show = new ShowParams();
        public override void OnGUI()
        {
            if (!EditorApplication.isPlaying) return;
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
            GUI.enabled = false;
            EditorTools.DrawDefaultInspector(show);
            GUI.enabled = true;
            GUILayout.EndScrollView();

        }
        Vector2 scroll;
    }
}
