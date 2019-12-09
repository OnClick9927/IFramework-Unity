/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(ResetAttribute))]

    internal class ResetDrawer : PropertyDrawer
    {
        public static readonly GUIContent ResetScale = new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Reset");

        private class Styles
        {
            public static GUIStyle ResetButton;

            static Styles()
            {
                ResetButton = new GUIStyle()
                {
                    margin = new RectOffset(0, 0, 2, 0),
                    fixedWidth = 15,
                    fixedHeight = 15
                };
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(property, label);
                if (!property.editable) return;
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        using (new EditorGUI.DisabledGroupScope(property.intValue == 0))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.intValue = 0;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Boolean:
                        using (new EditorGUI.DisabledGroupScope(property.boolValue == false))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.boolValue = false;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Float:
                        using (new EditorGUI.DisabledGroupScope(property.floatValue == 0))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.floatValue = 0;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.String:
                        using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(property.stringValue)))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.stringValue = string.Empty;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Color:
                        using (new EditorGUI.DisabledGroupScope(property.colorValue == Color.white))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.colorValue = Color.white;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.ObjectReference:
                        using (new EditorGUI.DisabledGroupScope(property.objectReferenceValue == null))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.objectReferenceValue = null;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Enum:
                        using (new EditorGUI.DisabledGroupScope(property.enumValueIndex == 0))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.enumValueIndex = 0;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Vector2:
                        using (new EditorGUI.DisabledGroupScope(property.vector2Value == Vector2.zero))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.vector2Value = Vector2.zero;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Vector3:
                        using (new EditorGUI.DisabledGroupScope(property.vector3Value == Vector3.zero))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.vector3Value = Vector3.zero;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Vector4:
                        using (new EditorGUI.DisabledGroupScope(property.vector4Value == Vector4.zero))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.vector4Value = Vector4.zero;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Rect:
                        using (new EditorGUI.DisabledGroupScope(property.rectValue == Rect.zero))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.rectValue = Rect.zero;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.ArraySize:
                        using (new EditorGUI.DisabledGroupScope(property.arraySize ==0))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.arraySize =0;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Quaternion:
                        using (new EditorGUI.DisabledGroupScope(property.quaternionValue == Quaternion.identity))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.quaternionValue = Quaternion.identity;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.ExposedReference:
                        using (new EditorGUI.DisabledGroupScope(property.exposedReferenceValue ==null))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.exposedReferenceValue = null;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Vector2Int:
                        using (new EditorGUI.DisabledGroupScope(property.vector2IntValue == Vector2Int.zero))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.vector2IntValue = Vector2Int.zero;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.Vector3Int:
                        using (new EditorGUI.DisabledGroupScope(property.vector3IntValue == Vector3Int.zero))
                            if (GUILayout.Button(ResetScale, Styles.ResetButton))
                            {
                                property.vector3IntValue = Vector3Int.zero;
                                property.serializedObject.ApplyModifiedProperties();
                                EditorWindow.mouseOverWindow.Repaint();
                            }
                        break;
                    case SerializedPropertyType.LayerMask:
                    case SerializedPropertyType.FixedBufferSize:
                    case SerializedPropertyType.RectInt:
                    case SerializedPropertyType.BoundsInt:
                    case SerializedPropertyType.Character:
                    case SerializedPropertyType.AnimationCurve:
                    case SerializedPropertyType.Bounds:
                    case SerializedPropertyType.Generic:
                    case SerializedPropertyType.Gradient:
                        break;
                }
               
      
            }
        }
    }
}
