/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
namespace IFramework.GUITool
{
	public class GUIUtil
	{
        public static Color contentColor { get { return GUI.contentColor; } set { GUI.contentColor = value; } }
        public static Color backgroundColor { get { return GUI.backgroundColor; } set { GUI.backgroundColor = value; } }
        public static bool changed { get { return GUI.changed; } set { GUI.changed = value; } }
        public static bool enabled { get { return GUI.enabled; } set { GUI.enabled = value; } }
        public static Color color { get { return GUI.color; } set { GUI.color = value; } }
        public static int depth { get { return GUI.depth; } set { GUI.depth = value; } }
        public static string tooltip { get { return GUI.tooltip; } set { GUI.tooltip = value; } }
        public static GUISkin skin { get { return GUI.skin; } set { GUI.skin = value; } }
        public static Matrix4x4 matrix { get { return GUI.matrix; } set { GUI.matrix = value; } }

        public static  string systemCopyBuffer { get { return GUIUtility.systemCopyBuffer; } set { GUIUtility.systemCopyBuffer = value; } }
        public static bool hasModalWindow { get { return GUIUtility.hasModalWindow; }  }
        public static int hotControl { get { return GUIUtility.hotControl; } set { GUIUtility.hotControl = value; } }
        public static int keyboardControl { get { return GUIUtility.keyboardControl; } set { GUIUtility.keyboardControl = value; } }



        public static GUILayoutOption Width(float width)
        {
            return GUILayout.Width(width);
        }
        public static GUILayoutOption Height(float height)
        {
            return GUILayout.Height(height);
        }
        public static GUILayoutOption MaxHeight(float maxHeight)
        {
            return GUILayout.MaxHeight(maxHeight);
        }
        public static GUILayoutOption MaxWidth(float maxWidth)
        {
            return GUILayout.MaxWidth(maxWidth);
        }
        public static GUILayoutOption MinHeight(float minHeight)
        {
            return GUILayout.MinHeight(minHeight);
        }
        public static GUILayoutOption MinWidth(float minWidth)
        {
            return GUILayout.MinWidth(minWidth);
        }
        public static GUILayoutOption ExpandHeight(bool expand)
        {
            return GUILayout.ExpandHeight(expand);
        }
        public static GUILayoutOption ExpandWidth(bool expand)
        {
            return GUILayout.ExpandWidth(expand);
        }
        public static Rect AlignRectToDevice(Rect rect)
        {
            return GUIUtility.AlignRectToDevice(rect);
        }
        public static Rect AlignRectToDevice(Rect rect, out int widthInPixels, out int heightInPixels)
        {
            return GUIUtility.AlignRectToDevice(rect, out  widthInPixels, out  heightInPixels);
        }
        public static void ExitGUI()
        {
            GUIUtility.ExitGUI();
        }
        public static int GetControlID(FocusType focus, Rect position)
        {
            return GUIUtility.GetControlID(focus,position);
        }
        public static int GetControlID(GUIContent contents, FocusType focus, Rect position)
        {
            return GUIUtility.GetControlID(contents,focus, position);
        }
        public static int GetControlID(int hint, FocusType focus)
        {
            return GUIUtility.GetControlID(hint, focus);
        }
        public static int GetControlID(FocusType focus)
        {
            return GUIUtility.GetControlID( focus);
        }
        public static int GetControlID(int hint, FocusType focusType, Rect rect)
        {
            return GUIUtility.GetControlID(hint, focusType, rect);
        }
        public static int GetControlID(GUIContent contents, FocusType focus)
        {
            return GUIUtility.GetControlID(contents, focus);
        }
        public static object GetStateObject(Type t, int controlID)
        {
            return GUIUtility.GetStateObject(t, controlID);
        }
        public static Vector2 GUIToScreenPoint(Vector2 guiPoint)
        {
            return GUIUtility.GUIToScreenPoint(guiPoint);
        }
        public static object QueryStateObject(Type t, int controlID)
        {
            return GUIUtility.QueryStateObject(t, controlID);
        }
        public static void RotateAroundPivot(float angle, Vector2 pivotPoint)
        {
            GUIUtility.RotateAroundPivot(angle, pivotPoint);
        }
        public static void ScaleAroundPivot(Vector2 scale, Vector2 pivotPoint)
        {
            GUIUtility.ScaleAroundPivot(scale, pivotPoint);
        }
        public static Vector2 ScreenToGUIPoint(Vector2 screenPoint)
        {
            return GUIUtility.GUIToScreenPoint(screenPoint);
        }
        public static Rect ScreenToGUIRect(Rect screenRect)
        {
            return GUIUtility.ScreenToGUIRect(screenRect);
        }


         

        public static Rect GetAspectRect(float aspect, GUIStyle style)
        {
            return GUILayoutUtility.GetAspectRect(aspect, style);
        }
        public static Rect GetAspectRect(float aspect)
        {
            return GUILayoutUtility.GetAspectRect(aspect);
        }
        public static Rect GetAspectRect(float aspect, params GUILayoutOption[] options)
        {
            return GUILayoutUtility.GetAspectRect(aspect, options);
        }
        public static Rect GetAspectRect(float aspect, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayoutUtility.GetAspectRect(aspect, style,options);
        }
        public static Rect GetLastRect()
        {
            return GUILayoutUtility.GetLastRect();
        }
        public static Rect GetRect(float width, float height, params GUILayoutOption[] options)
        {
            return GUILayoutUtility.GetRect(width,height,options);
        }
        public static Rect GetRect(float width, float height)
        {
            return GUILayoutUtility.GetRect(width, height);
        }
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight)
        {
            return GUILayoutUtility.GetRect(minWidth, maxWidth, minHeight,maxHeight);
        }
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style)
        {
            return GUILayoutUtility.GetRect(minWidth, maxWidth, minHeight, maxHeight,style);
        }
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, params GUILayoutOption[] options)
        {
            return GUILayoutUtility.GetRect(minWidth, maxWidth, minHeight, maxHeight,options);
        }
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayoutUtility.GetRect(minWidth, maxWidth, minHeight, maxHeight,style,options);
        }
        public static Rect GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayoutUtility.GetRect(content,style,options);
        }
        public static Rect GetRect(GUIContent content, GUIStyle style)
        {
            return GUILayoutUtility.GetRect(content, style);
        }
        public static Rect GetRect(float width, float height, GUIStyle style)
        {
            return GUILayoutUtility.GetRect(width, height, style);
        }
        public static Rect GetRect(float width, float height, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayoutUtility.GetRect(width, height, style,options);
        }
       
	}
}
