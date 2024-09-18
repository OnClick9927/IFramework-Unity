using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework
{
    sealed class GenericPriorityQueue<TItem, TPriority> : IEnumerable
        where TItem : class, IGenericPriorityQueueNode<TPriority> where TPriority : IComparable<TPriority>
    {
        private int _numNodes;
        private TItem[] _nodes;
        private long _numNodesEverEnqueued;
        private readonly Comparison<TPriority> _comparer;

        public GenericPriorityQueue(int maxNodes) : this(maxNodes, Comparer<TPriority>.Default) { }

        public GenericPriorityQueue(int maxNodes, IComparer<TPriority> comparer) : this(maxNodes, comparer.Compare) { }

        public GenericPriorityQueue(int maxNodes, Comparison<TPriority> comparer)
        {
#if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("New queue size cannot be smaller than 1");
            }
#endif

            _numNodes = 0;
            _nodes = new TItem[maxNodes + 1];
            _numNodesEverEnqueued = 0;
            _comparer = comparer;
        }

        public int count
        {
            get
            {
                return _numNodes;
            }
        }


        public int capacity
        {
           
            get
            {
                return _nodes.Length - 1;
            }
        }


#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            Array.Clear(_nodes, 1, _numNodes);
            _numNodes = 0;
        }


#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(TItem node)
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


#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Enqueue(TItem node, TPriority priority)
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
        private void CascadeUp(TItem node)
        {
            int parent;
            if (node.position > 1)
            {
                parent = node.position >> 1;
                TItem parentNode = _nodes[parent];
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
                TItem parentNode = _nodes[parent];
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
        private void CascadeDown(TItem node)
        {
            int finalQueueIndex = node.position;
            int childLeftIndex = 2 * finalQueueIndex;

            if (childLeftIndex > _numNodes)
            {
                return;
            }

            int childRightIndex = childLeftIndex + 1;
            TItem childLeft = _nodes[childLeftIndex];
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
                TItem childRight = _nodes[childRightIndex];
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
                TItem childRight = _nodes[childRightIndex];
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
                    TItem childRight = _nodes[childRightIndex];
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
                    TItem childRight = _nodes[childRightIndex];
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


#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool HasHigherPriority(TItem higher, TItem lower)
        {
            var cmp = _comparer(higher.priority, lower.priority);
            return (cmp < 0 || (cmp == 0 && higher.insertPosition < lower.insertPosition));
        }


#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public TItem Dequeue()
        {
#if DEBUG
            if (_numNodes <= 0)
            {
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
            }
#endif

            TItem returnMe = _nodes[1];
            if (_numNodes == 1)
            {
                _nodes[1] = null;
                _numNodes = 0;
                return returnMe;
            }

            TItem formerLastNode = _nodes[_numNodes];
            _nodes[1] = formerLastNode;
            formerLastNode.position = 1;
            _nodes[_numNodes] = null;
            _numNodes--;

            CascadeDown(formerLastNode);
            return returnMe;
        }

        public void Resize(int maxNodes)
        {
#if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("Queue size cannot be smaller than 1");
            }

            if (maxNodes < _numNodes)
            {
                throw new InvalidOperationException($"Called Resize({maxNodes}), but current queue contains {_numNodes} nodes");
            }
#endif

            TItem[] newArray = new TItem[maxNodes + 1];
            int highestIndexToCopy = Math.Min(maxNodes, _numNodes);
            Array.Copy(_nodes, newArray, highestIndexToCopy + 1);
            _nodes = newArray;
        }

        public TItem first
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


#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void UpdatePriority(TItem node, TPriority priority)
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
        private void OnNodeUpdated(TItem node)
        {
            int parentIndex = node.position >> 1;

            if (parentIndex > 0 && HasHigherPriority(node, _nodes[parentIndex]))
            {
                CascadeUp(node);
            }
            else
            {
                CascadeDown(node);
            }
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Remove(TItem node)
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

            if (node.position == _numNodes)
            {
                _nodes[_numNodes] = null;
                _numNodes--;
                return;
            }

            TItem formerLastNode = _nodes[_numNodes];
            _nodes[node.position] = formerLastNode;
            formerLastNode.position = node.position;
            _nodes[_numNodes] = null;
            _numNodes--;

            OnNodeUpdated(formerLastNode);
        }


#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void ResetNode(TItem node)
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


        public IEnumerator<TItem> GetEnumerator()
        {
#if NET_VERSION_4_5 // ArraySegment在4.5之前没有 实现IEnumerable接口
            IEnumerable<TItem> e = new ArraySegment<TItem>(_nodes, 1, _numNodes);
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
