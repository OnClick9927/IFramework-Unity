/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
    public class EditorGUIUtil : GUIUtil
    {
        public static bool textFieldHasSelection { get { return EditorGUIUtility.textFieldHasSelection; } }
        public static bool isProSkin { get { return EditorGUIUtility.isProSkin; } }
        public static Texture2D whiteTexture { get { return EditorGUIUtility.whiteTexture; } }
        public static float pixelsPerPoint { get { return EditorGUIUtility.pixelsPerPoint; } }
        public static float singleLineHeight { get { return EditorGUIUtility.singleLineHeight; } }
        public static float currentViewWidth { get { return EditorGUIUtility.currentViewWidth; } }
        public static float standardVerticalSpacing { get { return EditorGUIUtility.standardVerticalSpacing; } }

        public static string EsystemCopyBuffer { get { return EditorGUIUtility.systemCopyBuffer; } set { EditorGUIUtility.systemCopyBuffer = value; } }
        public static float fieldWidth { get { return EditorGUIUtility.fieldWidth; } set { EditorGUIUtility.fieldWidth = value; } }
        public static bool editingTextField { get { return EditorGUIUtility.editingTextField; } set { EditorGUIUtility.editingTextField = value; } }
        public static bool hierarchyMode { get { return EditorGUIUtility.hierarchyMode; } set { EditorGUIUtility.hierarchyMode = value; } }
        public static bool wideMode { get { return EditorGUIUtility.wideMode; } set { EditorGUIUtility.wideMode = value; } }
        public static float labelWidth { get { return EditorGUIUtility.labelWidth; } set { EditorGUIUtility.labelWidth = value; } }


        public static void AddCursorRect(Rect position, UnityEditor. MouseCursor mouse)
        {
            EditorGUIUtility.AddCursorRect(position, mouse);
        }
        public static void AddCursorRect(Rect position, UnityEditor.MouseCursor mouse, int controlID)
        {
            EditorGUIUtility.AddCursorRect(position, mouse, controlID);
        }
        public static Event CommandEvent(string commandName)
        {
            return EditorGUIUtility.CommandEvent(commandName);
        }
        public static void DrawColorSwatch(Rect position, Color color)
        {
            EditorGUIUtility.DrawColorSwatch(position, color);
        }
        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor)
        {
            EditorGUIUtility.DrawCurveSwatch(position, curve, property, color, bgColor);
        }
        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Rect curveRanges)
        {
            EditorGUIUtility.DrawCurveSwatch(position, curve, property, color, bgColor, curveRanges);
        }
        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Color topFillColor, Color bottomFillColor, Rect curveRanges)
        {
            EditorGUIUtility.DrawCurveSwatch(position, curve, property, color, bgColor, topFillColor, bottomFillColor, curveRanges);
        }
        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Color topFillColor, Color bottomFillColor)
        {
            EditorGUIUtility.DrawCurveSwatch(position, curve, property, color, bgColor, topFillColor, bottomFillColor);
        }
        public static void DrawRegionSwatch(Rect position, AnimationCurve curve, AnimationCurve curve2, Color color, Color bgColor, Rect curveRanges)
        {
            EditorGUIUtility.DrawRegionSwatch(position, curve, curve2, color, bgColor, curveRanges);
        }
        public static void DrawRegionSwatch(Rect position, SerializedProperty property, SerializedProperty property2, Color color, Color bgColor, Rect curveRanges)
        {
            EditorGUIUtility.DrawRegionSwatch(position, property, property2, color, bgColor, curveRanges);
        }
        public static Texture2D FindTexture(string name)
        {
            return EditorGUIUtility.FindTexture(name);
        }
        public static GUISkin GetBuiltinSkin(EditorSkin skin)
        {
            return EditorGUIUtility.GetBuiltinSkin(skin);
        }
        public static List<Rect> GetFlowLayoutedRects(Rect rect, GUIStyle style, float horizontalSpacing, float verticalSpacing, List<string> items)
        {
            return EditorGUIUtility.GetFlowLayoutedRects(rect, style, horizontalSpacing, verticalSpacing, items);
        }

        public static Vector2 GetIconSize()
        {
            return EditorGUIUtility.GetIconSize();
        }
        public static int GetObjectPickerControlID()
        {
            return EditorGUIUtility.GetObjectPickerControlID();
        }
        public static UnityEngine.Object GetObjectPickerObject()
        {
            return EditorGUIUtility.GetObjectPickerObject();
        }
        public static bool HasObjectThumbnail(Type objType)
        {
            return EditorGUIUtility.HasObjectThumbnail(objType);
        }
        public static Color HSVToRGB(float H, float S, float V, bool hdr)
        {
            return Color.HSVToRGB(H, S, V, hdr);
        }
        public static Color HSVToRGB(float H, float S, float V)
        {
            return Color.HSVToRGB(H, S, V);
        }
        public static GUIContent IconContent(string name, [UnityEngine.Internal.DefaultValue("null")] string text)
        {
            return EditorGUIUtility.IconContent(name, text);
        }
        public static GUIContent IconContent(string name)
        {
            return EditorGUIUtility.IconContent(name);
        }
        public static bool IsDisplayReferencedByCameras(int displayIndex)
        {
            return EditorGUIUtility.IsDisplayReferencedByCameras(displayIndex);
        }
        public static UnityEngine.Object Load(string path)
        {
            return EditorGUIUtility.Load(path);
        }
        public static UnityEngine.Object LoadRequired(string path)
        {
            return EditorGUIUtility.LoadRequired(path);
        }

        public static GUIContent ObjectContent(UnityEngine.Object obj, Type type)
        {
            return EditorGUIUtility.ObjectContent(obj, type);
        }
        public static void PingObject(UnityEngine.Object obj)
        {
            EditorGUIUtility.PingObject(obj);
        }
        public static void PingObject(int targetInstanceID)
        {
            EditorGUIUtility.PingObject(targetInstanceID);
        }
        public static Rect PixelsToPoints(Rect rect)
        {
            return EditorGUIUtility.PixelsToPoints(rect);
        }
        public static Vector2 PixelsToPoints(Vector2 position)
        {
            return EditorGUIUtility.PixelsToPoints(position);

        }
        public static Vector2 PointsToPixels(Vector2 position)
        {
            return EditorGUIUtility.PointsToPixels(position);
        }
        public static Rect PointsToPixels(Rect rect)
        {
            return EditorGUIUtility.PointsToPixels(rect);
        }
        public static void QueueGameViewInputEvent(Event evt)
        {
            EditorGUIUtility.QueueGameViewInputEvent(evt);
        }

        public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
        {
            Color.RGBToHSV(rgbColor, out H, out S, out V);
        }
        public static string SerializeMainMenuToString()
        {
            return EditorGUIUtility.SerializeMainMenuToString();
        }
        public static void SetIconSize(Vector2 size)
        {
            EditorGUIUtility.SetIconSize(size);
        }
        public static void SetMenuLocalizationTestMode(bool onoff)
        {
            EditorGUIUtility.SetMenuLocalizationTestMode(onoff);
        }
        public static void SetWantsMouseJumping(int wantz)
        {
            EditorGUIUtility.SetWantsMouseJumping(wantz);
        }
        public static void ShowObjectPicker<T>(UnityEngine.Object obj, bool allowSceneObjects, string searchFilter, int controlID) where T : UnityEngine.Object
        {
            EditorGUIUtility.ShowObjectPicker<T>(obj, allowSceneObjects, searchFilter, controlID);
        }
#if UNITY_2018
        public static GUIContent TrIconContent(string iconName, string tooltip = null)
        {
            return EditorGUIUtility.TrIconContent(iconName, tooltip);
        }
        public static GUIContent TrIconContent(Texture icon, string tooltip = null)
        {
            return EditorGUIUtility.TrIconContent(icon, tooltip);
        }
        public static GUIContent[] TrTempContent(string[] texts)
        {
            return EditorGUIUtility.TrTempContent(texts);
        }
        public static GUIContent[] TrTempContent(string[] texts, string[] tooltips)
        {
            return EditorGUIUtility.TrTempContent(texts,tooltips);

        }
        public static GUIContent TrTempContent(string text)
        {
            return EditorGUIUtility.TrTempContent(text);
        }
        public static GUIContent TrTextContent(string text, string tooltip, string iconName)
        {
            return EditorGUIUtility.TrTextContent(text,tooltip,iconName);
        }
        public static GUIContent TrTextContent(string text, string tooltip = null, Texture icon = null)
        {
            return EditorGUIUtility.TrTextContent(text,tooltip,icon);
        }
        public static GUIContent TrTextContent(string key, string text, string tooltip, Texture icon)
        {
            return EditorGUIUtility.TrTextContent(key,text,tooltip,icon);
        }
        public static GUIContent TrTextContent(string text, Texture icon)
        {
            return EditorGUIUtility.TrTextContent(text,icon);
        }
        public static GUIContent TrTextContentWithIcon(string text, string tooltip, string iconName)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text,tooltip,iconName);
        }
        public static GUIContent TrTextContentWithIcon(string text, string iconName)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text,iconName);
        }
        public static GUIContent TrTextContentWithIcon(string text, string tooltip, Texture icon)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text,tooltip,icon);
        }
        public static GUIContent TrTextContentWithIcon(string text, string tooltip, UnityEditor.MessageType messageType)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text,tooltip,messageType);
        }
        public static GUIContent TrTextContentWithIcon(string text, UnityEditor.MessageType messageType)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text,messageType);
        }
        public static GUIContent TrTextContentWithIcon(string text, Texture icon)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text,icon);
        }
#endif
       
    }
}

