/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Text;

namespace IFramework
{
    public static partial class StringConvert
	{
        public static string ConvertToString(this int self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this short self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this long self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this UInt16 self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this UInt32 self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this UInt64 self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this float self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this double self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this decimal self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this string self)
        {
            return self;
        }
        public static string ConvertToString(this bool self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this char self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this byte self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this DateTime self)
        {
            return self.ToString();
        }
        public static string ConvertToString(this byte[] self)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < self.Length; i++)
                sb.Append(self[i].ToString("X2"));
            return sb.ToString();
        }

        public static bool TryConvert(this string self, out byte[] result)
        {
            if (self.Length %2!= 0) throw new System.Exception("Parse Err Color");
            result = new byte[self.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = byte.Parse(self.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return true;
        }


        public static bool TryConvert(this string self, out DateTime result)
        {
            return DateTime.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out UInt16 result)
        {
            return UInt16.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out UInt32 result)
        {
            return UInt32.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out UInt64 result)
        {
            return UInt64.TryParse(self, out result);
        }
        public static bool TryConvert(this string self,out bool result)
        {
           return  bool.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out float result)
        {
            return float.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out int result)
        {
            return int.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out long result)
        {
            return long.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out short result)
        {
            return short.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out byte result)
        {
            return byte.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out double result)
        {
            return double.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out decimal result)
        {
            return decimal.TryParse(self, out result);
        }
        public static bool TryConvert(this string self, out char result)
        {
            return char .TryParse(self, out result);
        }

    }
}
