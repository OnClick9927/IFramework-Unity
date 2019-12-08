/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    public class Log
    {
        public const string StoName = "LogSetting";
        public static int LogLevel = 0;
        public static int WarnningLevel = 0;
        public static int ErrLevel = 0;
        public static bool Enable = true;
        public static bool LogEnable = true;
        public static bool WarnningEnable = true;
        public static bool ErrEnable = true;
        public static ILoger loger { get; set; }
        static Log()
        {
            ReadInfo();
            loger = new UnityLoger();
        }
        public static void ReadInfo()
        {
            LogSetting s = Resources.Load<LogSetting>(StoName);
            LogEnable = s.LogEnable;
            WarnningEnable = s.WarnningEnable;
            ErrEnable = s.ErrEnable;
            LogLevel = s.LogLevel;
            WarnningLevel = s.WarnningLevel;
            Enable = s.Enable;
            ErrLevel = s.ErrLevel;
        }

        public static void L(object message, int lev = 0, params object[] paras)
        {
            if (!Enable) return;
            if (!LogEnable) return;
            if (LogLevel > lev) return;
            loger.Log(LogType.Log, message, paras);
        }
        public static void W(object message, int lev = 50, params object[] paras)
        {
            if (!Enable) return;
            if (!WarnningEnable) return;
            if (WarnningLevel > lev) return;
            loger.Log(LogType.Warning, message, paras);
        }
        public static void E(object message, int lev = 100, params object[] paras)
        {
            if (!Enable) return;
            if (!ErrEnable) return;

            if (ErrLevel > lev) return;
            loger.Log(LogType.Error, message, paras);
        }

        public static void LF(object message, string format, int lev = 0, params object[] paras)
        {
            if (!Enable) return;
            if (!LogEnable) return;
            if (LogLevel > lev) return;
            loger.LogFormat(LogType.Log, format, message, paras);
        }
        public static void WF(object message, string format, int lev = 50, params object[] paras)
        {
            if (!Enable) return;
            if (!WarnningEnable) return;
            if (WarnningLevel > lev) return;
            loger.LogFormat(LogType.Warning, format, message, paras);
        }
        public static void EF(object message, string format, int lev = 100, params object[] paras)
        {
            if (!Enable) return;
            if (!ErrEnable) return;
            if (ErrLevel > lev) return;
            loger.LogFormat(LogType.Error, format, message, paras);
        }

    }

    public interface ILoger
    {
        void Log(LogType logType, object message, params object[] paras);
        void LogFormat(LogType logType, string format, object message, params object[] paras);
    }
    public class UnityLoger : ILoger
    {
        public void Log(LogType logType, object message, params object[] paras)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Log:
                    Debug.Log(message);
                    break;
            }
        }

        public void LogFormat(LogType logType, string format, object message, params object[] paras)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogErrorFormat(message as Object, format, paras);
                    break;
                case LogType.Warning:
                    Debug.LogWarningFormat(message as Object, format, paras);
                    break;
                case LogType.Log:
                    Debug.LogFormat(message as Object, format, paras);
                    break;
            }
        }
    }
    public interface ILog { }
    public static class LogExtension
    {
        public static void Log(this string message, int lev = 0, params object[] paras)
        {
            IFramework.Log.L(message, lev, paras);
        }
        public static void Warning(this string message, int lev = 50, params object[] paras)
        {
            IFramework.Log.W(message, lev, paras);
        }
        public static void Err(this string message, int lev = 100, params object[] paras)
        {
            IFramework.Log.E(message, lev, paras);
        }

        public static void LogFormat(this string message, string format, int lev = 0, params object[] paras)
        {
            IFramework.Log.LF(message, format, lev, paras);
        }
        public static void WarningFormat(this string message, string format, int lev = 50, params object[] paras)
        {
            IFramework.Log.WF(message, format, lev, paras);
        }
        public static void ErrFormat(this string message, string format, int lev = 100, params object[] paras)
        {
            IFramework.Log.EF(message, format, lev, paras);
        }

        public static void Log(this ILog self, object message, int lev = 0, params object[] paras)
        {
            IFramework.Log.L(message, lev, paras);
        }
        public static void Warning(this ILog self, object message, int lev =50, params object[] paras)
        {
            IFramework.Log.W(message, lev, paras);
        }
        public static void Err(this ILog self, object message, int lev = 100, params object[] paras)
        {
            IFramework.Log.E(message, lev, paras);
        }

        public static void LogFormat(this ILog self, object message, string format, int lev = 0, params object[] paras)
        {
            IFramework.Log.LF(message,format, lev, paras);
        }
        public static void WarningFormat(this ILog self, object message, string format, int lev = 50, params object[] paras)
        {
            IFramework.Log.WF(message, format, lev, paras);
        }
        public static void ErrFormat(this ILog self, object message, string format, int lev = 100, params object[] paras)
        {
            IFramework.Log.EF(message, format, lev, paras);
        }
    }
}