/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{

    [CustomGUIElement(typeof(HorizontalSlider))]
    public class HorizontalSliderEditor : SliderElementEditor
    {
        private HorizontalSlider slider { get { return element as HorizontalSlider; } }

        public override void OnSceneGUI(Action child)
        {
            if (!slider.active) return;
            BeginGUI();
            slider.value = GUILayout.HorizontalSlider(slider.value, slider.startValue, slider.endValue, slider.slider, slider.thumb, CalcGUILayOutOptions()); ;
            slider.position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
