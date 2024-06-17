namespace IFramework
{
    interface IGenericPriorityQueueNode<TPriority>
    {
        /// <summary>
        /// 节点元素的优先级 <br />
        /// 无法修改 - 请查阅 queue.Enqueue() 和 queue.UpdatePriority()
        /// </summary>
        TPriority priority { get; protected internal set; }

        /// <summary>
        /// 当前元素在队列中的位置
        /// </summary>
        int position { get; internal set; }

        /// <summary>
        /// 元素插入时的序号
        /// </summary>
        long insertPosition { get; internal set; }

    }

}
