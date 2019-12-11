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
    [CustomGUIElement(typeof(ParentImageElement))]
    public class ParentImageElementEditor : ParentGUIElementEditor
    {
        private ParentImageElement ele { get { return element as ParentImageElement; } }
        private bool insFold = true;
        private GUIStyleDesign imageStyleDrawer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (imageStyleDrawer == null)
                imageStyleDrawer = new GUIStyleDesign(ele.imageStyle, "Image Style");
            insFold = FormatFoldGUI(insFold, "Image", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref ele.image, false);
            imageStyleDrawer.OnGUI();
        }
    }
}
