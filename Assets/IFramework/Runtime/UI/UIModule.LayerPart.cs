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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IFramework.UI
{
    partial class UIModule
    {
        private class LayerPart
        {


            public List<UIPanel> GetPanelsByLayerName(string name)
            {
                List<UIPanel> panels = null;
                if (!_panelOrders.TryGetValue(name, out panels))
                {
                    panels = new List<UIPanel>();
                    _panelOrders[name] = panels;
                }
                return panels;
            }
            public List<UIPanel> FindPanelsByLayerName(string name)
            {
                List<UIPanel> panels = null;
                _panelOrders.TryGetValue(name, out panels);
                return panels;
            }

            private Dictionary<string, List<UIPanel>> _panelOrders;
            private Dictionary<string, RectTransform> _layers;
            private UIModule module;
            private Empty4Raycast raycast;
            private BaseRaycaster raycast_unity;
            private bool _force_show_raycast;
            private List<string> layerNames;
            public LayerPart(UIModule module)
            {
                this.module = module;
                _panelOrders = new Dictionary<string, List<UIPanel>>();
                _layers = new Dictionary<string, RectTransform>();
            }
            private RectTransform CreateLayer(string layerName, Transform parent)
            {
                GameObject go = new GameObject(layerName);
                RectTransform rect = go.AddComponent<RectTransform>();
                //var group = rect.gameObject.AddComponent<CanvasGroup>();
                rect.SetParent(parent);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.localPosition = Vector3.zero;
                rect.sizeDelta = Vector3.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
     
                _layers.Add(layerName, rect);

                return rect;
            }
            public void CreateLayers(Canvas canvas)
            {
                layerNames = module.GetLayerNames();
                foreach (var item in layerNames)
                    CreateLayer(item, canvas.transform);
                raycast_unity = canvas.GetComponent<BaseRaycaster>();

                //CreateLayer(UILayerData.item_layer, parent);
                //SwitchLayerVisible(UILayerData.item_layer, false);
                if (raycast_unity == null)
                {
                    var ray = CreateLayer(UILayerData.rayCast_layer, canvas.transform);
                    raycast = ray.gameObject.AddComponent<Empty4Raycast>();
                }
                AcceptRayCast();
            }

            public RectTransform GetLayerTransform(string layer) => _layers[layer];


            public void RefuseRayCast()
            {
                if (raycast_unity == null)
                    raycast.raycastTarget = true;
                else
                    raycast_unity.enabled = false;
            }

            public void AcceptRayCast()
            {
                if (_force_show_raycast) return;
                if (raycast_unity == null)
                    raycast.raycastTarget = false;
                else
                    raycast_unity.enabled = true;
            }
            public void ForceRefuseRayCast()
            {
                _force_show_raycast = true;
                RefuseRayCast();
            }
            public void ForceAcceptRayCast()
            {
                _force_show_raycast = false;
                AcceptRayCast();
            }
            public void Clear()
            {
                _layers.Clear();
            }

            public void SetAsLastOrder(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);
                var list = GetPanelsByLayerName(layerName);
                list.Remove(panel);
                list.Add(panel);
                panel.SetSiblingIndex(list.Count);
            }
            public void SetOrder(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);
                var list = GetPanelsByLayerName(layerName);
                SetAsLastOrder(path, panel);

            }
            public UIPanel GetTopShowPanel(int layer)
            {
                var layerName = module.GetLayerName(layer);
                var list = FindPanelsByLayerName(layerName);
                if (list == null) return null;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].show)
                    {
                        return list[i];
                    }
                }
                return null;
            }
            public UIPanel GetTopPanel(int layer)
            {
                var layerName = module.GetLayerName(layer);
                var list = FindPanelsByLayerName(layerName);
                if (list == null || list.Count == 0) return null;
                return list[list.Count - 1];
            }
            public void RemovePanel(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);
                FindPanelsByLayerName(layerName)?.Remove(panel);
            }

            public void LegalLayerPanelVisible()
            {
                bool visible = true;

                for (int i = layerNames.Count - 1; i >= 0; i--)
                {
                    var layerName = layerNames[i];

                    var list = FindPanelsByLayerName(layerName);
                    if (list == null) continue;
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        var panel = list[j];
                        var _visible = visible && panel.show;
                        if (panel.SwitchVisible(_visible))
                            module.CallPanelVisibleChange(panel, _visible);
                        if (_visible)
                        {
                            var path = panel.GetPath();
                            if (module.GetPanelFullScreen(path))
                                visible = false;
                        }
                    }




                }
            }

        }
    }
}
