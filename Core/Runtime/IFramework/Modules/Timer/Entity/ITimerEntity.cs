
using System;

namespace IFramework.Timer
{
    /// <summary>
    /// TimeEntity接口
    /// </summary>
    public interface ITimerEntity
    {
        /// <summary>
        /// 每次调用的等待时间
        /// </summary>
        float repeatDelay { get; }
        /// <summary>
        /// 开始调用的等待时间
        /// </summary>
        float delay { get; }
        /// <summary>
        /// 执行次数
        /// </summary>
        int repeat { get; }
        /// <summary>
        /// 时间比例
        /// </summary>
        float timeScale { get; }
        /// <summary>
        /// 状态
        /// </summary>
        EntityState state { get; }
        /// <summary>
        /// 继续
        /// </summary>
        void Start();
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        /// <summary>
        /// 取消
        /// </summary>
        void Cancel(bool callComplete);
        /// <summary>
        /// 设置子定时器
        /// </summary>
        /// <param name="timerEntity">子定时器</param>
        /// <param name="type">子定时器类型</param>
        void SetInnerTimer(ITimerEntity timerEntity, InnerType type = InnerType.Parallel);
        /// <summary>
        /// 注册
        /// </summary>
        void Subscribe();
        /// <summary>
        /// 设置时间比例
        /// </summary>
        void SetTimeScale(float scale);

        /// <summary>
        /// 注册开始调用的回调方法
        /// </summary>
        /// <param name="startAction">回调方法</param>
        void SubscribeStart(Action startAction);
        /// <summary>
        /// 解绑开始调用的回调方法
        /// </summary>
        /// <param name="startAction">回调方法</param>
        void UnSubscribeStart(Action startAction);
        /// <summary>
        /// 注册每帧的回调方法
        /// </summary>
        /// <param name="updateAction">回调方法</param>
        void SubsribeUpdate(Action updateAction);
        /// <summary>
        /// 解绑每帧的回调方法
        /// </summary>
        /// <param name="updateAction">回调方法</param>
        void UnSubsribeUpdate(Action updateAction);
        /// <summary>
        /// 注册完成的回调方法
        /// </summary>
        /// <param name="completeAction">回调函数</param>
        void SubscribeComplete(Action completeAction);
        /// <summary>
        /// 解绑完成的回调方法
        /// </summary>
        /// <param name="completeAction">回调函数</param>
        void UnSubscribeComplete(Action completeAction);
    }
}
