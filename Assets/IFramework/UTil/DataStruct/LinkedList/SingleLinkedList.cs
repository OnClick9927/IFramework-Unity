/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;

namespace IFramework
{
    public class SingleLinkedList<T>: IDisposable, ICollection<T>, IList<T>, IEnumerable, IEnumerable<T>, ICloneable
    {
        private SingleLinkedNode<T> head;
        public int count;

        public int Count { get { return count; } }
        public bool IsReadOnly { get { return false; ; } }

        public SingleLinkedList() { }
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new Exception("Out Of Range");
                SingleLinkedNode<T> tempNode = head;
                while (index-- > 0)
                {
                    tempNode = tempNode.next;
                }
                return tempNode.data;
            }
            set
            {
                if (index < 0 || index >= count) throw new Exception("Out Of Range");
                SingleLinkedNode<T> tempNode = head;
                while (index-- > 0)
                {
                    tempNode = tempNode.next;
                }
                tempNode.data = value;
            }
        }
        public int IndexOf(T t)
        {
            if (head == null) return -1;
            SingleLinkedNode<T> tempNode = head;
            int tempCount = 0;
            while (tempNode.next != null)
            {
                if (t.Equals(tempNode.data))
                {
                    return tempCount;
                }
                else
                {
                    tempNode = tempNode.next;
                    tempCount++;
                }
            }
            if (tempNode.data.Equals(t)) return tempCount;
            return -1;
        }
        public bool Contains(T t)
        {
            return IndexOf(t) == -1;
        }

        public T AddHead(T t)
        {
            SingleLinkedNode<T> newHead = new SingleLinkedNode<T>(t);
            if (++count == 1) head = newHead;
            else
            {
                newHead.next = head;
                head = newHead;
            }
            return t;
        }
        public void Add(T t)
        {
            SingleLinkedNode<T> node = new SingleLinkedNode<T>(t);
            if (++count == 1) head = node;
            else
            {
                SingleLinkedNode<T> tempNode = head;
                while (tempNode.next != null)
                {
                    tempNode = tempNode.next;
                }
                tempNode.next = node;
            }
        }

        public void AddRange(IEnumerable<T> ts)
        {
            if (head == null) AddHead(ts.GetEnumerator().Current);
            SingleLinkedNode<T> tempNode = head;
            while (tempNode.next != null)
            {
                tempNode = tempNode.next;
            }
            while (ts.GetEnumerator().MoveNext())
            {
                SingleLinkedNode<T> node = new SingleLinkedNode<T>(ts.GetEnumerator().Current);
                tempNode.next = node;
                tempNode = tempNode.next;
                count++;
            }
        }
        public void Insert(int index,T t)
        {
            if (index < 0) return;
            if (head == null || index == 0) AddHead(t);
            else if (index >= count) Add(t);
            else
            {
                SingleLinkedNode<T> node = new SingleLinkedNode<T>(t);
                SingleLinkedNode<T> preTempNode = head;
                while (--index > 0)
                {
                    preTempNode = preTempNode.next;
                }
                node.next = preTempNode.next;
                preTempNode.next = node;
                count++;
            }
        }

        public bool RemoveHead()
        {
            if (head == null) return true;
            head = head.next;
            count--;
            return true;
        }
        public bool RemoveTail()
        {
            if (head == null) return true;
            SingleLinkedNode<T> tempNode = head;
            if (tempNode.next == null)
            {
                head = null;
                count--;
                return true;
            }
            while (tempNode.next.next != null)
            {
                tempNode = tempNode.next;
            }
            count--;
            tempNode.next = null;
            return true;
        }

        public bool Remove(T t)
        {
            if (count == 0) return false;
            if (head.data.Equals(t)) return RemoveHead();
            SingleLinkedNode<T> tempNode = head;
            while (tempNode.next != null && !tempNode.next.data.Equals(t))
            {
                tempNode = tempNode.next;
            }
            if (tempNode.next == null) return false;
            else
            {
                tempNode.next = tempNode.next.next;
                count--;
                return true;
            }
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) return;
            if (index == 0) RemoveHead();
            else
            {
                SingleLinkedNode<T> tempNode = head;
                while (--index> 0)
                {
                    tempNode = tempNode.next;
                }
                tempNode.next = tempNode.next.next;
                count--;
            }
        }

        public void Reverse()
        {
            if (count == 0 || count == 1) return;
            SingleLinkedNode<T> emptyNode = new SingleLinkedNode<T>(default(T));
            emptyNode.next = head;
            SingleLinkedNode<T> preTmpNode = head;
            SingleLinkedNode<T> tempNode = head.next;
            while (tempNode != null)
            {
                preTmpNode.next = tempNode.next;
                tempNode.next = emptyNode.next;
                emptyNode.next = tempNode;
                tempNode = preTmpNode.next;
            }
            head = emptyNode.next;
        }

        public void Clear()
        {
            count = 0;
            head = null;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            for (SingleLinkedNode<T> node = head; node != null && i<array.Length; node = node.next)
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
        public T[] ToArray()
        {
            if (head == null) return default(T[]);
            T[] result = new T[count];
            int i = 0;
            for (SingleLinkedNode<T> node = head; node != null; node = node.next)
                result[i++] = node.data;
            return result;
        }
        public object Clone()
        {
            return this.ReflectionDeepCopy();
        }
        public void Dispose()
        {
            Clear();
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
