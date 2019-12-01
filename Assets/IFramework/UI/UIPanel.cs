/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework
{
    public enum UIPanelLayer
    {
        BGBG,               //非常BG
        Background,         //BG
        AnimationUnderPage, //背景动画
        Common,             //普通
        AnimationOnPage,    //上层动画
        PopUp,              //弹框
        Guide,              //引导
        Toast,              //对话框
        Top,                //Top
        TopTop,             //非常Top
    }
    public class UIEventArgs : IEventArgs
    {
        public UIPanel StackTop;
        public UIPanel PressOne;
        public UIPanel PopOne;
        public bool isInspectorBtn;
    }

    public interface IUIPanel
    {
        UIPanelLayer PanelLayer { get; set; }
        string PanelName { get; set; }
        void OnLoad(UIEventArgs arg);
        void OnTop(UIEventArgs arg);
        void OnPress(UIEventArgs arg);
        void OnPop(UIEventArgs arg);
        void OnCacheClear(UIEventArgs arg);
    }
    public abstract class UIPanel : MonoBehaviour, IUIPanel
    {
        public UIPanelLayer PanelLayer { get; set; }
        public virtual string PanelName  { get { return this.name; } set { this.name = value; } }

        void IUIPanel.OnCacheClear(UIEventArgs arg) { OnCacheClear(arg); }
        void IUIPanel.OnLoad(UIEventArgs arg) { OnLoad(arg); }
        void IUIPanel.OnPop(UIEventArgs arg) { OnPop(arg); }
        void IUIPanel.OnPress(UIEventArgs arg) { OnPress(arg); }
        void IUIPanel.OnTop(UIEventArgs arg) { OnTop(arg); }

        protected abstract void OnCacheClear(UIEventArgs arg);
        protected abstract void OnLoad(UIEventArgs arg);
        protected abstract void OnPop(UIEventArgs arg);
        protected abstract void OnPress(UIEventArgs arg);
        protected abstract void OnTop(UIEventArgs arg);
    }
}
