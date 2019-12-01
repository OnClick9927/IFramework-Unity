/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
namespace IFramework
{
    public static partial class StringConvert
    {

        private const string dot = ",";
        private const string leftBound = "[ ";
        private const string rightBound = " ]";
        private const char colon = ':';
        public static string ConvertToString<T>(this T self)
        {
            return self.ConvertToString(typeof(T));
        }
        public static string ConvertToString(this object self, Type type)
        {
            if (self == null) return string.Empty;
            else if (type == typeof(string)) return ((string)self).ConvertToString();
            else if (type == typeof(int)) return ((int)self).ConvertToString();
            else if (type == typeof(float)) return ((float)self).ConvertToString();
            else if (type == typeof(double)) return ((double)self).ConvertToString();
            else if (type == typeof(decimal)) return ((decimal)self).ConvertToString();
            else if (type == typeof(bool)) return ((bool)self).ConvertToString();
            else if (type == typeof(char)) return ((char)self).ConvertToString();
            else if (type == typeof(long)) return ((long)self).ConvertToString();
            else if (type == typeof(short)) return ((short)self).ConvertToString();
            else if (type == typeof(byte)) return ((byte)self).ConvertToString();
            else if (type == typeof(ushort)) return ((ushort)self).ConvertToString();
            else if (type == typeof(uint)) return ((uint)self).ConvertToString();
            else if (type == typeof(ulong)) return ((ulong)self).ConvertToString();
            else if (type == typeof(DateTime)) return ((DateTime)self).ConvertToString();
            else if (type == typeof(byte[])) return ((byte[])self).ConvertToString();
            else if (type == typeof(Rect)) return ((Rect)self).ConvertToString();
            else if (type == typeof(Vector2)) return ((Vector2)self).ConvertToString();
            else if (type == typeof(SplitType)) return ((SplitType)self).ConvertToString();
            else if (type == typeof(Color)) return ((Color)self).ConvertToString();
            else if (type == typeof(RectOffset)) return ((RectOffset)self).ConvertToString();
            else if (type == typeof(FontStyle)) return ((FontStyle)self).ConvertToString();
            else if (type == typeof(TextAnchor)) return ((TextAnchor)self).ConvertToString();
            else if (type == typeof(TextClipping)) return ((TextClipping)self).ConvertToString();
            else if (type == typeof(ImagePosition)) return ((ImagePosition)self).ConvertToString();
            else if (type == typeof(ScaleMode)) return ((ScaleMode)self).ConvertToString();
            else if (type == typeof(Vector4)) return ((Vector4)self).ConvertToString();
            else if (type == typeof(Vector3)) return ((Vector3)self).ConvertToString();
            else if (type == typeof(MaterialGlobalIlluminationFlags)) return ((MaterialGlobalIlluminationFlags)self).ConvertToString();

            else
                throw new Exception(string.Format("Can't Convert Type {0} ToString", type));
        }

        public static bool TryConvert<T>(this string self, out T t)
        {
            object obj = default(object);
            if (self.TryConvert(typeof(T), ref obj))
            {
                t = (T)obj;
                return true;
            }
            else
            {
                t = default(T);
                return false;
            }
        }
        public static bool TryConvert(this string self, Type type, ref object obj)
        {

            if (string.IsNullOrEmpty(self)) return false;
            if (type == typeof(object))
            {
                Type t = self.GetType();
                if (t == typeof(object))
                    return false;
                return TryConvert(self, t, ref obj);
            }
            else if (type == typeof(string))
            {
                obj = self;
                return true;
            }
            else if (type == typeof(int))
            {
                int res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(float))
            {
                float res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(double))
            {
                double res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(decimal))
            {
                decimal res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(bool))
            {
                bool res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(char))
            {
                char res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(long))
            {
                long res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(short))
            {
                short res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(byte))
            {
                byte res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(UInt16))
            {
                UInt16 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(UInt32))
            {
                UInt32 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(UInt64))
            {
                UInt64 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(DateTime))
            {
                DateTime res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(byte[]))
            {
                byte[] res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }




            else if (type == typeof(Rect))
            {
                Rect res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(Vector2))
            {
                Vector2 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(SplitType))
            {
                SplitType res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(Color))
            {
                Color res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(RectOffset))
            {
                RectOffset res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(FontStyle))
            {
                FontStyle res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(TextAnchor))
            {
                TextAnchor res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(TextClipping))
            {
                TextClipping res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(ImagePosition))
            {
                ImagePosition res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(ScaleMode))
            {
                ScaleMode res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(Vector4))
            {
                Vector4 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(Vector3))
            {
                Vector3 res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else if (type == typeof(MaterialGlobalIlluminationFlags))
            {
                MaterialGlobalIlluminationFlags res;
                if (!self.TryConvert(out res)) return false;
                obj = res;
                return true;
            }
            else
                throw new Exception(string.Format("Can't Convert String {0} To Type {1}", self, type));
        }
    }
}
