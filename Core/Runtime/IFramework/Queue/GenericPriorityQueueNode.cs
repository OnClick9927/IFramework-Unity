namespace IFramework.Queue
{
    /// <summary>
    /// 泛型优先级队列节点
    /// </summary>
    /// <typeparam name="TPriority">优先程度</typeparam>
    public class GenericPriorityQueueNode<TPriority>
    {
        /// <summary>
        /// 节点元素的优先级 <br />
        /// 无法修改 - 请查阅 queue.Enqueue() 和 queue.UpdatePriority()
        /// </summary>
        public TPriority priority { get; protected internal set; }

        /// <summary>
        /// 当前元素在队列中的位置
        /// </summary>
        public int position { get; internal set; }

        /// <summary>
        /// 元素插入时的序号
        /// </summary>
        public long insertPosition { get; internal set; }

    }

}
