/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework.Regex
 *History:        2018.11--
*********************************************************************************/
using System.Text.RegularExpressions;

namespace IFramework
{
    public static partial class StringExtension_Regex
    {
        public static bool IsMail(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^\s*([A-Za-z0-9_-]+(\.\w+)*@(\w+\.)+\w{2,5})s*$");
        }
        public static bool IsCellPhoneNumber(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^[1]+[3,5]+\d{9}");
        }
        public static bool IsAge(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^\d{1,2}$");
        }
        public static bool IsPassWord(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return new Regex(@"^[a-zA-Z]\w{5,15}$").IsMatch(self);
        }
        public static bool HasChinese(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"[\u4e00-\u9fa5]");
        }
        public static bool IsUrl(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"[a-zA-z]+://[^\s]*");
        }
        public static bool IsIPv4(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"((25[0 - 5] | 2[0 - 4]\d | ((1\d{ 2})| ([1 - 9] ?\d)))\.){ 3} (25[0 - 5] | 2[0 - 4]\d | ((1\d{ 2})| ([1 - 9] ?\d)))");
        }
        public static bool IsLegalFieldName(this string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^[_a-zA-Z][_a-zA-Z0-9]*$");
        }

        public static string RemoveChinese(this string self)
        {
            if (Regex.IsMatch(self, @"[\u4e00-\u9fa5]"))
            {
                string retValue = string.Empty;
                var strsStrings = self.ToCharArray();
                for (int index = 0; index < strsStrings.Length; index++)
                {
                    if (strsStrings[index] >= 0x4e00 && strsStrings[index] <= 0x9fa5) continue;
                    retValue += strsStrings[index];
                }
                return retValue;
            }
            else
                return self;
        }
        public static string Removeletters(this string self)
        {
            return Regex.Replace(self, "[a-zA_Z]", "", RegexOptions.IgnoreCase);
        }
        public static string RemoveNumbers(this string self)
        {
            return Regex.Replace(self, "[0,9]", "", RegexOptions.IgnoreCase);
        }
        public static string RemoveNumbers(this string self, int minLen, int maxLen)
        {
            return Regex.Replace(self, string.Format("[0,9]{{0},{1}}", minLen, maxLen), "", RegexOptions.IgnoreCase);
        }
        public static string RemoveNumbers(this string self, int minLen)
        {
            return Regex.Replace(self, string.Format("[0,9]{{0},}", minLen), "", RegexOptions.IgnoreCase);
        }

        public static string RemoveNotNumber(this string self)
        {
            return Regex.Replace(self, @"[^\d]*", "", RegexOptions.IgnoreCase);
        }

    }
}
