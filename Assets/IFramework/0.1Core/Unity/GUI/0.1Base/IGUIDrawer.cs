/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.14f1
 *Date:           2019-10-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.GUITool
{
    public static class IGUIDrawerExtension
    {
        static IGUIDrawerExtension()
        {
            r = new RectDrawer();
            l = new layoutDrawer();
        }
        static RectDrawer r;
        static layoutDrawer l;
        private class RectDrawer : IRectGUIDrawer { }
        private class layoutDrawer : ILayoutGUIDrawer { }
        public static T Pan<T>(this T self, Action action) where T : IGUIDrawer
        {
            if (action != null)
            {
                action();
            }
            return self;
        }
        public static ILayoutGUIDrawer GetLayoutDrawer<T>(this T self) where T : IGUIDrawer
        {
            return l;
        }
        public static IRectGUIDrawer GetRectDrawer<T>(this T self) where T : IGUIDrawer
        {
            return r;
        }
    }
    public interface IGUIDrawer { }
    public interface IRectGUIDrawer : IGUIDrawer { }
    public interface ILayoutGUIDrawer : IGUIDrawer { }
}
