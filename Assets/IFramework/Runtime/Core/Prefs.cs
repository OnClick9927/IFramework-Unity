﻿using System;
using System.Collections.Generic;
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
            private static string EditPath => "Assets/Editor/Prefs{0}.txt";
            private static string EditPath2 => "Assets/Editor/PrefsE{0}.txt";
            public string Read(string key)
            {
                var _path = compress ? EditPath2 : EditPath;
                _path = string.Format(_path, key);
                if (File.Exists(_path))
                    return File.ReadAllText(_path);
                return string.Empty;
            }

            public void Save(string key, string value)
            {
#if UNITY_EDITOR
                var _path = compress ? EditPath2 : EditPath;
                _path = string.Format(_path, key);

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

        private static Dictionary<string, Pairs> pairMap = new Dictionary<string, Pairs>();

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
            Init(key);
        }

        private static void Init(string key)
        {
            string value = GetRecorder().Read(key);
            Pairs pairs = PairFromString(value);
            if (pairMap.ContainsKey(key))
                pairMap[key] = pairs;
            else
                pairMap.Add(key, pairs);
        }
        public static string GetPairString(string key)
        {
            if (pairMap.TryGetValue(key, out var pairs))
                return PairToString(pairs);
            return string.Empty;
        }
        public static void SetPair(string key, string value)
        {
            var pairs = PairFromString(value);
            if (pairMap.ContainsKey(key))
                pairMap[key] = pairs;
            else
                pairMap.Add(key, pairs);
            SavePair(key, pairs);
        }


        public static void Save<T>(string key, T Obj) => Save<T>(Prefs.key, key, Obj);
        public static void Save<T>(string key_1, string key_2, T Obj)
        {
            var value = ObjectToString(Obj);

            if (pairMap.TryGetValue(key_1, out var pairs))
            {
                bool change = pairs.Save(key_2, value);
                if (change) SavePair(key_1, pairs);
            }
            else
            {
                Init(key_1);
                Save(key_1, key_2, Obj);
            }
        }
        private static void SavePair(string key, Pairs pairs) => GetRecorder().Save(key, PairToString(pairs));
        private static Pairs PairFromString(string value)
        {
            Pairs pairs = new Pairs();

            if (!string.IsNullOrEmpty(value))
            {
                if (compress)
                    value = DataCompress.GZipDecompressString(value);
                pairs = StringToObject<Pairs>(value);
            }
            return pairs;
        }

        private static string PairToString(Pairs pairs)
        {
            var str = ObjectToString(pairs);
            if (compress)
                str = DataCompress.GZipCompressString(str);
            return str;
        }


        public static T Read<T>(string key) => Read<T>(Prefs.key, key);
        public static T Read<T>(string key_1, string key_2)
        {
            if (pairMap.TryGetValue(key_1, out var pairs))
            {
                var str = pairs.Get(key_2);
                if (string.IsNullOrEmpty(str))
                    return default;
                return StringToObject<T>(str);
            }
            return default;
        }
        private static string ObjectToString<T>(T t) => JsonUtility.ToJson(t, true);
        private static T StringToObject<T>(string json) => JsonUtility.FromJson<T>(json);
    }

}