using System;

namespace IFramework.Queue
{
    /// <summary>
    /// 可变大小的优先级队列
    /// </summary>
    /// <typeparam name="TItem">队列元素</typeparam>
    /// <typeparam name="TPriority">紧急程度</typeparam>
    internal interface IFixedSizePriorityQueue<TItem, in TPriority> : IPriorityQueue<TItem, TPriority> where TPriority : IComparable<TPriority>
    {
        /// <summary>
        /// 重新设置队列的大小，使其可以容纳更多的节点，当前的所有节点都会保留<br />
        /// 如果试图将队列大小设置成比当前队列中的数量小时，会导致未定义的操作
        /// </summary>
        /// <param name="maxNodes">要设置的最大数量</param>
        void Resize(int maxNodes);

        /// <summary>
        /// 队列中可以入队的最大数量<br />
        /// 如果入队数量超过队列的大小(Count == MaxSize时Enqueue())，会导致未定义的操作
        /// </summary>
        int capcity { get; }

        /// <summary>
        /// 默认情况下在队列中的元素不能添加到另一个队列<br />
        /// 如果需要这么做，则在添加到另一个队列前调用当前队列的此方法
        /// </summary>
        void ResetNode(TItem node);
    }

}
