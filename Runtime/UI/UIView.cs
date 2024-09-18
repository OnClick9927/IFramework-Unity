/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/

namespace IFramework.UI.MVC
{

    public abstract class UIView : GameObjectView, IViewEventHandler
    {     
        protected abstract void OnLoad();
        protected abstract void OnShow();
        protected abstract void OnHide();
        protected abstract void OnClose();

        void IViewEventHandler.OnLoad() => OnLoad();

        void IViewEventHandler.OnShow() => OnShow();

        void IViewEventHandler.OnHide() => OnHide();

        void IViewEventHandler.OnClose() => OnClose();
    }
}
