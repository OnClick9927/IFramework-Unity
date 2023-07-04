namespace IFramework.Queue
{
    /// <summary>
    /// 快速优先级队列节点
    /// </summary>
    public class FastPriorityQueueNode
    {
        /// <summary>
        /// 节点元素的优先级 <br />.
        /// 无法修改 - 请查阅 queue.Enqueue() 和 queue.UpdatePriority()
        /// </summary>
        public float priority { get; protected internal set; }

        /// <summary>
        /// 当前元素在队列里的位置
        /// </summary>
        public int position { get; internal set; }
    }

}
