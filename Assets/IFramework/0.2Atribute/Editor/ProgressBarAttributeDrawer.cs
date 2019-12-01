/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    internal class ProgressBarAttributeDrawer : PropertyDrawer
    {
        private const string BarTextStyle =  "ProgressBarText";
        private GUIStyle barText;
        private GUIStyle BarText
        {
            get {
                if (barText==null)
                {
                    barText = new GUIStyle(BarTextStyle);
                    barText.alignment = TextAnchor.MiddleCenter;
                }
                return barText;
            }
        }
        private ProgressBarAttribute Bar { get { return attribute as ProgressBarAttribute; } }
        private float BarHeight
        {
            get
            {
                var style = new GUIStyle("ProgressBarBar");
                var content = new GUIContent(Bar.Text);
                return Mathf.Max(style.CalcHeight(content, Screen.width - 53), Bar.MinbarHeight);
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (Bar.ShowSlider)
                return base.GetPropertyHeight(property, label) + BarHeight;
            return BarHeight;

        }
        private Rect barRect;
        private Rect SliderRect;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Bar.ShowSlider)
            {
                barRect = new Rect(position.x,
                                          position.y + position.height - BarHeight,
                                          position.width,
                                          BarHeight).Zoom(  AnchorType.MiddleCenter,new Vector2(-10, -5));
                SliderRect = new Rect(position.x,
                                                position.y,
                                                position.width,
                                                position.height - BarHeight);
            }
            else
            {
                barRect = position.Zoom(AnchorType.MiddleCenter, new Vector2(-10, -5));
            }
           
            Eve(barRect, property);
            GUI.color = Bar.color;
            if (property.propertyType ==  SerializedPropertyType.Float)
            {
                if (Bar.ShowSlider)
                    EditorGUI.Slider(SliderRect,property,Bar.MinValue,Bar.MaxValue );

                EditorGUI.ProgressBar(barRect,(property.floatValue -Bar.MinValue)/ (Bar.MaxValue-Bar.MinValue),"");
                if (Event.current.type == EventType.Repaint)
                    if (string.IsNullOrEmpty(Bar.Text.Trim()))
                        BarText.Draw(barRect,
                            string.Format("{0}  [{1}/{2}]  :\t{3}  F", property.name.UpperFirst(), Bar.MinValue, Bar.MaxValue, property.floatValue),
                            false, false, false, false);
                    else BarText.Draw(barRect, Bar.Text, false, false, false, false);

            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (Bar.ShowSlider)
                    EditorGUI.IntSlider(SliderRect,property,(int)Bar.MinValue,(int)Bar.MaxValue);

                EditorGUI.ProgressBar(barRect,(float)(property.intValue - (int)Bar.MinValue) / ((int)Bar.MaxValue - (int)Bar.MinValue), "");
                if (Event.current.type == EventType.Repaint)
                    if (string.IsNullOrEmpty(Bar.Text.Trim()))
                        BarText.Draw(barRect, 
                            string.Format("{0}  [{1}/{2}]  :\t{3}  D", property.name.UpperFirst(),Bar.MinValue,Bar.MaxValue,property.intValue), 
                            false, false, false, false);
                    else BarText.Draw(barRect, Bar.Text, false, false, false, false);
            }
            
           
            GUI.color = Color.white;
        }

        private void Eve(Rect pos, SerializedProperty property)
        {
            Event e = Event.current;
            if (!pos.Zoom( AnchorType.MiddleCenter,new Vector2(4, 0)).Contains(e.mousePosition) || e.type!= EventType.MouseDrag) return;
            float width = e.mousePosition.x - pos.x;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = width / pos.width * (Bar.MaxValue - Bar.MinValue) + Bar.MinValue;
                if (e.mousePosition.x > pos.xMax)
                {
                    property.floatValue = Bar.MaxValue;
                }
                if (e.mousePosition.x < pos.xMin)
                {
                    property.floatValue = Bar.MinValue;
                }
            }
            else
            {
                float space = pos.width / (Bar.MaxValue - Bar.MinValue);
                float real = (property.intValue - Bar.MinValue) * space ;
                if (real < width - space)
                {
                    property.intValue++;
                }
                else if (real > width + space)
                    property.intValue--;
                if (e.mousePosition.x>pos.xMax)
                {
                    property.intValue = (int)Bar.MaxValue;
                }
                if (e.mousePosition.x < pos.xMin)
                {
                    property.intValue = (int)Bar.MinValue;
                }
            }
            e.Use();
        } 
       

    }
}
