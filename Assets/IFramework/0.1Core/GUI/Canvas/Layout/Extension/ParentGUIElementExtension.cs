/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.GUITool.LayoutDesign
{
    public static class ParentGUIElementExtension
    {
        public static T Element<T>(this T self, GUIElement element) where T : ParentGUIElement
        {
            element.parent = self;
            GUICanvas canvas = element.root as GUICanvas;
            if (canvas != null)
                canvas.TreeChange();
            return self;
        }
    }

}
