/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-08-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.GUITool
{
    public static partial class RectGUIDwerExtemsion
    {
        public static IRectGUIDrawer Label(this IRectGUIDrawer self, Rect position, string text)
        {
            GUI.Label(position, text);
            return self;
        }
        public static IRectGUIDrawer Label(this IRectGUIDrawer self, Rect position, string text, GUIStyle style)
        {
            GUI.Label(position, text, style);
            return self;
        }
        public static IRectGUIDrawer Label(this IRectGUIDrawer self, Rect position, GUIContent content)
        {
            GUI.Label(position, content);
            return self;
        }
        public static IRectGUIDrawer Label(this IRectGUIDrawer self, Rect position, Texture image)
        {
            GUI.Label(position, image);
            return self;
        }
        public static IRectGUIDrawer Label(this IRectGUIDrawer self, Rect position, GUIContent content, GUIStyle style)
        {
            GUI.Label(position, content, style);
            return self;
        }
        public static IRectGUIDrawer Label(this IRectGUIDrawer self, Rect position, Texture image, GUIStyle style)
        {
            GUI.Label(position, image, style);
            return self;
        }
        public static IRectGUIDrawer Button(this IRectGUIDrawer self, Action listener, Rect position, Texture image)
        {
            bool bo = GUI.Button(position, image);
            if (bo && listener != null)
            {
                listener();
            }
            return self;
        }
        public static IRectGUIDrawer Button(this IRectGUIDrawer self, Action listener, Rect position, Texture image, GUIStyle style)
        {
            bool bo = GUI.Button(position, image, style);
            if (bo && listener != null)
            {
                listener();
            }
            return self;
        }
        public static IRectGUIDrawer Button(this IRectGUIDrawer self, Action listener, Rect position, string text, GUIStyle style)
        {
            bool bo = GUI.Button(position, text, style);
            if (bo && listener != null)
            {
                listener();
            }
           
            return self;

        }
        public static IRectGUIDrawer Button(this IRectGUIDrawer self, Action listener, Rect position, GUIContent content)
        {
            bool bo = GUI.Button(position, content);
            if (bo && listener != null)
            {
                listener();
            }
           
            return self;

        }
        public static IRectGUIDrawer Button(this IRectGUIDrawer self, Action listener, Rect position, GUIContent content,GUIStyle style)
        {
            bool bo = GUI.Button(position, content,style);
            if (bo && listener != null)
            {
                listener();
            }
           
            return self;

        }
        public static IRectGUIDrawer Button(this IRectGUIDrawer self, Action listener, Rect position, string text)
        {
            bool bo = GUI.Button(position, text);
            if (bo && listener != null)
            {
                listener();
            }
            return self;

        }

        public static IRectGUIDrawer RepeatButton(this IRectGUIDrawer self, Action listener, Rect position, string text)
        {
            bool bo = GUI.RepeatButton(position, text);
            if (bo && listener != null)
            {
                listener();
            }
            return self;
        }
        public static IRectGUIDrawer RepeatButton(this IRectGUIDrawer self, Action listener, Rect position, Texture image)
        {
            bool bo = GUI.RepeatButton(position, image);
            if (bo && listener != null)
            {
                listener();
            }
            return self;
        }
        public static IRectGUIDrawer RepeatButton(this IRectGUIDrawer self, Action listener, Rect position, GUIContent content)
        {
            bool bo = GUI.RepeatButton(position, content);
            if (bo && listener != null)
            {
                listener();
            }
            return self;
        }
        public static IRectGUIDrawer RepeatButton(this IRectGUIDrawer self, Action listener, Rect position, string text, GUIStyle style)
        {
            bool bo = GUI.RepeatButton(position, text, style);
            if (bo && listener != null)
            {
                listener();
            }         
            return self;
        }
        public static IRectGUIDrawer RepeatButton(this IRectGUIDrawer self, Action listener, Rect position, Texture image, GUIStyle style)
        {
            bool bo = GUI.RepeatButton(position, image, style);
            if (bo && listener != null)
            {
                listener();
            }
            return self;
        }
        public static IRectGUIDrawer RepeatButton(this IRectGUIDrawer self, Action listener, Rect position, GUIContent content, GUIStyle style)
        {
            bool bo = GUI.RepeatButton(position, content, style);
            if (bo && listener != null)
            {
                listener();
            }
            return self;
        }

        public static IRectGUIDrawer Box(this IRectGUIDrawer self, Rect rect)
        {
            Box(self, rect, "");
            return self;

        }
        public static IRectGUIDrawer Box(this IRectGUIDrawer self, Rect rect, string text)
        {
            GUI.Box(rect, text);
            return self;

        }
        public static IRectGUIDrawer Box(this IRectGUIDrawer self, Rect rect, Texture image)
        {
            GUI.Box(rect, image); return self;

        }
        public static IRectGUIDrawer Box(this IRectGUIDrawer self, Rect rect, GUIContent content)
        {
            GUI.Box(rect, content); return self;

        }
        public static IRectGUIDrawer Box(this IRectGUIDrawer self, Rect rect, string text, GUIStyle style)
        {
            GUI.Box(rect, text, style); return self;

        }
        public static IRectGUIDrawer Box(this IRectGUIDrawer self, Rect rect, Texture image, GUIStyle style)
        {
            GUI.Box(rect, image, style); return self;

        }
        public static IRectGUIDrawer Box(this IRectGUIDrawer self, Rect rect, GUIContent content, GUIStyle style)
        {
            GUI.Box(rect, content, style); return self;

        }


        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position, GUIContent content, GUIStyle style)
        {
            GUI.BeginGroup(position, content, style);
            if (act != null) act();
            GUI.EndGroup(); return self;

        }
        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position, Texture image, GUIStyle style)
        {
            GUI.BeginGroup(position, image, style);
            if (act != null) act();
            GUI.EndGroup(); return self;

        }
        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position, string text, GUIStyle style)
        {
            GUI.BeginGroup(position, text, style);
            if (act != null) act();
            GUI.EndGroup(); return self;

        }
        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position, GUIStyle style)
        {
            GUI.BeginGroup(position, style);
            if (act != null) act();
            GUI.EndGroup(); return self;

        }
        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position, string text)
        {
            GUI.BeginGroup(position, text);
            if (act != null) act();
            GUI.EndGroup();
            return self;

        }
        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position, Texture image)
        {
            GUI.BeginGroup(position, image);
            if (act != null) act();
            GUI.EndGroup();
            return self;

        }
        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position)
        {
            GUI.BeginGroup(position);
            if (act != null) act();
            GUI.EndGroup();
            return self;

        }
        public static IRectGUIDrawer DrawGroup(this IRectGUIDrawer self, Action act, Rect position, GUIContent content)
        {
            GUI.BeginGroup(position, content);
            if (act != null) act();
            GUI.EndGroup();
            return self;

        }
        public static IRectGUIDrawer DrawClip(this IRectGUIDrawer self, Action act, Rect position)
        {
            GUI.BeginClip(position);
            if (act != null) act();
            GUI.EndClip();
            return self;

        }
        public static IRectGUIDrawer DrawClip(this IRectGUIDrawer self, Action act, Rect position, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
        {
            GUI.BeginClip(position, scrollOffset, renderOffset, resetOffset);
            if (act != null) act();
            GUI.EndClip();
            return self;

        }
        public static IRectGUIDrawer DrawTexture(this IRectGUIDrawer self, Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect)
        {
            GUI.DrawTexture(position, image, scaleMode, alphaBlend, imageAspect); return self;

        }
        public static IRectGUIDrawer DrawTexture(this IRectGUIDrawer self, Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend)
        {
            GUI.DrawTexture(position, image, scaleMode, alphaBlend); return self;

        }
        public static IRectGUIDrawer DrawTexture(this IRectGUIDrawer self, Rect position, Texture image, ScaleMode scaleMode)
        {
            GUI.DrawTexture(position, image, scaleMode); return self;

        }
        public static IRectGUIDrawer DrawTexture(this IRectGUIDrawer self, Rect position, Texture image)
        {
            GUI.DrawTexture(position, image); return self;

        }
        public static IRectGUIDrawer DrawTexture(this IRectGUIDrawer self, Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, Vector4 borderWidths, float borderRadius)
        {
            GUI.DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, color, borderWidths, borderRadius); return self;

        }
        public static IRectGUIDrawer DrawTexture(this IRectGUIDrawer self, Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, float borderWidth, float borderRadius)
        {
            GUI.DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, color, borderWidth, borderRadius); return self;

        }

        public static IRectGUIDrawer DrawTextureWithTexCoords(this IRectGUIDrawer self, Rect position, Texture image, Rect texCoords, bool alphaBlend)
        {
            GUI.DrawTextureWithTexCoords(position, image, texCoords, alphaBlend); return self;

        }
        public static IRectGUIDrawer DrawTextureWithTexCoords(this IRectGUIDrawer self, Rect position, Texture image, Rect texCoords)
        {
            GUI.DrawTextureWithTexCoords(position, image, texCoords); return self;

        }


        public static IRectGUIDrawer TextField(this IRectGUIDrawer self, Rect position, ref string text, int maxLength, GUIStyle style)
        {
            text = GUI.TextField(position, text, maxLength, style);
            return self;
        }
        public static IRectGUIDrawer TextField(this IRectGUIDrawer self, Rect position, ref string text, GUIStyle style)
        {
            text = GUI.TextField(position, text, style); return self;

        }
        public static IRectGUIDrawer TextField(this IRectGUIDrawer self, Rect position, ref string text, int maxLength)
        {
            text = GUI.TextField(position, text, maxLength); return self;

        }
        public static IRectGUIDrawer TextField(this IRectGUIDrawer self, Rect position, ref string text)
        {
            text = GUI.TextField(position, text); return self;

        }

        public static IRectGUIDrawer PasswordField(this IRectGUIDrawer self, Rect position, ref string password, char maskChar, int maxLength, GUIStyle style)
        {
            password = GUI.PasswordField(position, password, maskChar, maxLength, style); return self;

        }
        public static IRectGUIDrawer PasswordField(this IRectGUIDrawer self, Rect position, ref string password, char maskChar)
        {
            password = GUI.PasswordField(position, password, maskChar); return self;

        }
        public static IRectGUIDrawer PasswordField(this IRectGUIDrawer self, Rect position, ref string password, char maskChar, int maxLength)
        {
            password = GUI.PasswordField(position, password, maskChar, maxLength); return self;

        }
        public static IRectGUIDrawer PasswordField(this IRectGUIDrawer self, Rect position, ref string password, char maskChar, GUIStyle style)
        {
            password = GUI.PasswordField(position, password, maskChar, style); return self;

        }

        public static IRectGUIDrawer TextArea(this IRectGUIDrawer self, Rect position, ref string text, int maxLength)
        {
            text = GUI.TextArea(position, text, maxLength); return self;

        }
        public static IRectGUIDrawer TextArea(this IRectGUIDrawer self, Rect position, ref string text)
        {
            text = GUI.TextArea(position, text); return self;

        }
        public static IRectGUIDrawer TextArea(this IRectGUIDrawer self, Rect position, ref string text, int maxLength, GUIStyle style)
        {
            text = GUI.TextArea(position, text, maxLength, style); return self;

        }
        public static IRectGUIDrawer TextArea(this IRectGUIDrawer self, Rect position, ref string text, GUIStyle style)
        {
            text = GUI.TextArea(position, text, style); return self;

        }

        public static IRectGUIDrawer DrawScrollView(this IRectGUIDrawer self, Action act, Rect position, ref Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
        {

            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar);
            if (act != null) act();
            GUI.EndScrollView(); return self;

        }
        public static IRectGUIDrawer DrawScrollView(this IRectGUIDrawer self, Action act, Rect position, ref Vector2 scrollPosition, Rect viewRect)
        {
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect);
            if (act != null) act();
            GUI.EndScrollView(); return self;

        }
        public static IRectGUIDrawer DrawScrollView(this IRectGUIDrawer self, Action act, Rect position, ref Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
        {
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical);
            if (act != null) act();
            GUI.EndScrollView(); return self;

        }
        public static IRectGUIDrawer DrawScrollView(this IRectGUIDrawer self, Action act, Rect position, ref Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
        {
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar);
            if (act != null) act();
            GUI.EndScrollView(); return self;

        }


        public static IRectGUIDrawer SelectionGrid(this IRectGUIDrawer self, Rect position, ref int selected, Texture[] images, int xCount, GUIStyle style)
        {
            selected = GUI.SelectionGrid(position, selected, images, xCount, style); return self;

        }
        public static IRectGUIDrawer SelectionGrid(this IRectGUIDrawer self, Rect position, ref int selected, GUIContent[] contents, int xCount, GUIStyle style)
        {
            selected = GUI.SelectionGrid(position, selected, contents, xCount, style); return self;

        }
        public static IRectGUIDrawer SelectionGrid(this IRectGUIDrawer self, Rect position, ref int selected, string[] texts, int xCount, GUIStyle style)
        {
            selected = GUI.SelectionGrid(position, selected, texts, xCount, style); return self;

        }
        public static IRectGUIDrawer SelectionGrid(this IRectGUIDrawer self, Rect position, ref int selected, GUIContent[] content, int xCount)
        {
            selected = GUI.SelectionGrid(position, selected, content, xCount); return self;

        }
        public static IRectGUIDrawer SelectionGrid(this IRectGUIDrawer self, Rect position, ref int selected, Texture[] images, int xCount)
        {
            selected = GUI.SelectionGrid(position, selected, images, xCount); return self;

        }
        public static IRectGUIDrawer SelectionGrid(this IRectGUIDrawer self, Rect position, ref int selected, string[] texts, int xCount)
        {
            selected = GUI.SelectionGrid(position, selected, texts, xCount); return self;

        }

        public static IRectGUIDrawer Toggle(this IRectGUIDrawer self, Rect position, ref bool value, Texture image, GUIStyle style)
        {
            value = GUI.Toggle(position, value, image, style); return self;

        }
        public static IRectGUIDrawer Toggle(this IRectGUIDrawer self, Rect position, ref bool value, GUIContent content, GUIStyle style)
        {
            value = GUI.Toggle(position, value, content, style); return self;

        }
        public static IRectGUIDrawer Toggle(this IRectGUIDrawer self, Rect position, ref bool value, string text, GUIStyle style)
        {
            value = GUI.Toggle(position, value, text, style); return self;

        }
        public static IRectGUIDrawer Toggle(this IRectGUIDrawer self, Rect position, ref bool value, GUIContent content)
        {
            value = GUI.Toggle(position, value, content); return self;

        }
        public static IRectGUIDrawer Toggle(this IRectGUIDrawer self, Rect position, ref bool value, Texture image)
        {
            value = GUI.Toggle(position, value, image); return self;

        }
        public static IRectGUIDrawer Toggle(this IRectGUIDrawer self, Rect position, ref bool value, string text)
        {
            value = GUI.Toggle(position, value, text); return self;

        }
        public static IRectGUIDrawer Toggle(this IRectGUIDrawer self, Rect position, int id, ref bool value, GUIContent content, GUIStyle style)
        {
            value = GUI.Toggle(position, id, value, content, style); return self;

        }

        public static IRectGUIDrawer Toolbar(this IRectGUIDrawer self, Rect position, ref int selected, GUIContent[] contents, GUIStyle style)
        {
            selected = GUI.Toolbar(position, selected, contents, style); return self;

        }
        public static IRectGUIDrawer Toolbar(this IRectGUIDrawer self, Rect position, ref int selected, string[] texts)
        {
            selected = GUI.Toolbar(position, selected, texts); return self;

        }
        public static IRectGUIDrawer Toolbar(this IRectGUIDrawer self, Rect position, ref int selected, Texture[] images)
        {
            selected = GUI.Toolbar(position, selected, images); return self;

        }
        public static IRectGUIDrawer Toolbar(this IRectGUIDrawer self, Rect position, ref int selected, GUIContent[] contents)
        {
            selected = GUI.Toolbar(position, selected, contents); return self;

        }
        public static IRectGUIDrawer Toolbar(this IRectGUIDrawer self, Rect position, ref int selected, string[] texts, GUIStyle style)
        {
            selected = GUI.Toolbar(position, selected, texts, style); return self;

        }
        public static IRectGUIDrawer Toolbar(this IRectGUIDrawer self, Rect position, ref int selected, Texture[] images, GUIStyle style)
        {
            selected = GUI.Toolbar(position, selected, images, style); return self;

        }
        public static IRectGUIDrawer Toolbar(this IRectGUIDrawer self, Rect position, ref int selected, GUIContent[] contents, GUIStyle style, GUI.ToolbarButtonSize buttonSize)
        {
            selected = GUI.Toolbar(position, selected, contents, style, buttonSize); return self;

        }

        public static IRectGUIDrawer VerticalScrollbar(this IRectGUIDrawer self, Rect position, ref float value, float size, float topValue, float bottomValue, GUIStyle style)
        {
            value = GUI.VerticalScrollbar(position, value, size, topValue, bottomValue, style); return self;

        }
        public static IRectGUIDrawer VerticalScrollbar(this IRectGUIDrawer self, Rect position, ref float value, float size, float topValue, float bottomValue)
        {
            value = GUI.VerticalScrollbar(position, value, size, topValue, bottomValue); return self;

        }


        public static IRectGUIDrawer VerticalSlider(this IRectGUIDrawer self, Rect position, ref float value, float topValue, float bottomValue)
        {
            value = GUI.VerticalSlider(position, value, topValue, bottomValue); return self;

        }
        public static IRectGUIDrawer VerticalSlider(this IRectGUIDrawer self, Rect position, ref float value, float topValue, float bottomValue, GUIStyle slider, GUIStyle thumb)
        {
            value = GUI.VerticalSlider(position, value, topValue, bottomValue, slider, thumb); return self;

        }
        public static IRectGUIDrawer Slider(this IRectGUIDrawer self, Rect position, ref float value, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id)
        {
            value = GUI.Slider(position, value, size, start, end, slider, thumb, horiz, id); return self;

        }


        public static IRectGUIDrawer ModalWindow(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, GUIContent content, GUIStyle style)
        {
            clientRect = GUI.ModalWindow(id, clientRect, func, content, style); return self;

        }
        public static IRectGUIDrawer ModalWindow(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, Texture image, GUIStyle style)
        {
            clientRect = GUI.ModalWindow(id, clientRect, func, image, style); return self;

        }
        public static IRectGUIDrawer ModalWindow(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, string text, GUIStyle style)
        {
            clientRect = GUI.ModalWindow(id, clientRect, func, text, style); return self;

        }
        public static IRectGUIDrawer ModalWindow(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, GUIContent content)
        {
            clientRect = GUI.ModalWindow(id, clientRect, func, content); return self;

        }
        public static IRectGUIDrawer ModalWindow(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, string text)
        {
            clientRect = GUI.ModalWindow(id, clientRect, func, text); return self;

        }
        public static IRectGUIDrawer ModalWindow(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, Texture image)
        {
            clientRect = GUI.ModalWindow(id, clientRect, func, image); return self;

        }

        public static IRectGUIDrawer Window(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, Texture image, GUIStyle style)
        {
            clientRect = GUI.Window(id, clientRect, func, image, style); return self;

        }
        public static IRectGUIDrawer Window(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, string text)
        {
            clientRect = GUI.Window(id, clientRect, func, text); return self;

        }
        public static IRectGUIDrawer Window(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, Texture image)
        {
            clientRect = GUI.Window(id, clientRect, func, image); return self;

        }
        public static IRectGUIDrawer Window(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, GUIContent content)
        {
            clientRect = GUI.Window(id, clientRect, func, content); return self;

        }
        public static IRectGUIDrawer Window(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, string text, GUIStyle style)
        {
            clientRect = GUI.Window(id, clientRect, func, text, style); return self;

        }
        public static IRectGUIDrawer Window(this IRectGUIDrawer self, int id, ref Rect clientRect, GUI.WindowFunction func, GUIContent title, GUIStyle style)
        {
            clientRect = GUI.Window(id, clientRect, func, title, style); return self;

        }



        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position, GUIContent content, GUIStyle style)
        {
            GUI.BeginGroup(position, content, style);
            return self;
        }
        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position, Texture image, GUIStyle style)
        {
            GUI.BeginGroup(position, image, style);

            return self;
        }
        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position, string text, GUIStyle style)
        {
            GUI.BeginGroup(position, text, style);
            return self;
        }
        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position, GUIStyle style)
        {
            GUI.BeginGroup(position, style);
            return self;
        }
        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position, string text)
        {
            GUI.BeginGroup(position, text);
            return self;
        }
        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position, Texture image)
        {
            GUI.BeginGroup(position, image);
            return self;
        }
        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position)
        {
            GUI.BeginGroup(position);
            return self;
        }
        public static IRectGUIDrawer BeginGroup(this IRectGUIDrawer self, Rect position, GUIContent content)
        {
            GUI.BeginGroup(position, content);
            return self;
        }
        public static IRectGUIDrawer BeginClip(this IRectGUIDrawer self, Rect position)
        {
            GUI.BeginClip(position);
            return self;
        }
        public static IRectGUIDrawer BeginClip(this IRectGUIDrawer self, Rect position, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
        {
            GUI.BeginClip(position, scrollOffset, renderOffset, resetOffset);
            return self;
        }
        public static IRectGUIDrawer BeginScrollView(this IRectGUIDrawer self, Rect position, ref Vector2 scrollPosition, Rect viewRect)
        {
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect);
            return self;
        }
        public static IRectGUIDrawer BeginScrollView(this IRectGUIDrawer self, Rect position, ref Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
        {
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar);
            return self;
        }
        public static IRectGUIDrawer BeginScrollView(this IRectGUIDrawer self, Rect position, ref Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
        {
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical);
            return self;
        }
        public static IRectGUIDrawer BeginScrollView(this IRectGUIDrawer self, Rect position, ref Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
        {
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar);
            return self;
        }
        public static IRectGUIDrawer EndGroup(this IRectGUIDrawer self)
        {
            GUI.EndGroup();
            return self;
        }
        public static IRectGUIDrawer EndScrollView(this IRectGUIDrawer self)
        {
            GUI.EndScrollView();
            return self;
        }
        public static IRectGUIDrawer EndClip(this IRectGUIDrawer self)
        {
            GUI.EndClip();
            return self;
        }
    }

}
