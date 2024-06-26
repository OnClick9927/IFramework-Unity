using System;
using System.IO;
using UnityEngine;
namespace IFramework
{
    public class Prefs
    {
        static class DataCompress
        {
            /// <summary>
            /// 将传入字符串以GZip算法压缩后，返回Base64编码字符
            /// </summary>
            /// <param name="rawString">需要压缩的字符串</param>
            /// <returns>压缩后的Base64编码的字符串</returns>
            public static string GZipCompressString(string rawString)
            {
                if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
                {
                    return "";
                }
                else
                {
                    byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                    byte[] zippedData = Compress(rawData);
                    return Convert.ToBase64String(zippedData);
                }
            }

            /// <summary>
            /// GZip压缩
            /// </summary>
            /// <param name="rawData"></param>
            /// <returns></returns>
            static byte[] Compress(byte[] rawData)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
                compressedzipStream.Write(rawData, 0, rawData.Length);
                compressedzipStream.Close();
                return ms.ToArray();
            }


            /// <summary>
            /// 将传入的二进制字符串资料以GZip算法解压缩
            /// </summary>
            /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
            /// <returns>原始未压缩字符串</returns>
            public static string GZipDecompressString(string zippedString)
            {
                if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
                {
                    return "";
                }
                else
                {
                    byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
                    return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
                }
            }

            /// <summary>
            /// ZIP解压
            /// </summary>
            /// <param name="zippedData"></param>
            /// <returns></returns>
            private static byte[] Decompress(byte[] zippedData)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(zippedData);
                System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
                System.IO.MemoryStream outBuffer = new System.IO.MemoryStream();
                byte[] block = new byte[1024];
                while (true)
                {
                    int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                        break;
                    else
                        outBuffer.Write(block, 0, bytesRead);
                }
                compressedzipStream.Close();
                return outBuffer.ToArray();

            }
        }

        public interface IPrefsRecorder
        {
            void Save(string key, string value);
            string Read(string key);
        }
        private class EditorPrefsRecorder : IPrefsRecorder
        {
            private static string EditPath => $"Assets/Editor/AOTP{key}.txt";
            private static string EditPath2 => $"Assets/Editor/AOTPE{key}.txt";
            public string Read(string key)
            {
                var _path = compress ? EditPath2 : EditPath;
                if (File.Exists(_path))
                    return File.ReadAllText(_path);
                return string.Empty;
            }

            public void Save(string key, string value)
            {
#if UNITY_EDITOR
                var _path = compress ? EditPath2 : EditPath;
                File.WriteAllText(_path, value);
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }
        private class PlayerPrefsRecorder : IPrefsRecorder
        {

            public string Read(string key)
            {
                return PlayerPrefs.GetString(key);
            }

            public void Save(string key, string value)
            {
                PlayerPrefs.SetString(key, value);
            }
        }


        private static IPrefsRecorder recorder = new PlayerPrefsRecorder();
        private static IPrefsRecorder recorder_editor = new EditorPrefsRecorder();

        [System.Serializable]
        private class Pairs
        {
            [UnityEngine.SerializeField]
            private SerializableDictionary<string, string> _pairs2
                = new SerializableDictionary<string, string>();
            public string Get(string key)
            {
                if (_pairs2.ContainsKey(key))
                    return _pairs2[key];
                return null;
            }
            public bool Save(string key, string value)
            {
                string _str;
                if (_pairs2.TryGetValue(key, out _str))
                {
                    if (_str == value) return false;
                    _pairs2[key] = value;
                    return true;
                }
                else
                    _pairs2.Add(key, value);
                return true;
            }


        }
        private static Pairs pairs = new Pairs();
        private static string key = "Perfs";
        private static bool compress = false;
        public static void SetPrefsRecorder(IPrefsRecorder _recorder)
        {
            recorder = _recorder;
        }
        private static IPrefsRecorder GetRecorder()
        {
#if UNITY_EDITOR
            return recorder_editor;
#else
            return recorder;
#endif
        }
        public static void SetCompress(bool _com)
        {
            compress = _com;
        }
        public static void SetKey(string key)
        {
            Prefs.key = key;
            Init();
        }

        private static void Init()
        {
            string value = GetRecorder().Read(key);
            if (!string.IsNullOrEmpty(value))
            {
                if (compress)
                    value = DataCompress.GZipDecompressString(value);
                pairs = JsonUtility.FromJson<Pairs>(value);
            }
            else
            {
                pairs = new Pairs();
            }
        }



        public static void Save<T>(string key, T Obj)
        {
            var value = JsonUtility.ToJson(Obj);
            bool change = pairs.Save(key, value);
            if (change) Save();
        }
        public static T Read<T>(string key)
        {
            var str = pairs.Get(key);
            if (string.IsNullOrEmpty(str))
                return default;
            return JsonUtility.FromJson<T>(str);
        }
        private static void Save()
        {
            var str = JsonUtility.ToJson(pairs);
            if (compress)
                str = DataCompress.GZipCompressString(str);
            GetRecorder().Save(key, str);
        }
    }

}
