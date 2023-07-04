using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Queue
{
    /// <summary>
    /// 简易优先级队列
    /// </summary>
    /// <typeparam name="TItem">队列元素</typeparam>
    /// <typeparam name="TPriority">紧急程度</typeparam>
    public class SimplePriorityQueue<TItem, TPriority> : IPriorityQueue<TItem, TPriority> where TPriority : IComparable<TPriority>
    {
        /// <summary>
        /// 简单优先级队列节点
        /// </summary>
        private class SimpleNode : GenericPriorityQueueNode<TPriority>
        {
            public TItem Data { get; private set; }

            public SimpleNode(TItem data)
            {
                Data = data;
            }
        }

        private const int INITIAL_QUEUE_SIZE = 10;
        private readonly GenericPriorityQueue<SimpleNode, TPriority> _queue;
        private readonly Dictionary<TItem, IList<SimpleNode>> _itemToNodesCache;
        private readonly IList<SimpleNode> _nullNodesCache;

        #region 构造器
        /// <summary>
        /// Ctor
        /// </summary>
        public SimplePriorityQueue() : this(Comparer<TPriority>.Default, EqualityComparer<TItem>.Default) { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="priorityComparer">用于比较优先级的TPriority值, 默认为Comparer&lt;TPriority&gt;.default</param>
        public SimplePriorityQueue(IComparer<TPriority> priorityComparer) : this(priorityComparer.Compare, EqualityComparer<TItem>.Default) { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="priorityComparer">用于比较优先级的TPriority值</param>
        public SimplePriorityQueue(Comparison<TPriority> priorityComparer) : this(priorityComparer, EqualityComparer<TItem>.Default) { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="itemEquality">用于比较TItem值是否相等的方法</param>
        public SimplePriorityQueue(IEqualityComparer<TItem> itemEquality) : this(Comparer<TPriority>.Default, itemEquality) { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="priorityComparer">用于比较优先级的TPriority值, 默认为Comparer&lt;TPriority&gt;.default</param>
        /// <param name="itemEquality">用于比较TItem值是否相等的方法</param>
        public SimplePriorityQueue(IComparer<TPriority> priorityComparer, IEqualityComparer<TItem> itemEquality) : this(priorityComparer.Compare, itemEquality) { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="priorityComparer">用于比较优先级的TPriority值, 默认为Comparer&lt;TPriority&gt;.default</param>
        /// <param name="itemEquality">用于比较TItem值是否相等的方法</param>
        public SimplePriorityQueue(Comparison<TPriority> priorityComparer, IEqualityComparer<TItem> itemEquality)
        {
            _queue = new GenericPriorityQueue<SimpleNode, TPriority>(INITIAL_QUEUE_SIZE, priorityComparer);
            _itemToNodesCache = new Dictionary<TItem, IList<SimpleNode>>(itemEquality);
            _nullNodesCache = new List<SimpleNode>();
        }
        #endregion

        /// <summary>
        /// 根据所给的item获取队列中的节点
        /// </summary>
        /// <returns>匹配的SimpleNode节点</returns>
        private SimpleNode GetExistingNode(TItem item)
        {
            if (item == null)
            {
                return _nullNodesCache.Count > 0 ? _nullNodesCache[0] : null;
            }

            IList<SimpleNode> nodes;
            if (!_itemToNodesCache.TryGetValue(item, out nodes))
            {
                return null;
            }
            return nodes[0];
        }

        /// <summary>
        /// 将节点添加到节点缓存<br />
        /// O(1) 或 O(log n)
        /// </summary>
        /// <param name="node">需要添加到缓存的节点</param>
        private void AddToNodeCache(SimpleNode node)
        {
            if (node.Data == null)
            {
                _nullNodesCache.Add(node);
                return;
            }

            IList<SimpleNode> nodes;
            if (!_itemToNodesCache.TryGetValue(node.Data, out nodes))
            {
                nodes = new List<SimpleNode>();
                _itemToNodesCache[node.Data] = nodes;
            }
            nodes.Add(node);
        }

        /// <summary>
        /// 将节点从节点缓存中删除<br />
        /// O(1) 或 O(log n) 【假设没有重复】
        /// </summary>
        /// <param name="node">需要从缓存中删除的节点</param>
        private void RemoveFromNodeCache(SimpleNode node)
        {
            if (node.Data == null)
            {
                _nullNodesCache.Remove(node);
                return;
            }

            IList<SimpleNode> nodes;
            if (!_itemToNodesCache.TryGetValue(node.Data, out nodes))
            {
                return;
            }
            nodes.Remove(node);
            if (nodes.Count == 0)
            {
                _itemToNodesCache.Remove(node.Data);
            }
        }

        /// <summary>
        /// 返回队列里的元素数量<br />
        /// O(1)
        /// </summary>
        public int count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.count;
                }
            }
        }

        /// <summary>
        /// 队列的第一个元素<br />
        /// 如果队列为空，则报错<br />
        /// O(1)
        /// </summary>
        public TItem first
        {
            get
            {
                lock (_queue)
                {
                    if (_queue.count <= 0)
                    {
                        throw new InvalidOperationException("Cannot call .first on an empty queue");
                    }

                    return _queue.first.Data;
                }
            }
        }

        /// <summary>
        /// 清空队列里的所有元素<br />
        /// O(n)
        /// </summary>
        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
                _itemToNodesCache.Clear();
                _nullNodesCache.Clear();
            }
        }

        /// <summary>
        /// 判断当前元素是否在队列中<br />
        /// O(1)
        /// </summary>
        /// <param name="item">需要判断的元素</param>
        /// <returns>如果队列包含这个元素则返回true 否则为false</returns>
        public bool Contains(TItem item)
        {
            lock (_queue)
            {
                return item == null ? _nullNodesCache.Count > 0 : _itemToNodesCache.ContainsKey(item);
            }
        }

        /// <summary>
        /// 队列头出队<br />
        /// 如果队列是空的则抛出异常<br />
        /// O(log n)
        /// </summary>
        /// <returns>出队的元素</returns>
        public TItem Dequeue()
        {
            lock (_queue)
            {
                if (_queue.count <= 0)
                {
                    throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
                }

                SimpleNode node = _queue.Dequeue();
                RemoveFromNodeCache(node);
                return node.Data;
            }
        }

        /// <summary>
        /// 不用lock(_queue)和AddToNodeCache(node)将给定的元素和优先级值入队
        /// </summary>
        /// <param name="item">入队元素</param>
        /// <param name="priority">优先级值</param>
        /// <returns>入队的SimpleNode对象</returns>
        private SimpleNode EnqueueNoLockOrCache(TItem item, TPriority priority)
        {
            SimpleNode node = new SimpleNode(item);
            if (_queue.count == _queue.capcity)
            {
                _queue.Resize(_queue.capcity * 2 + 1);
            }
            _queue.Enqueue(node, priority);
            return node;
        }

        /// <summary>
        /// 将节点元素入队，优先级值小的将会排在队列前面，优先级相同的节点以先进先出排序<br />
        /// 队列大小会自动调整，所以不用考虑是否会满<br />
        /// 重复添加以及空值都是允许的<br />
        /// O(log n)
        /// </summary>
        public void Enqueue(TItem item, TPriority priority)
        {
            lock (_queue)
            {
                IList<SimpleNode> nodes;
                if (item == null)
                {
                    nodes = _nullNodesCache;
                }
                else if (!_itemToNodesCache.TryGetValue(item, out nodes))
                {
                    nodes = new List<SimpleNode>();
                    _itemToNodesCache[item] = nodes;
                }
                SimpleNode node = EnqueueNoLockOrCache(item, priority);
                nodes.Add(node);
            }
        }

        /// <summary>
        /// 不重复入队
        /// 将一个不在队列中的item及其优先级入队，优先级值小的会被排在前面，相同的优先级以先进先出排序<br />
        /// 队列是自动设置大小的，所以不用关心队列是否是满的<br />
        /// 空值是允许的<br />
        /// O(log n)
        /// </summary>
        /// <returns> 成功入队返回true，否则返回false；如果在队列中存在则返回false</returns>
        public bool EnqueueWithoutDuplicates(TItem item, TPriority priority)
        {
            lock (_queue)
            {
                IList<SimpleNode> nodes;
                if (item == null)
                {
                    if (_nullNodesCache.Count > 0)
                    {
                        return false;
                    }
                    nodes = _nullNodesCache;
                }
                else if (_itemToNodesCache.ContainsKey(item))
                {
                    return false;
                }
                else
                {
                    nodes = new List<SimpleNode>();
                    _itemToNodesCache[item] = nodes;
                }
                SimpleNode node = EnqueueNoLockOrCache(item, priority);
                nodes.Add(node);
                return true;
            }
        }

        /// <summary>
        /// 从队列中删除给定节点元素，这个节点不一定是队列的头<br />
        /// 如果节点并未在队列中，结果是未定义的，如果不确定，则先使用Contains()确认<br />
        /// 如果有多个副本加入到队列中，则删除匹配的第一个<br />
        /// O(log n)
        /// </summary>
        public void Remove(TItem item)
        {
            lock (_queue)
            {
                SimpleNode removeMe;
                IList<SimpleNode> nodes;
                if (item == null)
                {
                    if (_nullNodesCache.Count == 0)
                    {
                        throw new InvalidOperationException($"Cannot call Remove() on a node which is not enqueued: {item}");
                    }
                    removeMe = _nullNodesCache[0];
                    nodes = _nullNodesCache;
                }
                else
                {
                    if (!_itemToNodesCache.TryGetValue(item, out nodes))
                    {
                        throw new InvalidOperationException($"Cannot call Remove() on a node which is not enqueued: {item}");
                    }
                    removeMe = nodes[0];
                    if (nodes.Count == 1)
                    {
                        _itemToNodesCache.Remove(item);
                    }
                }
                _queue.Remove(removeMe);
                nodes.Remove(removeMe);
            }
        }

        /// <summary>
        /// 更改队列里某个节点的优先级<br />
        /// 对不在这个队列中的节点元素调用这个方法会抛出异常<br />
        /// 如果item多次入队，只有第一个item会被更新<br />
        /// (如果需要同一时间多次入队且可以同时将他们更新，请将你的items放入包装类中来区分它们)<br />
        /// O(log n)
        /// </summary>
        public void UpdatePriority(TItem item, TPriority priority)
        {
            lock (_queue)
            {
                SimpleNode updateMe = GetExistingNode(item);
                if (updateMe == null)
                {
                    throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + item);
                }
                _queue.UpdatePriority(updateMe, priority);
            }
        }

        /// <summary>
        /// 返回给定Item的TPriority值
        /// 对不在队列中的节点元素调用此方法会抛出异常
        /// 如果item多次入队，只有第一个的优先级值会被返回<br />
        /// (如果需要同一时间多次入队且可以同时将他们更新，请将你的items放入包装类中来区分它们)<br />
        /// O(1)
        /// </summary>
        /// <returns>对应的优先级值</returns>
        public TPriority GetPriority(TItem item)
        {
            lock (_queue)
            {
                SimpleNode findMe = GetExistingNode(item);
                if (findMe == null)
                {
                    throw new InvalidOperationException($"Cannot call GetPriority() on a node which is not enqueued: {item}");
                }
                return findMe.priority;
            }
        }

        #region Try* methods for multithreading
        /// Get the head of the queue, without removing it (use TryDequeue() for that).
        /// Useful for multi-threading, where the queue may become empty between calls to Contains() and First
        /// Returns true if successful, false otherwise
        /// O(1)
        public bool TryFirst(out TItem first)
        {
            if (_queue.count > 0)
            {
                lock (_queue)
                {
                    if (_queue.count > 0)
                    {
                        first = _queue.first.Data;
                        return true;
                    }
                }
            }

            first = default(TItem);
            return false;
        }

        /// <summary>
        /// Removes the head of the queue (node with minimum priority; ties are broken by order of insertion), and sets it to first.
        /// Useful for multi-threading, where the queue may become empty between calls to Contains() and Dequeue()
        /// Returns true if successful; false if queue was empty
        /// O(log n)
        /// </summary>
        public bool TryDequeue(out TItem first)
        {
            if (_queue.count > 0)
            {
                lock (_queue)
                {
                    if (_queue.count > 0)
                    {
                        SimpleNode node = _queue.Dequeue();
                        first = node.Data;
                        RemoveFromNodeCache(node);
                        return true;
                    }
                }
            }

            first = default(TItem);
            return false;
        }

        /// <summary>
        /// Attempts to remove an item from the queue.  The item does not need to be the head of the queue.  
        /// Useful for multi-threading, where the queue may become empty between calls to Contains() and Remove()
        /// Returns true if the item was successfully removed, false if it wasn't in the queue.
        /// If multiple copies of the item are enqueued, only the first one is removed. 
        /// O(log n)
        /// </summary>
        public bool TryRemove(TItem item)
        {
            lock (_queue)
            {
                SimpleNode removeMe;
                IList<SimpleNode> nodes;
                if (item == null)
                {
                    if (_nullNodesCache.Count == 0)
                    {
                        return false;
                    }
                    removeMe = _nullNodesCache[0];
                    nodes = _nullNodesCache;
                }
                else
                {
                    if (!_itemToNodesCache.TryGetValue(item, out nodes))
                    {
                        return false;
                    }
                    removeMe = nodes[0];
                    if (nodes.Count == 1)
                    {
                        _itemToNodesCache.Remove(item);
                    }
                }
                _queue.Remove(removeMe);
                nodes.Remove(removeMe);
                return true;
            }
        }

        /// <summary>
        /// Call this method to change the priority of an item.
        /// Useful for multi-threading, where the queue may become empty between calls to Contains() and UpdatePriority()
        /// If the item is enqueued multiple times, only the first one will be updated.
        /// (If your requirements are complex enough that you need to enqueue the same item multiple times <i>and</i> be able
        /// to update all of them, please wrap your items in a wrapper class so they can be distinguished).
        /// Returns true if the item priority was updated, false otherwise.
        /// O(log n)
        /// </summary>
        public bool TryUpdatePriority(TItem item, TPriority priority)
        {
            lock (_queue)
            {
                SimpleNode updateMe = GetExistingNode(item);
                if (updateMe == null)
                {
                    return false;
                }
                _queue.UpdatePriority(updateMe, priority);
                return true;
            }
        }

        /// <summary>
        /// Attempt to get the priority of the given item.
        /// Useful for multi-threading, where the queue may become empty between calls to Contains() and GetPriority()
        /// If the item is enqueued multiple times, only the priority of the first will be returned.
        /// (If your requirements are complex enough that you need to enqueue the same item multiple times <i>and</i> be able
        /// to query all their priorities, please wrap your items in a wrapper class so they can be distinguished).
        /// Returns true if the item was found in the queue, false otherwise
        /// O(1)
        /// </summary>
        public bool TryGetPriority(TItem item, out TPriority priority)
        {
            lock (_queue)
            {
                SimpleNode findMe = GetExistingNode(item);
                if (findMe == null)
                {
                    priority = default(TPriority);
                    return false;
                }
                priority = findMe.priority;
                return true;
            }
        }
        #endregion
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            List<TItem> queueData = new List<TItem>();
            lock (_queue)
            {
                //将队列里的所有item放到单独的列表，因为我们不希望在锁中yield return
                foreach (var node in _queue)
                {
                    queueData.Add(node.Data);
                }
            }

            return queueData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    /// <summary>
    /// A simplified priority queue implementation.  Is stable, auto-resizes, and thread-safe, at the cost of being slightly slower than
    /// FastPriorityQueue
    /// This class is kept here for backwards compatibility.  It's recommended you use SimplePriorityQueue&lt;TItem, TPriority&gt;
    /// </summary>
    /// <typeparam name="TItem">The type to enqueue</typeparam>
    public class SimplePriorityQueue<TItem> : SimplePriorityQueue<TItem, float>
    {
        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        public SimplePriorityQueue() { }

        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        /// <param name="comparer">The comparer used to compare priority values.  Defaults to Comparer&lt;float&gt;.default</param>
        public SimplePriorityQueue(IComparer<float> comparer) : base(comparer) { }

        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        /// <param name="comparer">The comparison function to use to compare priority values</param>
        public SimplePriorityQueue(Comparison<float> comparer) : base(comparer) { }
    }
}
