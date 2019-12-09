/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.GUITool.RectDesign
{
    [CustomGUIElement(typeof(ToolBarElement))]
    public class ToolBarElementEditor : GUIElementEditor
    {
        private ToolBarElement toolbar { get { return element as ToolBarElement; } }
        private bool insFold = true;
        private GUIStyleEditor styleDrawer;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (styleDrawer == null)
                styleDrawer = new GUIStyleEditor(toolbar.style, "ToolBar Style");
            insFold = FormatInspectorHeadGUI(insFold, "ToolBar Element", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.IntField("Value", ref toolbar.value);
            styleDrawer.OnGUI();
        }
    }
}
