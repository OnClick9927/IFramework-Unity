/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *History:        2018.11--
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace IFramework.UI
{
    /// <summary>
    /// ui加载器
    /// </summary>
    public abstract class UIAsset
    {
        private UILayerData layer;
        private PanelCollection collection;

        protected UIAsset(UILayerData layer, PanelCollection collection)
        {
            this.layer = layer;
            this.collection = collection;
            if (layer == null)
                Log.FE("UIAsset layer can not be null");
            if (collection != null)
            {
                collection.ListToMap();
            }
        }
        public abstract UIPanel LoadPanel(RectTransform parent, string path);
        public abstract bool LoadPanelAsync(LoadPanelAsyncOperation op);
        public virtual void DestroyPanel(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
        public virtual Canvas GetCanvas() { return null; }

        public PanelCollection.Data GetData(string path) => collection?.GetData(path);
        //public bool GetIgnoreOrder() => layer.ignoreOrder;
        public List<string> GetLayerNames() => layer.GetLayerNames();
        public virtual int GetPanelLayer(string path)
        {
            var data = GetData(path);
            if (data != null)
                return data.layer;
            return 0;
        }
        //public virtual int GetPanelLayerOrder(string path)
        //{
        //    var data = GetData(path);
        //    if (data != null)
        //        return data.order;
        //    return 0;
        //}
        public virtual bool GetPanelFullScreen(string path)
        {
            var data = GetData(path);
            if (data != null)
                return data.fullScreen;
            return false;
        }
        public virtual string GetLayerName(int layer) => this.layer.GetLayerName(layer);
        public virtual int LayerNameToIndex(string layerName) => this.layer.LayerNameToIndex(layerName);
    }
}
