using System;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface ILoger
    {
        void Log(object messages ,params object[] paras);
        void Warn(object messages, params object[] paras);
        void Error(object messages, params object[] paras);
        void Exception(Exception ex);
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
