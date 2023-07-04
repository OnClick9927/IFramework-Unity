using System;
using System.Collections.Generic;

namespace IFramework.Queue
{
    /// <summary>
    /// 优先级队列接口
    /// </summary>
    /// <typeparam name="TItem">队列元素</typeparam>
    /// <typeparam name="TPriority">紧急程度</typeparam>
    public interface IPriorityQueue<TItem, in TPriority> : IEnumerable<TItem> where TPriority : IComparable<TPriority>
    {
        /// <summary>
        /// 将优先级节点入队，优先级值小的将会排在队列前面(越小越紧急)
        /// </summary>
        void Enqueue(TItem node, TPriority priority);

        /// <summary>
        /// 将队列的第一个元素从队列中出队
        /// </summary>
        TItem Dequeue();

        /// <summary>
        /// 清除队列里的所有元素
        /// </summary>
        void Clear();

        /// <summary>
        /// 判断队列中是否包含所给元素
        /// </summary>
        /// <param name="node">需要判断是否包含的元素</param>
        /// <returns>如果包含则返回true，否则返回false</returns>
        bool Contains(TItem node);

        /// <summary>
        /// 在队列中移除匹配的第一个的所给元素
        /// </summary>
        void Remove(TItem node);

        /// <summary>
        /// 更新节点元素的优先程度
        /// </summary>
        void UpdatePriority(TItem node, TPriority priority);

        /// <summary>
        /// 队列的第一个元素
        /// </summary>
        TItem first { get; }

        /// <summary>
        /// 元素数量
        /// </summary>
        int count { get; }
    }
}
