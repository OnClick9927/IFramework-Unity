/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IFramework.Localization
{
    class YouDao
    {


        [System.Serializable]
        public class TranslateResult
        {
            public int errorCode;
            public string[] translation;
            public string speakUrl;
            public string requestId;
            public string tSpeakUrl;
            public string l;
            public bool isWord;
            public string query;
        }
        public static async Task<TranslateResult> Translate(string content, string from, string to)
        {
            string appKey = LocalizationSetting.youDaoAppId;
            string appSecret = LocalizationSetting.youDaoAppSecret;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://openapi.youdao.com/api");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //当前UTC时间戳（秒）
            string curtime = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000).ToString();
            //UUID 唯一通用识别码
            string salt = DateTime.Now.Millisecond.ToString();
            string input = content == null ? null : content.Length <= 20 ? content : (content.Substring(0, 10) + content.Length + content.Substring(content.Length - 10, 10));
            byte[] inputBytes = Encoding.UTF8.GetBytes(appKey + input + salt + curtime + appSecret);
            byte[] hashedBytes = new SHA256CryptoServiceProvider().ComputeHash(inputBytes);
            //签名 sha256(应用ID + input + salt + curtime + 应用秘钥)
            //其中input的计算方式为：input=content前10个字符 + content长度 + cotent后10个字符（当cotent长度大于20）或 input=content字符串（当content长度小于等于20）
            string sign = BitConverter.ToString(hashedBytes).Replace("-", "");
            //签名类型
            string signType = "v3";
            //参数列表
            string args = string.Format("from={0}&to={1}&signType={2}&curtime={3}&q={4}&appKey={5}&salt={6}&sign={7}",
                from, to, signType, curtime, content, appKey, salt, sign);
            byte[] data = Encoding.UTF8.GetBytes(args);
            request.ContentLength = data.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                await reqStream.WriteAsync(data, 0, data.Length);
                reqStream.Close();
            }
            var httpWebResponse = await request.GetResponseAsync();
            Stream stream = httpWebResponse.GetResponseStream();
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string responseStr = await reader.ReadToEndAsync();

                var response = JsonUtility.FromJson<TranslateResult>(responseStr);
                return response;
            }
        }

    }
}
