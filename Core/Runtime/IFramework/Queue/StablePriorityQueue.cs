using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Queue
{
    /// <summary>
    /// 稳定优先级队列
    /// </summary>
    /// <typeparam name="T">节点元素类型</typeparam>
    public sealed class StablePriorityQueue<T> : IFixedSizePriorityQueue<T, float> where T : StablePriorityQueueNode
    {
        private int _numNodes;
        private T[] _nodes;
        private long _numNodesEverEnqueued;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="capicty">允许入队的最大数量(如果超过将导致未定义的操作)</param>
        public StablePriorityQueue(int capicty = 4)
        {
#if DEBUG
            if (capicty <= 0)
            {
                throw new InvalidOperationException("New queue size cannot be smaller than 1");
            }
#endif

            _numNodes = 0;
            _nodes = new T[capicty + 1];
            _numNodesEverEnqueued = 0;
        }

        /// <summary>
        /// 返回队列里的元素数量<br />
        /// O(1)
        /// </summary>
        public int count
        {
            get
            {
                return _numNodes;
            }
        }

        /// <summary>
        /// 队列中可以入队的最大数量<br />
        /// 如果入队数量超过队列的大小(Count == MaxSize时Enqueue())，会导致未定义的操作<br />
        /// O(1)
        /// </summary>
        public int capcity
        {
            get
            {
                return _nodes.Length - 1;
            }
        }

        /// <summary>
        /// 移除队列里的所有元素<br />
        /// O(n) (因此请勿频繁使用此方法)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            Array.Clear(_nodes, 1, _numNodes);
            _numNodes = 0;
        }

        /// <summary>
        /// 判断当前元素是否在队列中<br />
        /// 如果元素被添加到其他的队列，则结果是不确定的，
        /// 除非调用了之前队列的ResetNode(node)方法<br />
        /// O(1)
        /// </summary>
        /// <param name="node">需要判断的元素</param>
        /// <returns>如果队列包含这个元素则返回true 否则为false</returns>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(T node)
        {
#if DEBUG
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.position < 0 || node.position >= _nodes.Length)
            {
                throw new InvalidOperationException("node.position has been corrupted. Did you change it manually?");
            }
#endif

            return (_nodes[node.position] == node);
        }

        /// <summary>
        /// 将优先级节点入队，优先级值小的将会排在队列前面，优先级相同的节点以先进先出排序<br />
        /// 如果队列是满的，则执行未定义的操作<br />
        /// 如果节点元素已经入队了，则执行未定义的操作<br />
        /// 如果节点已经在队列中了，除非在此之前调用了之前队列的ResetNode(node)方法，否则结果是未定义的<br />
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Enqueue(T node, float priority)
        {
#if DEBUG
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (_numNodes >= _nodes.Length - 1)
            {
                throw new InvalidOperationException($"Queue is full - node cannot be added: {node}");
            }
            if (Contains(node))
            {
                throw new InvalidOperationException($"Node is already enqueued: {node}");
            }
#endif

            node.priority = priority;
            _numNodes++;
            _nodes[_numNodes] = node;
            node.position = _numNodes;
            node.insertPosition = _numNodesEverEnqueued++;
            CascadeUp(node);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeUp(T node)
        {
            int parent;
            if (node.position > 1)
            {
                parent = node.position >> 1;
                T parentNode = _nodes[parent];
                if (HasHigherPriority(parentNode, node))
                    return;

                _nodes[node.position] = parentNode;
                parentNode.position = node.position;

                node.position = parent;
            }
            else
            {
                return;
            }
            while (parent > 1)
            {
                parent >>= 1;
                T parentNode = _nodes[parent];
                if (HasHigherPriority(parentNode, node))
                    break;

                _nodes[node.position] = parentNode;
                parentNode.position = node.position;

                node.position = parent;
            }
            _nodes[node.position] = node;
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeDown(T node)
        {
            int finalQueueIndex = node.position;
            int childLeftIndex = 2 * finalQueueIndex;

            if (childLeftIndex > _numNodes)
            {
                return;
            }

            int childRightIndex = childLeftIndex + 1;
            T childLeft = _nodes[childLeftIndex];
            if (HasHigherPriority(childLeft, node))
            {
                if (childRightIndex > _numNodes)
                {
                    node.position = childLeftIndex;
                    childLeft.position = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    _nodes[childLeftIndex] = node;
                    return;
                }
                T childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childLeft, childRight))
                {
                    childLeft.position = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    childRight.position = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            else if (childRightIndex > _numNodes)
            {
                return;
            }
            else
            {
                T childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childRight, node))
                {
                    childRight.position = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                else
                {
                    return;
                }
            }

            while (true)
            {
                childLeftIndex = 2 * finalQueueIndex;

                if (childLeftIndex > _numNodes)
                {
                    node.position = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }

                childRightIndex = childLeftIndex + 1;
                childLeft = _nodes[childLeftIndex];
                if (HasHigherPriority(childLeft, node))
                {
                    if (childRightIndex > _numNodes)
                    {
                        node.position = childLeftIndex;
                        childLeft.position = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        _nodes[childLeftIndex] = node;
                        break;
                    }
                    T childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childLeft, childRight))
                    {
                        childLeft.position = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        childRight.position = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                else if (childRightIndex > _numNodes)
                {
                    node.position = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }
                else
                {
                    T childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childRight, node))
                    {
                        childRight.position = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    else
                    {
                        node.position = finalQueueIndex;
                        _nodes[finalQueueIndex] = node;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 对两个参数的优先级进行比较
        /// </summary>
        /// <param name="higher">第一个参数</param>
        /// <param name="lower">第二个参数</param>
        /// <returns>
        /// 如果第一个参数的优先级大于第二个参数则返回true，否则返回false<br />
        /// 如果两个参数为同一个节点，返回false
        /// </returns>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool HasHigherPriority(T higher, T lower)
        {
            return (higher.priority < lower.priority ||
                (higher.priority == lower.priority && higher.insertPosition < lower.insertPosition));
        }

        /// <summary>
        /// 队列头部出队
        /// 如果队列为空则执行未定义的操作<br />
        /// O(log n)
        /// </summary>
        /// <returns>出队的元素</returns>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public T Dequeue()
        {
#if DEBUG
            if (_numNodes <= 0)
            {
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
            }
#endif

            T returnMe = _nodes[1];
            //如果节点元素已经是最后一个节点，则直接移除
            if (_numNodes == 1)
            {
                _nodes[1] = null;
                _numNodes = 0;
                return returnMe;
            }

            //将节点与最后一个节点对换
            T formerLastNode = _nodes[_numNodes];
            _nodes[1] = formerLastNode;
            formerLastNode.position = 1;
            _nodes[_numNodes] = null;
            _numNodes--;

            //将formerLastNode (不再是最后一个节点)向下冒泡
            CascadeDown(formerLastNode);
            return returnMe;
        }

        /// <summary>
        /// 重新设置队列的大小，使其可以容纳更多的节点，当前的所有节点都会保留<br />
        /// 如果试图将队列大小设置成比当前队列中的数量小时，会执行未定义的操作<br />
        /// O(n)
        /// </summary>
        public void Resize(int maxNodes)
        {
#if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("Queue size cannot be smaller than 1");
            }

            if (maxNodes < _numNodes)
            {
                throw new InvalidOperationException($"Called Resize({ maxNodes }), but current queue contains { _numNodes } nodes");
            }
#endif

            T[] newArray = new T[maxNodes + 1];
            int highestIndexToCopy = Math.Min(maxNodes, _numNodes);
            Array.Copy(_nodes, newArray, highestIndexToCopy + 1);
            _nodes = newArray;
        }

        /// <summary>
        /// 队列的第一个元素<br />
        /// 如果队列为空，则执行未定义的操作<br />
        /// O(1)
        /// </summary>
        public T first
        {
            get
            {
#if DEBUG
                if (_numNodes <= 0)
                {
                    throw new InvalidOperationException("Cannot call .first on an empty queue");
                }
#endif

                return _nodes[1];
            }
        }

        /// <summary>
        /// 更改队列里某个节点的优先级<br />
        /// <b>忘记调用这个方法会导致队列损坏!</b><br />
        /// 对不在这个队列中的节点元素调用这个方法会执行未定义的行为<br />
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void UpdatePriority(T node, float priority)
        {
#if DEBUG
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (!Contains(node))
            {
                throw new InvalidOperationException($"Cannot call UpdatePriority() on a node which is not enqueued: {node}");
            }
#endif

            node.priority = priority;
            OnNodeUpdated(node);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void OnNodeUpdated(T node)
        {
            //按需向上或向下冒泡更新节点
            int parentIndex = node.position >> 1;

            if (parentIndex > 0 && HasHigherPriority(node, _nodes[parentIndex]))
            {
                CascadeUp(node);
            }
            else
            {
                //如果 parentNode == node(node是根节点) 则会调用CascadeDown
                CascadeDown(node);
            }
        }

        /// <summary>
        /// 从队列中删除给定节点元素，这个节点不一定是队列的头<br />
        /// 如果节点并未在队列中，结果是未定义的，如果不确定，则先使用Contains()确认<br />
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Remove(T node)
        {
#if DEBUG
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (!Contains(node))
            {
                throw new InvalidOperationException($"Cannot call Remove() on a node which is not enqueued: {node}");
            }
#endif

            //如果节点元素已经是最后一个节点，则直接移除
            if (node.position == _numNodes)
            {
                _nodes[_numNodes] = null;
                _numNodes--;
                return;
            }

            //将节点与最后一个节点对换
            T formerLastNode = _nodes[_numNodes];
            _nodes[node.position] = formerLastNode;
            formerLastNode.position = node.position;
            _nodes[_numNodes] = null;
            _numNodes--;

            //将formerLastNode (不再是最后一个节点)按需向上或者向下冒泡排序
            OnNodeUpdated(formerLastNode);
        }

        /// <summary>
        /// 默认情况下有加入过一个队列的节点元素不能添加到另一个队列<br />
        /// 如果需要这么做，则在添加到另一个队列前调用当前队列的此方法<br />
        /// 如果节点当前在节点中或者属于其他队列，则结果是未定义的
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void ResetNode(T node)
        {
#if DEBUG
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (Contains(node))
            {
                throw new InvalidOperationException("node.ResetNode was called on a node that is still in the queue");
            }
#endif

            node.position = 0;
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<T> GetEnumerator()
        {
#if NET_VERSION_4_5 // ArraySegment在4.5之前没有 实现IEnumerable接口
            IEnumerable<T> e = new ArraySegment<T>(_nodes, 1, _numNodes);
            return e.GetEnumerator();
#else
            for (int i = 1; i <= _numNodes; i++)
                yield return _nodes[i];
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
