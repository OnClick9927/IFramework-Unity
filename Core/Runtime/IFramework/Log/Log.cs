using System;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class Log
    {
        public static bool enable = true;
        public static bool enable_L = true;
        public static bool enable_W = true;
        public static bool enable_E = true;
        public static ILoger loger { get; set; }
        public static ILogRecorder recorder { get; set; }
        static Log()
        {
            loger = new CSLogger();
           
        }

        public static void L(object message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_L) return;
            if (loger!=null)
                loger.Log(message, paras);
            if (recorder!=null)
                recorder.Log(message, paras);
        }
        public static void W(object message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_W) return;
            if (loger!=null)
                loger.Warn(message, paras);
            if (recorder != null)
                recorder.Warn(message, paras);
        }
        public static void E(object message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_E) return;
            if (loger != null)
                loger.Error( message, paras);
            if (recorder != null)
                recorder.Log(message, paras);
        }
        public static void Exception(Exception ex)
        {
            if (!enable) return;
            if (!enable_E) return;
            if (loger != null)
                loger.Exception(ex);
            if (recorder != null)
                recorder.Exception(ex);
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
