/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/

namespace IFramework.UI
{

    public abstract class UIView : GameObjectView, IUIView
    {
        protected abstract void OnLoad();
        protected abstract void OnShow();
        protected abstract void OnHide();
        protected abstract void OnClose();
        protected abstract void OnEnable();
        protected abstract void OnDisable();

        void IUIView.OnLoad()
        {
            Log.FL($"UIView: {this.GetType().Name} OnLoad");
            OnLoad();
        }

        void IUIView.OnShow()
        {
            Log.FL($"UIView: {this.GetType().Name} OnShow");

            OnShow();
        }

        void IUIView.OnHide()
        {
            Log.FL($"UIView: {this.GetType().Name} OnHide");
            OnHide();
        }

        void IUIView.OnClose()
        {
            Log.FL($"UIView: {this.GetType().Name} OnClose");

            DisposeUIEvents();
            DisposeEvents();
            OnClose();
        }

        void IUIView.OnEnable()
        {
            Log.FL($"UIView: {this.GetType().Name} OnEnable");

            OnEnable();
        }

        void IUIView.OnDisable()
        {
            Log.FL($"UIView: {this.GetType().Name} OnDisable");

            OnDisable();
        }
    }
}
