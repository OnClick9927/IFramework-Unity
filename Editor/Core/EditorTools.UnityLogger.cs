using System;
using System.Runtime.CompilerServices;

namespace IFramework
{
    partial class EditorTools
    {
        public class UnityLogger : ILogger
        {
            public static string GetLogFilePath()
            {
                return GetFilePath().ToAssetsPath();
            }
            private static string GetFilePath([CallerFilePath] string path = "")
            {
                return path;
            }
            public void Error(string messages, params object[] paras)
            {
                UnityEngine.Debug.LogErrorFormat(messages, paras);
            }
            public void Exception(Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
            public void Log(string messages, params object[] paras)
            {
                UnityEngine.Debug.LogFormat(messages, paras);
            }
            public void Warn(string messages, params object[] paras)
            {
                UnityEngine.Debug.LogWarningFormat(messages, paras);
            }
            public void Assert(bool condition, string messages, params object[] paras)
            {
                UnityEngine.Debug.AssertFormat(condition, messages, paras);
            }

        }
    }

}
