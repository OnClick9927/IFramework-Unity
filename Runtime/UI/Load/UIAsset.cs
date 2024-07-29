/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *History:        2018.11--
*********************************************************************************/

using UnityEngine;

namespace IFramework.UI
{
    /// <summary>
    /// ui加载器
    /// </summary>
    public abstract class UIAsset
    {

        private PanelCollection collection;

        protected UIAsset(PanelCollection collection)
        {
            this.collection = collection;
            if (collection != null)
            {
                collection.ListToMap();
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract UIPanel LoadPanel(RectTransform parent, string name);

        public abstract bool LoadPanelAsync(string path, LoadPanelAsyncOperation op);

        public abstract bool LoadItemAsync(string path, LoadItemAsyncOperation op);

        public virtual void ReleaseItemAsset(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
        public virtual void DestroyPanel(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
        public virtual Canvas GetCanvas() { return null; }

        public PanelCollection.Data GetData(string path) => collection?.GetData(path);
        public virtual UILayer GetPanelLayer(string path)
        {
            var data = GetData(path);
            if (data != null)
                return data.layer;
            return UILayer.Background;
        }
        public virtual int GetPanelLayerOrder(string path)
        {
            var data = GetData(path);
            if (data != null)
                return data.order;
            return 0;
        }
        public virtual bool GetPanelHideScene(string path)
        {
            var data = GetData(path);
            if (data != null)
                return data.fullScreen;
            return false;
        }
    }
}
