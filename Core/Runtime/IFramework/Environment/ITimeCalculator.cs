using System;

namespace IFramework
{
    /// <summary>
    /// 时间计算
    /// </summary>
    public interface ITimeCalculator
    {
        /// <summary>
        /// 每次刷新的时间
        /// </summary>
        TimeSpan deltaTime { get; }
        /// <summary>
        /// 初始化-现在时间
        /// </summary>
        TimeSpan timeSinceInit { get; }
    }
}
