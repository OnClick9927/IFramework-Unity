/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework
{
	public class DoubleLinkedList<T>: IDisposable, ICollection<T>, IList<T>, IEnumerable, IEnumerable<T>, ICloneable
	{
        private DoubleLinkedNode<T> head;
        private DoubleLinkedNode<T> trail;
        private int count;
        public bool IsReadOnly { get { return false; } }
        public int Count { get { return count; } }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new Exception("Out of Range");
                if (count > (index << 1))
                {
                    DoubleLinkedNode<T> tempHead = head;
                    while (index-- > 0)
                    {
                        tempHead = tempHead.next;
                    }
                    return tempHead.data;
                }
                else
                {
                    DoubleLinkedNode<T> tempTail = trail;
                    while (++index < count)
                    {
                        tempTail = tempTail.pre;
                    }
                    return tempTail.data;
                }
            }
            set
            {
                if (index < 0 || index >= count) throw new Exception("Out of Range");
                if (count > (index << 1))
                {
                    DoubleLinkedNode<T> tempHead = head;
                    while (index-- > 0)
                    {
                        tempHead = tempHead.next;
                    }
                    tempHead.data = value;
                }
                else
                {
                    DoubleLinkedNode<T> tempTail = trail;
                    while (++index < count)
                    {
                        tempTail = tempTail.pre;
                    }
                    tempTail.data = value;
                }
            }

        }

        public void Add(T t)
        {
            DoubleLinkedNode<T> node = new DoubleLinkedNode<T>(t);
            if (count == 0) head = trail = node;
            else
            {
                trail.next = node;
                node.pre = trail;
                trail = node;
            }
            count++;
        }
        public void Clear()
        {
            count = 0;
            head = trail = null;
        }
        public object Clone()
        {
            return this.ReflectionDeepCopy();
        }
        public bool Contains(T t)
        {
            return IndexOf(t) != -1;
        }
        public int IndexOf(T t)
        {
            if (count == 0) return -1;
            if (count == 1) return head.data.Equals(t)? 0 : -1;
            if (count == 2) return head.data.Equals(t)? 0 : trail.data.Equals(t) ? 1 : -1;
            DoubleLinkedNode<T> tempHead = head;
            DoubleLinkedNode<T> tempTail = trail;
            int headCount = 0;
            while (!tempHead.data.Equals(t) && !tempTail.data.Equals(t) && tempTail != tempHead && tempHead.next != tempTail)
            {
                tempHead = tempHead.next;
                tempTail = tempTail.pre;
                headCount++;
            }
            return tempHead.data.Equals(t) ? headCount : tempTail.data.Equals(t) ? count-headCount-1 : -1;
        }
        public void Dispose()
        {
            count = 0;
            head = trail = null;
        }
        public bool Remove(T t)
        {
            if (count == 0) return false ;
            if (count==1)
            {
                if (head.data.Equals(t))
                {
                    head = trail = null;
                    count--;
                    return true;
                }
                return false;
            }
            DoubleLinkedNode<T> tempHead = head;
            DoubleLinkedNode<T> tempTail = trail;
            while (!tempHead.data.Equals(t) && !tempTail.data.Equals(t) && tempTail != tempHead && tempHead.next != tempTail)
            {
                tempHead = tempHead.next;
                tempTail = tempTail.pre;
            }
            if (tempHead.data.Equals(t))
            {
                if (tempHead == head)
                {
                    head = tempHead.next;
                    tempHead.next.pre = null;
                }
                else
                {
                    tempHead.pre.next = tempHead.next;
                    tempHead.next.pre = tempHead.pre;
                }
                count--;
                return true;
            }
            if (tempTail.data.Equals(t))
            {
                if (tempTail == trail)
                {
                    trail = tempTail.pre;
                    tempTail.pre.next = null;
                }
                else
                {
                    tempTail.pre.next = tempTail.next;
                    tempTail.next.pre = tempTail.pre;
                }
                count--;
                return true;
            }
            return false;
        }
        public void RemoveAt(int index)
        {
            if (index >= count || index< 0 ) return;
            if (count==1)
            {
                head = trail = null;
                count--;
                return;
            }
            if (count >(index<<1))
            {
                DoubleLinkedNode<T> tempHead = head;
                while (index-->0)
                {
                    tempHead = tempHead.next;
                }
                if (tempHead == head)
                {
                    head = tempHead.next;
                    tempHead.next.pre = null;
                }
                else
                {
                    tempHead.pre.next = tempHead.next;
                    tempHead.next.pre = tempHead.pre;
                }
                count--;
            }
            else
            {
                DoubleLinkedNode<T> tempTail = trail;
                while (++index < count)
                {
                    tempTail = tempTail.pre;
                }
                if (tempTail == trail)
                {
                    trail = tempTail.pre;
                    tempTail.pre.next = null;
                }
                else
                {
                    tempTail.pre.next = tempTail.next;
                    tempTail.next.pre = tempTail.pre;
                }
                count--;
            }
        }
        public void Insert(int index, T t)
        {
            if (index < 0) return;
            else if (count == 0 || index >= count)
            {
                Add(t);
            } 
            else
            {
                DoubleLinkedNode<T> node = new DoubleLinkedNode<T>(t);
                if (count > (index << 1))
                {
                    DoubleLinkedNode<T> tempHead = head;
                    while (--index >= 0)
                    {
                        tempHead = tempHead.next;
                    }
                    if (tempHead == head)
                    {
                        node.next = head;
                        head.pre = node;
                        head = node;
                    }
                    else
                    {
                        node.pre = tempHead.pre;
                        tempHead.pre.next = node;
                        node.next = tempHead;
                        tempHead.pre = node;
                    }
                    count++;
                }
                else
                {
                    DoubleLinkedNode<T> temptail = trail;
                    while (++index < count)
                    {
                        temptail = temptail.pre;
                    }
                    node.pre = temptail.pre;
                    node.next = temptail;
                    temptail.pre.next = node;
                    temptail.pre = node;
                    count++;
                }
            }
        }

        public void Reverse()
        {
            if (head == null || trail == null)
                return;
            DoubleLinkedNode<T> prev;
            DoubleLinkedNode<T> next;
            DoubleLinkedNode<T> newFirst=null;
            DoubleLinkedNode<T> newLast=null;
            DoubleLinkedNode<T> node = head;
            while (node != null)
            {
                prev = node.pre;
                next = node.next;
                if (node.pre == null) newLast = node;
                else if (node.next == null) newFirst = node;
                node.next = prev;
                node.pre = next;
                node = node.pre;

            }
            head = newFirst;
            trail = newLast;
        }
        public T[] ToArray()
        {
            if (head == null) return default(T[]);
            T[] result = new T[count];
            int i = 0;
            for (DoubleLinkedNode<T> node = head; node != null; node = node.next)
                result[i++] = node.data;
            return result;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            for (DoubleLinkedNode<T> node = head; node != null && i<array.Length; node = node.next)
                array[i++] = node.data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(ToArray());

        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(ToArray());
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private T[] arr;
            private int index;
            public object Current { get { return arr[index]; } }
            T IEnumerator<T>.Current { get { return arr[index]; } }
            public Enumerator(T[] arr)
            {
                this.arr = arr;
                index = -1;
            }
            public void Dispose()
            {
                index = -1;
                Array.Clear(arr, 0, arr.Length);
            }

            public bool MoveNext()
            {
                if (++index < arr.Length) return true;
                return false;
            }
            public void Reset()
            {
                index = -1;
            }
        }
    }
}
