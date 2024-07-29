using System;

namespace IFramework
{
    partial class EditorTools
    {
        public class UnityLogger : ILogger
        {
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
