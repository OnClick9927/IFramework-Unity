using System;

namespace IFramework
{
    public interface ILogger
    {
        void Log(object messages, params object[] paras);
        void Warn(object messages, params object[] paras);
        void Error(object messages, params object[] paras);
        void Exception(Exception ex);
        void Assert(object messages, params object[] paras);
    }
    public class Log
    {
        public static bool enable = true;
        public static bool enable_L = true;
        public static bool enable_W = true;
        public static bool enable_E = true;
        public static ILogger logger { get; set; }
        static Log()
        {
          
           
        }

        public static void L(object message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_L) return;
            if (logger!=null)
                logger.Log(message, paras);
 
        }
        public static void W(object message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_W) return;
            if (logger!=null)
                logger.Warn(message, paras);

        }
        public static void E(object message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_E) return;
            if (logger != null)
                logger.Error( message, paras);

        }
        public static void Exception(Exception ex)
        {
            if (!enable) return;
            if (!enable_E) return;
            if (logger != null)
                logger.Exception(ex);

        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
