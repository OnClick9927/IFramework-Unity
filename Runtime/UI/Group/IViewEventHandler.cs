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
    public interface IViewEventHandler
    {
        void OnLoad();
        void OnShow();
        void OnHide();
        void OnClose();
    }
}
