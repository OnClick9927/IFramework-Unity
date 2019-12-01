/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-10-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(IsShowAttribute))]
    public class IsShowDrawer : PropertyDrawer
    {
        private IsShowAttribute Attr { get { return this.attribute as IsShowAttribute; } }
        private float Height;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty p = property.serializedObject.FindProperty(Attr.checkProperty);
            if (p.type=="bool" && p.boolValue==false)
                Height = 0;
            else
                Height= base.GetPropertyHeight(property, label);
            return Height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Height == 0) return;
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
