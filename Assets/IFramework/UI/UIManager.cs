/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace IFramework
{
    [MonoSingletonPath("IFramework/UIManager")]
    [HelpURL("https://gitee.com/OnClick9927/Learning___IFramework")]
    public class UIManager : MonoSingletonPropertyClass<UIManager>
    {
        public delegate UIPanel LoadDel(Type type, string path, string name, UIPanelLayer layer, UIEventArgs arg);
        private List<LoadDel> UILoader;
        public static int LoaderCount { get { return Instance.UILoader.Count; } }
        public static void AddLoader(LoadDel UILoader)
        {
            Instance.UILoader.Add(UILoader);
        }
        public static UIPanel LoadUIPanel(Type type, string path, UIPanelLayer layer, string name, UIEventArgs arg)
        {
            UIPanel ui = default(UIPanel);
            if (Instance.UILoader == null || LoaderCount == 0) Log.E("NO UILoader");
            else
            {
                for (int i = 0; i < Instance.UILoader.Count; i++)
                {
                    var result = Instance.UILoader[i].Invoke(type, path, name, layer, arg);
                    if (result != null)
                    {
                        ui = result;
                        break;
                    }
                }
            }
            if (ui == null)
            {
                Log.E(string.Format("NO ui Type: {0}  Path: {1}  Layer: {2}  Name: {3}", type, path, layer, name));
                return ui;
            }
            ui = Instantiate(ui);
            ui.PanelLayer = layer;
            Instance.SetParent(ui);
            (ui as IUIPanel).OnLoad(arg);
            ui.PanelName = name;
            if (!Instance.panelDic.ContainsKey(type)) Instance.panelDic.Add(type, new List<UIPanel>()); Instance.panelDic[type].Add(ui);
            return ui;
        }
        public static T LoadUIPanel<T>(string path, string name, UIPanelLayer layer, UIEventArgs arg) where T : UIPanel
        {
            return (T)LoadUIPanel(typeof(T), path, layer, name, arg);
        }


        private Dictionary<Type, List<UIPanel>> panelDic;
        public Stack<UIPanel> UIStack;
        public Stack<UIPanel> UICache;
        public static UIPanel StackTop { get { return Instance.UIStack.Peek(); } }
        public static int StackCount { get { return Instance.UIStack.Count; } }
        public static int CacheCount { get { return Instance.UICache.Count; } }
        public static bool IsInStack(UIPanel panel) { return Instance.UIStack.Contains(panel); }
        public static bool IsInCache(UIPanel panel) { return Instance.UICache.Contains(panel); }
        public static bool IsInUse(UIPanel panel) { return IsInCache(panel) || IsInStack(panel); }

        public static UIPanel Peek() { return Instance.UIStack.Peek(); }
        public static UIPanel CachePeek() { return Instance.UICache.Peek(); }

        public static void Push(UIPanel ui, UIEventArgs arg)
        {
            arg.PopOne = null;
            arg.StackTop = null;
            arg.PressOne = null;
            if (StackCount != 0) arg.PressOne = Peek();
            Instance.UIStack.Push(ui);
            arg.StackTop = Peek();

            if (arg.PressOne != null) (arg.PressOne as IUIPanel).OnPress(arg);
            (arg.StackTop as IUIPanel).OnTop(arg);
            if (Instance.UICache.Count > 0) ClearCache(arg);

        }
        public static void GoForWard(UIEventArgs arg)
        {
            if (CacheCount == 0) return;
            var ui = Instance.UICache.Pop();
            arg.PopOne = null;
            arg.StackTop = null;
            arg.PressOne = null;
            if (StackCount != 0) arg.PressOne = Peek();
            Instance.UIStack.Push(ui);
            arg.StackTop = Peek();

            if (arg.PressOne != null) (arg.PressOne as IUIPanel).OnPress(arg);
            (arg.StackTop as IUIPanel).OnTop(arg);
        }
        public static void GoBack(UIEventArgs arg)
        {
            if (StackCount == 0) return;
            arg.PopOne = null;
            arg.StackTop = null;
            arg.PressOne = null;
            if (StackCount > 0)
            {
                var info = Instance.UIStack.Pop();
                Instance.UICache.Push(info);
                arg.PopOne = info;
            }
            if (StackCount > 0) arg.StackTop = Peek();

            if (arg.PopOne != null) (arg.PopOne as IUIPanel).OnPop(arg);
            if (arg.StackTop != null) (arg.StackTop as IUIPanel).OnTop(arg);

        }
        public static void ClearCache(UIEventArgs arg)
        {
            while (Instance.UICache.Count != 0)
            {
                UIPanel p = Instance.UICache.Pop();
                if (!IsInUse(p))
                {
                    Instance.panelDic[p.GetType()].Remove(p);
                    (p as IUIPanel).OnCacheClear(arg);
                }
            }
        }

        public static UIPanel Get(Type type, string path, UIPanelLayer layer, string name, UIEventArgs arg, bool loadNew = false)
        {
            if (!type.IsSubclassOf(typeof(UIPanel)))
            {
                Log.E(string.Format("{0} is Not UIpanel", type));
                return default(UIPanel);
            }
            //if (UICache.Count > 0) ClearCache(arg);

            if (loadNew || !Instance.panelDic.ContainsKey(type))
            {
                UIPanel ui = LoadUIPanel(type, path, layer, name, arg);
                Push(ui, arg);
                return ui;
            }
            else
            {
                UIPanel ui = Instance.panelDic[type].Find((p) => { return p.GetType() == type && p.PanelLayer == layer && p.PanelName == name; });
                if (ui == null) return Get(type, path, layer, name, arg, true);
                else
                {
                    Push(ui, arg); return ui;
                }
            }
        }
        public static T Get<T>(string path, UIPanelLayer layer, string name, UIEventArgs arg, bool loadNew = false) where T : UIPanel
        {
            return (T)Get(typeof(T), path, layer, name, arg, loadNew);
        }


        private Transform UIRoot;

        private Transform BGBG;
        private Transform Background;
        private Transform AnimationUnderPage;
        private Transform Common;
        private Transform AnimationOnPage;
        private Transform PopUp;
        private Transform Guide;
        private Transform Toast;
        private Transform Top;
        private Transform TopTop;
        private Transform UICamera;
        public static void SetCamera(Camera ca, bool isLast = true, int index = -1)
        {
            Instance.UICamera.SetChildWithIndex(ca.transform, !isLast ? index : Instance.UICamera.childCount);
        }
        public void SetParent(UIPanel ui, bool isLast = true, int index = -1)
        {
            switch (ui.PanelLayer)
            {
                case UIPanelLayer.BGBG:
                    BGBG.SetChildWithIndex(ui.transform, !isLast ? index : BGBG.childCount);
                    break;
                case UIPanelLayer.Background:
                    Background.SetChildWithIndex(ui.transform, !isLast ? index : Background.childCount);
                    break;
                case UIPanelLayer.AnimationUnderPage:
                    AnimationUnderPage.SetChildWithIndex(ui.transform, !isLast ? index : AnimationUnderPage.childCount);
                    break;
                case UIPanelLayer.Common:
                    Common.SetChildWithIndex(ui.transform, !isLast ? index : Common.childCount);
                    break;
                case UIPanelLayer.AnimationOnPage:
                    AnimationOnPage.SetChildWithIndex(ui.transform, !isLast ? index : AnimationOnPage.childCount);
                    break;
                case UIPanelLayer.PopUp:
                    PopUp.SetChildWithIndex(ui.transform, !isLast ? index : PopUp.childCount);
                    break;
                case UIPanelLayer.Guide:
                    Guide.SetChildWithIndex(ui.transform, !isLast ? index : Guide.childCount);
                    break;
                case UIPanelLayer.Toast:
                    Toast.SetChildWithIndex(ui.transform, !isLast ? index : Toast.childCount);
                    break;
                case UIPanelLayer.Top:
                    Top.SetChildWithIndex(ui.transform, !isLast ? index : Top.childCount);
                    break;
                case UIPanelLayer.TopTop:
                    TopTop.SetChildWithIndex(ui.transform, !isLast ? index : TopTop.childCount);
                    break;
                default:
                    break;
            }
            ui.transform.LocalIdentity();
        }
        protected override void OnSingletonInit()
        {
            UIRoot = Instantiate(Resources.Load<GameObject>("UIRoot")).transform;
            UIRoot.name = "UIRoot";
            UIRoot.SetParent(transform);
            BGBG = UIRoot.transform.Find("BGBG");
            Background = UIRoot.transform.Find("Background");
            AnimationUnderPage = UIRoot.transform.Find("AnimationUnderPage");
            Common = UIRoot.transform.Find("Common");
            AnimationOnPage = UIRoot.transform.Find("AnimationOnPage");
            PopUp = UIRoot.transform.Find("PopUp");
            Guide = UIRoot.transform.Find("Guide");
            Toast = UIRoot.transform.Find("Toast");
            Top = UIRoot.transform.Find("Top");
            TopTop = UIRoot.transform.Find("TopTop");

            UICamera = UIRoot.transform.Find("UICamera");

            panelDic = new Dictionary<Type, List<UIPanel>>();
            UIStack = new Stack<UIPanel>();
            UICache = new Stack<UIPanel>();
            UILoader = new List<LoadDel>();
        }

        public override void Dispose()
        {
            panelDic.Clear();
            UIStack.Clear();
            UICache.Clear();
            UILoader.Clear();
        }
        private void OnDestroy()
        {
             Dispose();
        }
    }


}
