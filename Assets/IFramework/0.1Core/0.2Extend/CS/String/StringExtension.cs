/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Linq;
using System.Text;
namespace IFramework
{
    public static partial class StringExtension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static bool IsNullorEmptyAfterTrim(this string self)
        {
            return string.IsNullOrEmpty(self.Trim());
        }

        public static string ReplaceFirst(this string self, string oldValue, string newValue, int startAt = 0)
        {
            var index = self.IndexOf(oldValue, startAt);
            if (index < 0)
            {
                return self;
            }
            return (self.Substring(0, index) + newValue + self.Substring(index + oldValue.Length));
        }
        public static bool ContainsSpace(this string self)
        {
            return self.Contains(" ");
        }
        public static string RemoveString(this string self, params string[] targets)
        {
            return targets.Aggregate(self, (current, t) => current.Replace(t, string.Empty));
        }
        public static string[] SplitAndTrim(this string self, params char[] separator)
        {
            var res = self.Split(separator);
            for (var i = 0; i < res.Length; i++)
            {
                res[i] = res[i].Trim();
            }
            return res;
        }
        public static string Cut(this string self, string front, string behined)
        {
            var startIndex = self.IndexOf(front) + front.Length;
            var endIndex = self.IndexOf(behined);
            if (startIndex < 0 || endIndex < 0)
                return self;
            return self.Substring(startIndex, endIndex - startIndex);
        }
        public static string CutAfter(this string self, string front)
        {
            var startIndex = self.IndexOf(front) + front.Length;
            return startIndex < 0 ? self : self.Substring(startIndex);
        }
        public static string CutBefore(this string str, string behined)
        {
            int index = str.LastIndexOf(behined);
            return str.Substring(0, index);
        }

        public static string UpperFirst(this string self)
        {
            return char.ToUpper(self[0]) + self.Substring(1);
        }
        public static string LowerFirst(this string self)
        {
            return char.ToLower(self[0]) + self.Substring(1);
        }
        public static string ToUnixLineEndings(this string self)
        {
            return self.Replace("\r\n", "\n").Replace("\r", "\n");
        }
        public static string AppendHead(this string self, string toPrefix)
        {
            return new StringBuilder(toPrefix).Append(self).ToString();
        }
        public static string AppendFormat(this string self, string toAppend, params object[] args)
        {
            return new StringBuilder(self).AppendFormat(toAppend, args).ToString();
        }

        public static string Append(this string self, string toAppend)
        {
            return new StringBuilder(self).Append(toAppend).ToString();
        }
        public static string Append(this string self, string[] toAppend)
        {
            return new StringBuilder(self).Append(toAppend).ToString();
        }

        public static string AddSpacedBetwwenWord(this string self)
        {
            var sb = new StringBuilder(self.Length * 2);
            sb.Append(char.ToUpper(self[0]));
            for (var i = 1; i < self.Length; i++)
            {
                if (char.IsUpper(self[i]) && self[i - 1] != ' ')
                {
                    sb.Append(' ');
                }
                sb.Append(self[i]);
            }
            return sb.ToString();
        }
    }
}