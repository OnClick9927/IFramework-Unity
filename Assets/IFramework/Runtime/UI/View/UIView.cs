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

        void IUIView.OnLoad() => OnLoad();

        void IUIView.OnShow() => OnShow();

        void IUIView.OnHide() => OnHide();

        void IUIView.OnClose()
        {
            DisposeUIEvents();
            DisposeEvents();
            OnClose();
        }
    }
}
