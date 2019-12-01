/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework
{
    public enum AnchorType
    {
        UpperLeft = 0,
        UpperCenter = 1,
        UpperRight = 2,
        MiddleLeft = 3,
        MiddleCenter = 4,
        MiddleRight = 5,
        LowerLeft = 6,
        LowerCenter = 7,
        LowerRight = 8
    }
    public enum SplitType
    {
        Vertical, Horizontal
    }

    public static class RectExtension
    {
        public static Rect MutlZoom(this Rect rect, AnchorType type, float scale)
        {
            return MutlZoom(rect,type,new Vector2(scale,scale));
        }
        public static Rect MutlZoom(this Rect rect, AnchorType type, Vector2 scale)
        {
            float tempW = rect.width * scale.x;
            float tempH = rect.height * scale.y;
            switch (type)
            {
                case AnchorType.UpperLeft:
                    break;
                case AnchorType.UpperCenter:
                    rect.x -= (tempW - rect.width) / 2;
                    break;
                case AnchorType.UpperRight:
                    rect.x -= tempW - rect.width;
                    break;
                case AnchorType.MiddleLeft:
                    rect.y -= (tempH - rect.height) / 2;
                    break;
                case AnchorType.MiddleCenter:
                    rect.x -= (tempW - rect.width) / 2;
                    rect.y -= (tempH - rect.height) / 2;
                    break;
                case AnchorType.MiddleRight:
                    rect.y -= (tempH - rect.height) / 2;
                    rect.x -= tempW - rect.width;
                    break;
                case AnchorType.LowerLeft:
                    rect.y -= tempH - rect.height;
                    break;
                case AnchorType.LowerCenter:
                    rect.y -= tempH - rect.height;
                    rect.x -= (tempW - rect.width) / 2;
                    break;
                case AnchorType.LowerRight:
                    rect.y -= tempH - rect.height;
                    rect.x -= tempW - rect.width;
                    break;
            }
            rect.width = tempW;
            rect.height = tempH;
            return rect;
        }
        public static Rect MutlZoom(this Rect rect, AnchorType type,Vector2 offset, Vector2 scale)
        {
            Rect r = MutlZoom(rect, type, scale);
            r.position += offset;
            return  r;
        }

        public static Rect Zoom(this Rect rect, AnchorType type, float pixel)
        {
            return Zoom(rect,type,new Vector2(pixel, pixel));
        }
        public static Rect Zoom(this Rect rect, AnchorType type,Vector2 pixelOffset)
        {
            float tempW = rect.width + pixelOffset.x;
            float tempH = rect.height + pixelOffset.y;
            switch (type)
            {
                case AnchorType.UpperLeft:
                    break;
                case AnchorType.UpperCenter:
                    rect.x -= (tempW - rect.width) / 2;
                    break;
                case AnchorType.UpperRight:
                    rect.x -= tempW - rect.width;
                    break;
                case AnchorType.MiddleLeft:
                    rect.y -= (tempH - rect.height) / 2;
                    break;
                case AnchorType.MiddleCenter:
                    rect.x -= (tempW - rect.width) / 2;
                    rect.y -= (tempH - rect.height) / 2;
                    break;
                case AnchorType.MiddleRight:
                    rect.y -= (tempH - rect.height) / 2;
                    rect.x -= tempW - rect.width;
                    break;
                case AnchorType.LowerLeft:
                    rect.y -= tempH - rect.height;
                    break;
                case AnchorType.LowerCenter:
                    rect.y -= tempH - rect.height;
                    rect.x -= (tempW - rect.width) / 2;
                    break;
                case AnchorType.LowerRight:
                    rect.y -= tempH - rect.height;
                    rect.x -= tempW - rect.width;
                    break;
            }
            rect.width = tempW;
            rect.height = tempH;
            return rect;
        }


        public static Rect MoveDown(this Rect r, float pixels = 1)
        {
            r.y += pixels;
            return r;
        }
        public static Rect MoveUp(this Rect r, float pixels = 1)
        {
            return r.MoveDown(-pixels);
        }
        public static Rect MoveRight(this Rect r, float pixels = 1)
        {
            r.x += pixels;
            return r;
        }
        public static Rect MoveLeft(this Rect r, float pixels = 1)
        {
            return r.MoveRight(-pixels);
        }

        public static Rect MoveDownBySacle(this Rect r, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                r.y += r.height;
            }
            return r;
        }
        public static Rect MoveUpBySacle(this Rect r, int count = 1)
        {
            return r.MoveDownBySacle(-count);
        }
        public static Rect MoveRightBySacle(this Rect r, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                r.x += r.width;
            }
            return r;
        }
        public static Rect MoveLeftBySacle(this Rect r, int count = 1)
        {
            return r.MoveRightBySacle(-count);
        }


        public static Rect CutBottom(this Rect r, float pixels)
        {
            r.yMax -= pixels;
            return r;
        }
        public static Rect CutTop(this Rect r, float pixels)
        {
            r.yMin += pixels;
            return r;
        }
        public static Rect CutRight(this Rect r, float pixels)
        {
            r.xMax -= pixels;
            return r;
        }
        public static Rect CutLeft(this Rect r, float pixels)
        {
            r.xMin += pixels;
            return r;
        }
        public static Rect CutHorizontal(this Rect r, float pixels)
        {
            return r.CutLeft(pixels).CutRight(pixels);
        }
        public static Rect CutVertical(this Rect r, float pixels)
        {
            return r.CutTop(pixels).CutBottom(pixels);
        }
        public static Rect Cut(this Rect r, float pixels)
        {
            return r.Margin(-pixels);
        }

        public static Rect Margin(this Rect r, float pixels)
        {
            r.xMax += pixels;
            r.xMin -= pixels;
            r.yMax += pixels;
            r.yMin -= pixels;
            return r;
        }

        public static Rect[] Split(this Rect r, SplitType type, float offset, float padding = 0, bool justMid = true)
        {
            switch (type)
            {
                case SplitType.Vertical:
                    return r.VerticalSplit(offset, padding, justMid);
                case SplitType.Horizontal:
                    return r.HorizontalSplit(offset, padding, justMid);
                default:
                    return default(Rect[]);
            }
        }
        public static Rect SplitRect(this Rect r, SplitType type, float offset, float padding = 0)
        {
            switch (type)
            {
                case SplitType.Vertical:
                    return r.VerticalSplitRect(offset, padding);
                case SplitType.Horizontal:
                    return r.HorizontalSplitRect(offset, padding);
                default:
                    return default(Rect);
            }
        }

        public static Rect[] VerticalSplit(this Rect r, float width, float padding = 0, bool justMid = true)
        {
            if (justMid)
                return new Rect[2]{
                r.CutRight((int)(r.width-width)).CutRight(padding).CutRight(-Mathf.CeilToInt(padding/2f)),
                r.CutLeft(width).CutLeft(padding).CutLeft(-Mathf.FloorToInt(padding/2f))
            };
            return new Rect[2]{
                r.CutRight((int)(r.width-width)).Cut(padding).CutRight(-Mathf.CeilToInt(padding/2f)),
                r.CutLeft(width).Cut(padding).CutLeft(-Mathf.FloorToInt(padding/2f))
            };
        }
        public static Rect[] HorizontalSplit(this Rect r, float height, float padding = 0, bool justMid = true)
        {
            if (justMid)
                return new Rect[2]{
                r.CutBottom((int)(r.height-height)).CutBottom(padding).CutBottom(-Mathf.CeilToInt(padding/2f)),
                r.CutTop(height).CutTop(padding).CutTop(-Mathf.FloorToInt(padding/2f))
                };
            return new Rect[2]{
                r.CutBottom((int)(r.height-height)).Cut(padding).CutBottom(-Mathf.CeilToInt(padding/2f)),
                r.CutTop(height).Cut(padding).CutTop(-Mathf.FloorToInt(padding/2f))
            };
        }
        public static Rect HorizontalSplitRect(this Rect r, float height, float padding = 0)
        {
            Rect rect = r.CutBottom((int)(r.height - height)).Cut(padding).CutBottom(-Mathf.CeilToInt(padding / 2f));
            rect.y += rect.height;
            rect.height = padding;
            return rect;
        }
        public static Rect VerticalSplitRect(this Rect r, float width, float padding = 0)
        {
            Rect rect = r.CutRight((int)(r.width - width)).Cut(padding).CutRight(-Mathf.CeilToInt(padding / 2f));
            rect.x += rect.width;
            rect.width = padding;
            return rect;
        }

        public static Rect ClampWidth(this Rect r, float min, float max)
        {
            r.width = Mathf.Clamp(r.width, min, max);
            return r;
        }
        public static Rect ClampHeight(this Rect r, float min, float max)
        {
            r.height = Mathf.Clamp(r.height, min, max);
            return r;
        }
        public static Rect ClampSize(this Rect r, Vector2 min, Vector2 max)
        {
            return r.ClampWidth(min.x, max.x).ClampHeight(min.y, max.y);
        }
        public static Rect ClampMinSize(this Rect r, float width, float height)
        {
            if (r.width < width)
                r.width = width;
            if (r.height < height)
                r.height = height;
            return r;
        }
        public static Rect ClampMinSize(this Rect r, Vector2 size)
        {
            if (r.width < size.x)
                r.width = size.x;
            if (r.height < size.y)
                r.height = size.y;
            return r;
        }
        public static Rect ClampMinSize(this Rect r, float size)
        {
            if (r.width < size)
                r.width = size;
            if (r.height < size)
                r.height = size;
            return r;
        }

        public static Rect ClampMaxSize(this Rect r, Vector2 size)
        {
            if (r.width > size.x)
                r.width = size.x;
            if (r.height > size.y)
                r.height = size.y;
            return r;
        }
        public static Rect ClampMaxSize(this Rect r, float width, float height)
        {
            if (r.width > width)
                r.width = width;
            if (r.height > height)
                r.height = height;
            return r;
        }
        public static Rect ClampMaxSize(this Rect r, float size)
        {
            if (r.width > size)
                r.width = size;
            if (r.height > size)
                r.height = size;
            return r;
        }

        public static Vector2 TopLeft(this Rect r)
        {
            return new Vector2(r.x, r.y);
        }
        public static Vector2 TopRight(this Rect r)
        {
            return new Vector2(r.xMax, r.y);
        }
        public static Vector2 BottomRight(this Rect r)
        {
            return new Vector2(r.xMax, r.yMax);
        }
        public static Vector2 BottomLeft(this Rect r)
        {
            return new Vector2(r.x, r.yMax);
        }



        public static Rect Lerp(this Rect r, Rect a, Rect b, float t)
        {
            r.x = Mathf.Lerp(a.x, b.x, t);
            r.y = Mathf.Lerp(a.y, b.y, t);
            r.width = Mathf.Lerp(a.width, b.width, t);
            r.height = Mathf.Lerp(a.height, b.height, t);
            return r;
        }
        public static Rect Set(this Rect self, Vector2 position, Vector2 size)
        {
            self.Set(position.x, position.y, size.x, size.y);
            return self;
        }


        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

    }
}
