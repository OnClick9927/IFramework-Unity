/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Security.Cryptography;
using System.Text;

namespace IFramework.Net
{
    public static partial class StringExtension
    {
        public static string ToSha1Base64(this string value, Encoding encoding)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes = sha1.ComputeHash(encoding.GetBytes(value));
            return Convert.ToBase64String(bytes);
        }

        public static string ToMd5(this string value, Encoding encoding)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(encoding.GetBytes(value));
            return Convert.ToBase64String(bytes);
        }
    }
}
