/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.UI
{
    partial class UIService
    {
        private class LoadPart
        {

            private int __Loading;
            public bool IsLoading => __Loading != 0;

            private int _loading
            {
                get { return __Loading; }
                set
                {
                    if (__Loading == value) return;
                    __Loading = value;
                    if (value == 0)
                        module.AcceptRayCast();
                    else
                        module.RefuseRayCast();
                }
            }

            private UIService module;
            private Dictionary<string, UIPanel> panels = new Dictionary<string, UIPanel>();

            public IEnumerable<string> GetLoadedPanelPaths() => panels.Keys;
            public void RemovePanel(string path)
            {

                if (panels.TryGetValue(path, out var panel))
                {
                    module.delPart.DestroyPanel(panel.gameObject);
                    panels.Remove(path);
                }
            }

            public LoadPart(UIService module)
            {
                this.module = module;
                panels = new Dictionary<string, UIPanel>();
            }
            public UIPanel Find(string path)
            {
                UIPanel ui;
                panels.TryGetValue(path, out ui);
                return ui;
            }

            public async void LoadPanel(string path, int layer, AsyncTask show_op)
            {
                var panel = Find(path);
                if (panel != null)
                    module.OnShowCallBack(path, true, panel, show_op);
                else
                {
                    var data = module.GetPanelData(path);
                    if (data == null)
                    {
                        Log.FE($"{path} Not Find Panel In {nameof(PanelCollection)} ,Try Gen Plan");
                        return;
                    }
                    RectTransform parent = module.GetLayerTransform(module.GetLayerName(layer));
                    _loading++;
                    var result = await module.delPart.LoadPanelAsync(parent, data);
                    if (result != null)
                    {

                        if (result != null) panels.Add(path, result);
                        module.UILoadComplete(path, result, show_op);
                    }
                    else
                        Log.FE($"Can't load ui with Name: {path}");
                    _loading--;
                }
            }

        }
    }
}
