/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework.UI
{
    interface IUIView
    {
        void OnLoad();
        void OnShow();
        void OnHide();
        void OnClose();

        void OnBecameVisible();
        void OnBecameInvisible();
        void OnHideAsync(PanelAsyncOperation operation);
        void OnCloseAsync(PanelAsyncOperation operation);
    }
    public abstract class UIView : WidgetView, IUIView
    {
        public UIPanel panel { get; private set; }

        internal void SetPanel(UIPanel panel)
        {
            this.panel = panel;
            SetGameObject(panel.gameObject);
        }
        protected abstract void OnLoad();
        protected abstract void OnShow();
        protected abstract void OnHide();
        protected abstract void OnClose();

        protected virtual void OnBecameVisible() { }
        protected virtual void OnBecameInvisible() { }

        protected virtual void OnHideAsync(PanelAsyncOperation operation) { }
        protected virtual void OnCloseAsync(PanelAsyncOperation operation) { }
        protected virtual void AfterOnClose() { }

        void IUIView.OnLoad() => OnLoad();

        void IUIView.OnShow() => OnShow();

        void IUIView.OnHide() => OnHide();
        void IUIView.OnHideAsync(PanelAsyncOperation operation) => OnHideAsync(operation);
        void IUIView.OnCloseAsync(PanelAsyncOperation operation) => OnCloseAsync(operation);
        void IUIView.OnClose()
        {
            OnClose();
            ClearFields();
            AfterOnClose();
        }

        void IUIView.OnBecameVisible() => OnBecameVisible();

        void IUIView.OnBecameInvisible() => OnBecameInvisible();


    }

}
