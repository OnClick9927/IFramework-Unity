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
    partial class UIModule
    {
        private class LayerPart
        {
            private Dictionary<string, List<UIPanel>> _panelOrders;
            private Dictionary<string, RunTimeUILayerData> _layers;
            private UIModule module;
            private Empty4Raycast raycast;
            private bool _force_show_raycast;
            private List<string> layerNames;
            public LayerPart(UIModule module)
            {
                this.module = module;
                _panelOrders = new Dictionary<string, List<UIPanel>>();
                _layers = new Dictionary<string, RunTimeUILayerData>();
            }
            private RunTimeUILayerData CreateLayer(string layerName, Transform parent)
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
                var data = new RunTimeUILayerData()
                {
                    //group = group,
                    rect = rect,
                    parent = parent,
                    name = layerName,
                };
                _layers.Add(layerName, data);

                return data;
            }
            public void CreateLayers(Transform parent)
            {
                layerNames = module.GetLayerNames();
                foreach (var item in layerNames)
                    CreateLayer(item, parent);
                //CreateLayer(UILayerData.item_layer, parent);
                //SwitchLayerVisible(UILayerData.item_layer, false);
                var ray = CreateLayer(UILayerData.rayCast_layer, parent);
                raycast = ray.rect.gameObject.AddComponent<Empty4Raycast>();
                HideRayCast();
            }
            //private void SwitchLayerVisible(string layerName, bool visible)
            //{
            //    var layer = GetRTLayerData(layerName);
            //    layer.group.alpha = visible ? 1 : 0;
            //    layer.group.blocksRaycasts = visible ? true : false;
            //    layer.group.interactable = visible ? true : false;
            //}
            public RunTimeUILayerData GetRTLayerData(string layer) => _layers[layer];


            public void ShowRayCast() => raycast.raycastTarget = true;
            public void HideRayCast()
            {
                if (_force_show_raycast) return;
                raycast.raycastTarget = false;
            }
            public void ForceShowRayCast()
            {
                _force_show_raycast = true;
                ShowRayCast();
            }
            public void ForceHideRayCast()
            {
                _force_show_raycast = false;
                HideRayCast();
            }
            public void Clear()
            {
                _layers.Clear();
            }

            public void SetAsLastOrder(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);

                if (!_panelOrders.ContainsKey(layerName))
                    _panelOrders.Add(layerName, new List<UIPanel>());
                var list = _panelOrders[layerName];
                list.Remove(panel);
                list.Add(panel);
                panel.SetSiblingIndex(list.Count);
            }
            public void SetOrder(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);
                if (!_panelOrders.ContainsKey(layerName))
                    _panelOrders.Add(layerName, new List<UIPanel>());
                var list = _panelOrders[layerName];
                if (module.GetIgnoreOrder())
                {
                    SetAsLastOrder(path, panel);
                }
                else
                {
                    if (list.Contains(panel)) return;
                    int order = module.GetPanelLayerOrder(path);
                    bool instert = false;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (module.GetPanelLayerOrder(list[i].GetPath()) > order)
                        {
                            var sbindex = list[i].GetSiblingIndex();
                            panel.SetSiblingIndex(sbindex);
                            list.Insert(sbindex, panel);
                            instert = true;
                            break;
                        }
                    }
                    if (!instert)
                        list.Add(panel);

                }

            }
            public UIPanel GetTopShowPanel(int layer)
            {
                var layerName = module.GetLayerName(layer);

                if (!_panelOrders.ContainsKey(layerName))
                    return null;
                var list = _panelOrders[layerName];
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
                if (!_panelOrders.ContainsKey(layerName))
                    return null;
                var list = _panelOrders[layerName];
                if (list.Count == 0) return null;
                return list[list.Count - 1];
            }
            public void RemovePanel(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);

                var list = _panelOrders[layerName];
                list.Remove(panel);
            }

            public void LegalLayerPanelVisible()
            {
                bool visible = true;

                for (int i = layerNames.Count - 1; i >= 0; i--)
                {
                    var layerName = layerNames[i];

                    if (_panelOrders.TryGetValue(layerName, out var list))
                    {
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
}
