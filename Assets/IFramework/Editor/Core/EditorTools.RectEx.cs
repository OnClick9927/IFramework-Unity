/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{

    partial class EditorTools
    {

        public static class RectEx
        {

            public static Rect Zoom(Rect rect, TextAnchor type, float pixel)
            {
                return Zoom(rect, type, new Vector2(pixel, pixel));
            }
            public static Rect Zoom(Rect rect, TextAnchor type, Vector2 pixelOffset)
            {
                float tempW = rect.width + pixelOffset.x;
                float tempH = rect.height + pixelOffset.y;
                switch (type)
                {
                    case TextAnchor.UpperLeft:
                        break;
                    case TextAnchor.UpperCenter:
                        rect.x -= (tempW - rect.width) / 2;
                        break;
                    case TextAnchor.UpperRight:
                        rect.x -= tempW - rect.width;
                        break;
                    case TextAnchor.MiddleLeft:
                        rect.y -= (tempH - rect.height) / 2;
                        break;
                    case TextAnchor.MiddleCenter:
                        rect.x -= (tempW - rect.width) / 2;
                        rect.y -= (tempH - rect.height) / 2;
                        break;
                    case TextAnchor.MiddleRight:
                        rect.y -= (tempH - rect.height) / 2;
                        rect.x -= tempW - rect.width;
                        break;
                    case TextAnchor.LowerLeft:
                        rect.y -= tempH - rect.height;
                        break;
                    case TextAnchor.LowerCenter:
                        rect.y -= tempH - rect.height;
                        rect.x -= (tempW - rect.width) / 2;
                        break;
                    case TextAnchor.LowerRight:
                        rect.y -= tempH - rect.height;
                        rect.x -= tempW - rect.width;
                        break;
                }
                rect.width = tempW;
                rect.height = tempH;
                return rect;
            }

            private static Rect CutBottom(Rect r, float pixels)
            {
                r.yMax -= pixels;
                return r;
            }
            private static Rect CutTop(Rect r, float pixels)
            {
                r.yMin += pixels;
                return r;
            }
            private static Rect CutRight(Rect r, float pixels)
            {
                r.xMax -= pixels;
                return r;
            }
            private static Rect CutLeft(Rect r, float pixels)
            {
                r.xMin += pixels;
                return r;
            }
            private static Rect Cut(Rect r, float pixels)
            {
                return Margin(r, -pixels);
            }
            private static Rect Margin(Rect r, float pixels)
            {
                r.xMax += pixels;
                r.xMin -= pixels;
                r.yMax += pixels;
                r.yMin -= pixels;
                return r;
            }

            public static Rect[] Split(Rect r, bool vertical, float offset, float padding = 0, bool justMid = true)
            {
                if (vertical)
                    return VerticalSplit(r, offset, padding, justMid);
                return HorizontalSplit(r, offset, padding, justMid);
            }
            public static Rect SplitRect(Rect r, bool vertical, float offset, float padding = 0)
            {
                if (vertical)
                    return VerticalSplitRect(r, offset, padding);
                return HorizontalSplitRect(r, offset, padding);
            }
            public static Rect[] VerticalSplit(Rect r, float width, float padding = 0, bool justMid = true)
            {
                if (justMid)
                    return new Rect[2]{
                CutRight(CutRight(CutRight(r,(int)(r.width-width)),padding),-Mathf.CeilToInt(padding/2f)),
                CutLeft(CutLeft(CutLeft(r,width),padding),-Mathf.FloorToInt(padding/2f))
            };
                return new Rect[2]{
                CutRight(Cut(CutRight(r,(int)(r.width-width)),padding),-Mathf.CeilToInt(padding/2f)),
                CutLeft(Cut(CutLeft(r,width),padding),-Mathf.FloorToInt(padding/2f))
            };
            }
            public static Rect[] HorizontalSplit(Rect r, float height, float padding = 0, bool justMid = true)
            {
                if (justMid)
                    return new Rect[2]{
                CutBottom(CutBottom(CutBottom(r,(int)(r.height-height)),padding),-Mathf.CeilToInt(padding/2f)),
                CutTop(CutTop(CutTop(r,height),padding),-Mathf.FloorToInt(padding/2f))
                };
                return new Rect[2]{
                CutBottom(Cut(CutBottom(r,(int)(r.height-height)),padding),-Mathf.CeilToInt(padding/2f)),
                CutTop(Cut(CutTop(r,height),padding),-Mathf.FloorToInt(padding/2f))
            };
            }
            private static Rect HorizontalSplitRect(Rect r, float height, float padding = 0)
            {
                Rect rect = CutBottom(Cut(CutBottom(r, (int)(r.height - height)), padding), -Mathf.CeilToInt(padding / 2f));
                rect.y += rect.height;
                rect.height = padding;
                return rect;
            }
            private static Rect VerticalSplitRect(Rect r, float width, float padding = 0)
            {
                Rect rect = CutRight(Cut(CutRight(r, (int)(r.width - width)), padding), -Mathf.CeilToInt(padding / 2f));
                rect.x += rect.width;
                rect.width = padding;
                return rect;
            }
            public static void DrawOutLine(Rect rect, Color color)
            {
                Handles.color = color;

                Handles.DrawAAPolyLine(2, new Vector3(rect.x,
                                             rect.y,
                                             0),
                              new Vector3(rect.x,
                                             rect.yMax,
                                             0),
                              new Vector3(rect.xMax,
                                             rect.yMax,
                                             0),
                              new Vector3(rect.xMax,
                                             rect.y,
                                             0),
                              new Vector3(rect.x,
                                             rect.y,
                                             0)
                                );

                Handles.color = Color.white;
            }
        }

    }
}
