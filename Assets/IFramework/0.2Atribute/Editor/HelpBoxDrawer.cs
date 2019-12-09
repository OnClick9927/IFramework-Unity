/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    internal class HelpBoxDrawer : PropertyDrawer
    {
        private float HelpBoxHeight
        {
            get
            {
                var style = new GUIStyle("HelpBox");
                var content = new GUIContent(HelpBoxAttribute.message);
                return Mathf.Max(style.CalcHeight(content, Screen.width - (HelpBoxAttribute.messageType != MessageType.None ? 53 : 21)), 40);
            }       
        }
        private HelpBoxAttribute HelpBoxAttribute { get { return attribute as HelpBoxAttribute; } }
        private float orgHeight;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            orgHeight = EditorGUI.GetPropertyHeight(property, label, true);
            if (!property.propertyPath.Contains("Array"))
                return orgHeight + HelpBoxHeight;
            return orgHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.propertyPath.Contains("Array"))
            {
                EditorGUI.HelpBox(new Rect(position.x,
                                            position.y,
                                            position.width,
                                            HelpBoxHeight),

                                            HelpBoxAttribute.message, (UnityEditor.MessageType)HelpBoxAttribute.messageType);
            }
            else
            {

            }
            EditorGUI.PropertyField(new Rect(position.x,
                                                position.y + position.height - orgHeight,
                                                position.width,
                                                orgHeight),

                                                property, label, true);

        }

       
       
    }
   
}
