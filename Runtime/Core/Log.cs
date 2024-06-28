using System;

namespace IFramework
{
    public interface ILogger
    {
        void Log(string messages, params object[] paras);
        void Warn(string messages, params object[] paras);
        void Error(string messages, params object[] paras);
        void Exception(Exception ex);
        void Assert(bool condition, string messages, params object[] paras);
    }
    public class Log
    {
        public static bool enable = true;
        public static bool enable_L = true;
        public static bool enable_W = true;
        public static bool enable_E = true;
        public static bool enable_A = true;

        public static ILogger logger { get; set; }
        static Log()
        {


        }

        public static void L(string message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_L) return;
            if (logger != null)
                logger.Log(message, paras);

        }
        public static void W(string message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_W) return;
            if (logger != null)
                logger.Warn(message, paras);

        }
        public static void E(string message, params object[] paras)
        {
            if (!enable) return;
            if (!enable_E) return;
            if (logger != null)
                logger.Error(message, paras);

        }
        public static void Exception(Exception ex)
        {
            if (!enable) return;
            if (!enable_E) return;
            if (logger != null)
                logger.Exception(ex);

        }
        public static void A(bool condition, string messages, params object[] paras)
        {
            if (!enable) return;
            if (!enable_A) return;
            if (logger != null)
                logger.Assert(condition, messages, paras);

        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
