/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.Serialization
{
    [OnFrameworkInitClass]
	public static partial class StringConvert_Mono
	{
        static StringConvert_Mono()
        {
            StringConvert.onTryConvert += (string str, Type type, ref object obj) =>
            {
                if (type == typeof(Vector2))
                {
                    Vector2 res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(Vector3))
                {
                    Vector3 res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(Vector4))
                {
                    Vector4 res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(Rect))
                {
                    Rect res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(RectOffset))
                {
                    RectOffset res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(Color))
                {
                    Color res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(SplitType))
                {
                    SplitType res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(FontStyle))
                {
                    FontStyle res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(TextAnchor))
                {
                    TextAnchor res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(TextClipping))
                {
                    TextClipping res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(ImagePosition))
                {
                    ImagePosition res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(ScaleMode))
                {
                    ScaleMode res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                else if (type == typeof(MaterialGlobalIlluminationFlags))
                {
                    MaterialGlobalIlluminationFlags res;
                    if (!str.TryConvert(out res)) return false;
                    obj = res;
                    return true;
                }
                return false;
            };
            StringConvert.onConvertToString += (ref string str, object obj, Type type) =>
            {
                if (type==typeof(Vector2))
                {
                    str = ConvertToString((Vector2)obj);
                    return true;
                }
                else if (type == typeof(Vector3))
                {
                    str = ConvertToString((Vector3)obj);
                    return true;
                }
                else if (type == typeof(Vector4))
                {
                    str = ConvertToString((Vector4)obj);
                    return true;
                }
                else if (type == typeof(Rect))
                {
                    str = ConvertToString((Rect)obj);
                    return true;
                }
                else if (type == typeof(RectOffset))
                {
                    str = ConvertToString((RectOffset)obj);
                    return true;
                }
                else if (type == typeof(Color))
                {
                    str = ConvertToString((Color)obj);
                    return true;
                }
                else if (type == typeof(SplitType))
                {
                    str = ConvertToString((SplitType)obj);
                    return true;
                }
                else if (type == typeof(FontStyle))
                {
                    str = ConvertToString((FontStyle)obj);
                    return true;
                }
                else if (type == typeof(TextAnchor))
                {
                    str = ConvertToString((TextAnchor)obj);
                    return true;
                }
                else if (type == typeof(TextClipping))
                {
                    str = ConvertToString((TextClipping)obj);
                    return true;
                }
                else if (type == typeof(ImagePosition))
                {
                    str = ConvertToString((ImagePosition)obj);
                    return true;
                }
                else if (type == typeof(ScaleMode))
                {
                    str = ConvertToString((ScaleMode)obj);
                    return true;
                }
                else if (type == typeof(MaterialGlobalIlluminationFlags))
                {
                    str = ConvertToString((MaterialGlobalIlluminationFlags)obj);
                    return true;
                }
                return false;
            };

        }
        public static string ConvertToString(this Vector2 self)
        {
           
            return string.Format("{0}X:{1},Y:{2}{3}",
                StringConvert.leftBound,
                self.x,
                self.y,
                StringConvert.rightBound);
        }
        public static bool TryConvert(this string self, out Vector2 result)
        {
            string[] temp = self.Split(',');
            if (temp.Length != 2 || !temp[0].Contains(StringConvert.leftBound) || !temp[1].Contains(StringConvert.rightBound))
                throw new System.Exception("Parse Err Rect");
            float x, y;
            if (temp[0].Split(StringConvert.colon)[1].TryConvert<float>(out x) &&
                 temp[1].Split(StringConvert.colon)[1].Replace(StringConvert.rightBound, "").TryConvert<float>(out y))
            {
                result = new Vector2(x, y);
                return true;
            }
            result = default(Vector2);
            return false;
        }

        public static string ConvertToString(this Vector3 self)
        {
            return string.Format("{0}X:{1},Y:{2},Z:{3}{4}",
                StringConvert.leftBound,
                self.x,
                self.y,
                self.z,
                StringConvert.rightBound);
        }
        public static bool TryConvert(this string self, out Vector3 result)
        {
            string[] temp = self.Split(',');
            if (temp.Length != 3 || !temp[0].Contains(StringConvert.leftBound) || !temp[2].Contains(StringConvert.rightBound))
                throw new System.Exception("Parse Err Rect");
            float x, y, z;
            if (temp[0].Split(StringConvert.colon)[1].TryConvert<float>(out x) &&
                 temp[1].Split(StringConvert.colon)[1].TryConvert<float>(out y) &&
                 temp[2].Split(StringConvert.colon)[1].Replace(StringConvert.rightBound, "").TryConvert<float>(out z))
            {
                result = new Vector3(x, y, z);
                return true;
            }
            result = default(Vector3);
            return false;
        }
        public static string ConvertToString(this Vector4 self)
        {
            return string.Format("{0}X:{1},Y:{2},Z:{3},W:{4}{5}",
                StringConvert.leftBound,
                self.x,
                self.y,
                self.z,
                self.w,
                StringConvert.rightBound);
        }
        public static bool TryConvert(this string self, out Vector4 result)
        {
            string[] temp = self.Split(',');
            if (temp.Length != 4 || !temp[0].Contains(StringConvert.leftBound) || !temp[3].Contains(StringConvert.rightBound))
                throw new System.Exception("Parse Err Rect");
            float x, y, z, w;
            if (temp[0].Split(StringConvert.colon)[1].TryConvert<float>(out x) &&
                 temp[1].Split(StringConvert.colon)[1].TryConvert<float>(out y) &&
                 temp[2].Split(StringConvert.colon)[1].TryConvert<float>(out z) &&
                 temp[3].Split(StringConvert.colon)[1].Replace(StringConvert.rightBound, "").TryConvert<float>(out w))
            {
                result = new Vector4(x, y, z, w);
                return true;
            }
            result = default(Vector4);
            return false;
        }
        public static string ConvertToString(this Rect self)
        {
            return string.Format("{0}X:{1},Y:{2},W:{3},H:{4}{5}",
                StringConvert.leftBound,
                self.x,
                self.y,
                self.width,
                self.height,
                StringConvert.rightBound );
        }
        public static bool TryConvert(this string self, out Rect result)
        {
            string[] temp = self.Split(',');
            if (temp.Length != 4 || !temp[0].Contains(StringConvert.leftBound)|| !temp[3].Contains(StringConvert.rightBound))
                throw new System.Exception("Parse Err Rect");
            float x, y, w, h;
            if ( temp[0].Split(StringConvert.colon)[1].TryConvert<float>(out x)&& 
                 temp[1].Split(StringConvert.colon)[1].TryConvert<float>(out y)&&
                 temp[2].Split(StringConvert.colon)[1].TryConvert<float>(out w)&&
                 temp[3].Split(StringConvert.colon)[1].Replace(StringConvert.rightBound, "").TryConvert<float>(out h)    )
            {
                result = new Rect(x,y,w,h);
                return true;
            }
            result = default(Rect);
            return false;
        }
        public static string ConvertToString(this RectOffset self)
        {
            return string.Format("{0}L:{1},R:{2},T:{3},B:{4}{5}",
                                StringConvert.leftBound,
                                self.left,
                                self.right,
                                self.top,
                                self.bottom,
                                StringConvert.rightBound);
        }
        public static bool TryConvert(this string self, out RectOffset result)
        {
            string[] temp = self.Split(',');
            if (temp.Length != 4 || !temp[0].Contains(StringConvert.leftBound) || !temp[3].Contains(StringConvert.rightBound))
                throw new System.Exception("Parse Err RectOffset");
            int L, R, T, B;
            if (temp[0].Split(StringConvert.colon)[1].TryConvert<int>(out L) &&
                 temp[1].Split(StringConvert.colon)[1].TryConvert<int>(out R) &&
                 temp[2].Split(StringConvert.colon)[1].TryConvert<int>(out T) &&
                 temp[3].Split(StringConvert.colon)[1].Replace(StringConvert.rightBound, "").TryConvert<int>(out B))
            {
                result = new RectOffset(L, R, T, B);
                return true;
            }
            result = default(RectOffset);
            return false;
        }

        public static string ConvertToString(this Color self)
        {
            int r = Mathf.RoundToInt(self.r * 255.0f);
            int g = Mathf.RoundToInt(self.g * 255.0f);
            int b = Mathf.RoundToInt(self.b * 255.0f);
            int a = Mathf.RoundToInt(self.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }
        public static bool TryConvert(this string self, out Color result)
        {
            if (self.Length != 8) throw new System.Exception("Parse Err Color");
            byte br = byte.Parse(self.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(self.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(self.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte cc = byte.Parse(self.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            float a = cc / 255f;
            result= new Color(r, g, b, a);
            return true;
        }

        public static string ConvertToString(this SplitType self)
        {
            return self.ToString();
        }
        public static bool TryConvert(this string self, out SplitType result)
        {
            result=(SplitType) System.Enum.Parse(typeof(SplitType), self);
            return true;
        }
        public static string ConvertToString(this FontStyle self)
        {
            return self.ToString();
        }
        public static bool TryConvert(this string self, out FontStyle result)
        {
            result = (FontStyle)System.Enum.Parse(typeof(FontStyle), self);
            return true;
        }
        public static string ConvertToString(this TextAnchor self)
        {
            return self.ToString();
        }
        public static bool TryConvert(this string self, out TextAnchor result)
        {
            result = (TextAnchor)System.Enum.Parse(typeof(TextAnchor), self);
            return true;
        }
        public static string ConvertToString(this TextClipping self)
        {
            return self.ToString();
        }
        public static bool TryConvert(this string self, out TextClipping result)
        {
            result = (TextClipping)System.Enum.Parse(typeof(TextClipping), self);
            return true;
        }
        public static string ConvertToString(this ImagePosition self)
        {
            return self.ToString();
        }
        public static bool TryConvert(this string self, out ImagePosition result)
        {
            result = (ImagePosition)System.Enum.Parse(typeof(ImagePosition), self);
            return true;
        }
        public static string ConvertToString(this ScaleMode self)
        {
            return self.ToString();
        }
        public static bool TryConvert(this string self, out ScaleMode result)
        {
            result = (ScaleMode)System.Enum.Parse(typeof(ScaleMode), self);
            return true;
        }
        public static string ConvertToString(this MaterialGlobalIlluminationFlags self)
        {
            return self.ToString();
        }
        public static bool TryConvert(this string self, out MaterialGlobalIlluminationFlags result)
        {
            result = (MaterialGlobalIlluminationFlags)System.Enum.Parse(typeof(MaterialGlobalIlluminationFlags), self);
            return true;
        }
        
    }
}
