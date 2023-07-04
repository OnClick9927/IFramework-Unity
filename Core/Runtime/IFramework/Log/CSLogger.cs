using System;

namespace IFramework
{
    class CSLogger : ILoger
    {
        public void Error(object messages, params object[] paras)
        {
            Console.WriteLine(string.Format("[Err] {0}:\t{1}", DateTime.Now.ToString(), messages), paras);
        }

        public void Exception(Exception ex)
        {
            throw ex;
        }

        public void Log(object messages, params object[] paras)
        {
            Console.WriteLine(string.Format("[Log] {0}:\t{1}", DateTime.Now.ToString(), messages), paras);
        }

        public void Warn(object messages, params object[] paras)
        {
            Console.WriteLine(string.Format("[Warn] {0}:\t{1}", DateTime.Now.ToString(), messages), paras);
        }
    }
}
