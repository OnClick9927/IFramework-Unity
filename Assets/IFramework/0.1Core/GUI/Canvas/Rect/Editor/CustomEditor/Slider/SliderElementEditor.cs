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
    [CustomGUIElement(typeof(SliderElement))]
    public class SliderElementEditor : GUIElementEditor
    {
        private SliderElement slider { get { return element as SliderElement; } }
        private bool insFold = true;
        private GUIStyleEditor thumbDrawer;
        private GUIStyleEditor sliderDrawer;



        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (sliderDrawer == null)
                sliderDrawer = new GUIStyleEditor(slider.slider, "Slider Style");
            if (thumbDrawer == null)
                thumbDrawer = new GUIStyleEditor(slider.thumb, "Thumb Style");
            insFold = FormatInspectorHeadGUI(insFold, "Slider", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.FloatField("Value", ref slider.value)
                .FloatField("Start Value", ref slider.startValue)
                .FloatField("End Value", ref slider.endValue);
            sliderDrawer.OnGUI();
            thumbDrawer.OnGUI();
        }
    }
}
