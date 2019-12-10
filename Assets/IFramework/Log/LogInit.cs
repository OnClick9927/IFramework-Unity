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
    [OnFrameworkInitClass]
     static class LogInit
    {
        public const string StoName = "LogSetting";
        static LogInit()
        {
            Log.loger = new UnityLoger();
            LogSetting setting = Resources.Load<LogSetting>(StoName);
            if (setting == null) return;
            Log.LogEnable = setting.LogEnable;
            Log.WarnningEnable = setting.WarnningEnable;
            Log.ErrEnable = setting.ErrEnable;
            Log.LogLevel = setting.LogLevel;
            Log.WarnningLevel = setting.WarnningLevel;
            Log.Enable = setting.Enable;
            Log.ErrLevel = setting.ErrLevel;
        }
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
                case LogType.Default:
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
                case LogType.Default:
                    Debug.LogFormat(message as Object, format, paras);
                    break;
            }
        }
    }

}