namespace IFramework.Queue
{
    /// <summary>
    /// 稳定优先级队列节点
    /// </summary>
    public class StablePriorityQueueNode : FastPriorityQueueNode
    {
        /// <summary>
        /// 元素插入时的序号
        /// </summary>
        public long insertPosition { get; internal set; }
    }
}
