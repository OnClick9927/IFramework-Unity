/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEngine;

namespace IFramework.UI
{
    public class PanelAsyncOperation : AsyncTask
    {
        public string path;
        protected override void ResetFromPool()
        {
            base.ResetFromPool();
            path = string.Empty;
        }
        protected override void BackToPool() => SetToPool(this);
        internal new static PanelAsyncOperation CreateFromPool() => AllocatePoolTask<PanelAsyncOperation>();


    }


    public class LoadPanelAsyncOperation : AsyncTask<UIPanel>
    {
        internal new static LoadPanelAsyncOperation CreateFromPool() => AllocatePoolTask<LoadPanelAsyncOperation>();

        protected override void ResetFromPool()
        {
            base.ResetFromPool();
            parent = null;
            show = null;
        }
        protected override void BackToPool() => SetToPool(this);
        public string path => show?.path;
        public RectTransform parent;
        internal PanelAsyncOperation show;
    }




}
