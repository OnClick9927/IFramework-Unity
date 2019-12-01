/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(SetPropertyAttribute))]
    internal class SetPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            SetPropertyAttribute setProperty = attribute as SetPropertyAttribute;
            if (EditorGUI.EndChangeCheck())
            {
                setProperty.IsDirty = true;
            }
            else if (setProperty.IsDirty)
            {
                object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);
                Type type = parent.GetType();
                PropertyInfo pi = type.GetProperty(setProperty.Name);
                if (pi == null)
                    Debug.LogError("Invalid property name: " + setProperty.Name + "\nCheck your [SetProperty] attribute");
                else
                    pi.SetValue(parent, fieldInfo.GetValue(parent), null);
                setProperty.IsDirty = false;
            }
        }

        private object GetParentObjectOfProperty(string path, object obj)
        {
            string[] fields = path.Split('.');
            if (fields.Length == 1) return obj;
            FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            obj = fi.GetValue(obj);
            return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
        }
    }
}
