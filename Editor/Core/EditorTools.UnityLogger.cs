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
            public void Error(object messages, params object[] paras)
            {
                UnityEngine.Debug.LogError(messages);
            }
            public void Exception(Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
            public void Log(object messages, params object[] paras)
            {
                UnityEngine.Debug.Log(messages);
            }
            public void Warn(object messages, params object[] paras)
            {
                UnityEngine.Debug.LogWarning(messages);
            }
            public void Assert(object messages, params object[] paras)
            {
                UnityEngine.Debug.LogAssertion(messages);
            }

        }
    }

}
