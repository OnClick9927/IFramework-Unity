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
    public interface IUIDelegate
    {
        void OnLayerTopChange(UILayer layer, string top);
        void OnLayerTopVisibleChange(UILayer layer, string v);
        void OnPanelClose(string path);
        void OnPanelHide(string path);
        void OnPanelLoad(string pannelName);
        void OnPanelShow(string pannelName);

    }
}
