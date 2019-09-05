/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
namespace IFramework
{
    public class CycLinkedList<T> : IDisposable, ICollection<T>, IList<T>, IEnumerable, IEnumerable<T>, ICloneable
    {
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new Exception("Out Of Range");
                if (curIndex > index)
                {
                    cur = head;
                    curIndex = 0;
                }
                while (curIndex !=index)
                {
                    cur = cur.next;
                    curIndex = ++curIndex % count;
                }
                return cur.data;
            }
            set
            {
                if (index < 0 || index >= count) throw new Exception("Out Of Range");
                if (curIndex > index)
                {
                    cur = head;
                    curIndex = 0;
                }
                while (curIndex != index)
                {
                    cur = cur.next;
                    curIndex = ++curIndex % count;
                }
                cur.data=value;
            }
        }
        private SingleLinkedNode<T> head;
        private SingleLinkedNode<T> trail;
        private SingleLinkedNode<T> cur;
        private int curIndex;

        private int count;
        public bool IsReadOnly { get { return false; } }
        public int Count { get { return count; } }

        public void Add(T t)
        {
            SingleLinkedNode<T> node = new SingleLinkedNode<T>(t);
            if (count == 0)
            {
                head = node;
                head.next = head;
                cur = trail = head;
                curIndex = 0;
            }
            else
            {
                trail.next = node;
                node.next = head;
                trail = node;
            }
            count++;
        }

        public bool Contains(T t)
        {
            return IndexOf(t)==-1;
        }
        public int IndexOf(T t)
        {
            int tempCount = count-1;
            while (!cur.data.Equals(t))
            {
                cur = cur.next;
                curIndex = ++curIndex % count;
                if (--tempCount <0)
                {
                    return -1;
                }
            }
            return curIndex;
        }
        public void Clear()
        {
            cur= head = trail = null;
            curIndex = 0;
            count = 0;
        }
        public object Clone()
        {
            return this.ReflectionDeepCopy();
        }
        public void Insert(int index, T t)
        {
            if (index < 0) return;
            if (index >= count) Add(t);
            else
            {
                SingleLinkedNode<T> node = new SingleLinkedNode<T>(t);
                if (curIndex >= index)
                {
                    cur = head;
                    curIndex = 0;
                }
                while (--index > 0)
                {
                    cur = cur.next;
                    curIndex = ++curIndex % count;
                }
                if (index == -1)
                {
                    trail.next = node;
                    node.next = head;
                    head = node;

                    cur = head;
                    curIndex = 0;
                }
                else
                {
                    node.next = cur.next;
                    cur.next = node;
                }
                count++;
            }
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (cur = head,curIndex=0; arrayIndex < array.Length && curIndex <count; arrayIndex++, curIndex++, cur = cur.next)
                array[arrayIndex] = cur.data;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public bool Remove(T t)
        {
            if (count == 1 && head.data.Equals(t))
            {
                Clear();return true;
            }
            else
            {
                int tempCount = count - 1;
                while (!cur.next.data.Equals(t))
                {
                    cur = cur.next;
                    curIndex = ++curIndex % count;
                    if (--tempCount < 0)
                    {
                        return false;
                    }
                }
                if (cur.next==head)
                {
                    head = cur.next.next;
                }
                if (cur.next == trail)
                {
                    trail = cur;
                }
                cur.next = cur.next.next;
                count--;
                return true;
            }
           
        }
        public void RemoveAt(int index)
        {
            if (index >= count || index < 0) return;
            if (index == 0)
            {
                trail.next = trail.next.next;
                head = trail.next;
                cur = head;
                curIndex = 0;
                count--;
            }
            else if (curIndex >= index)
            {
                cur = head;
                curIndex = 0;
                while (curIndex != index-1)
                {
                    cur = cur.next;
                    curIndex = ++curIndex % count;
                }
                cur.next = cur.next.next;
                count--;
            }
            else
            {
                while (curIndex != index-1)
                {
                    cur = cur.next;
                    curIndex = ++curIndex % count;
                }
                cur.next = cur.next.next;
                count--;
            }
            
        }
        public T[] ToArray()
        {
            if (head == null) return default(T[]);
            T[] result = new T[count];
            
            for (int i = 0; i<count ; cur = cur.next,i++,curIndex=++curIndex%count)
                result[curIndex] = cur.data;
            return result;
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
