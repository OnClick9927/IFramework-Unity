/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework
{
    public class CycArray<T> : IDisposable,ICollection<T>,IList<T>, IEnumerable, IEnumerable<T>, ICloneable
    {
        private int capacity;
        public int Capacity { get { return capacity; } }
        public bool IsEmpty { get { return tail == head; } }
        public int Count { get { return (tail - head + capacity) % capacity; } }
        private bool isFull { get { return (tail + 1) % capacity == head; } }


        private T[] array;
        private int head;
        private int tail;
        private int autoAllocLength;
        public CycArray() : this(4, 4) { }
        public CycArray(int capacity, int autoAllocLength)
        {
            array = new T[capacity];
            this.capacity = capacity;
            head = tail = 0;
            this.autoAllocLength = autoAllocLength;
        }
        public void Dispose()
        {
            this.capacity = head = tail = 0;
            Array.Clear(array, 0, array.Length);
        }

        public T this[int index]
        {
            get
            {
                if (index > Count)
                {
                    return default(T);
                }
                else
                {
                    return array[(head + index) % capacity];
                }
            }
            set
            {
                if (index > Count)
                {
                    throw new Exception("Out of Range");
                }
                else
                {
                    array[(head + index) % capacity] = value;
                }
            }
        }

        public bool Remove(T t)
        {
            int tempLen = Count;
            while (tempLen-- > 0)
            {
                int tempIndex = (tempLen + head) % capacity;
                if (array[tempIndex].Equals(t))
                {
                    int tempIndex2 = 0;
                    while (tempLen++ != Count)
                    {
                        tempIndex = (tempLen - head - 1) % capacity;
                        tempIndex2 = (tempLen + head) % capacity;
                        array[tempIndex] = array[tempIndex2];
                    }
                    tail = (tail - 1) % capacity;
                    return true;
                }
            }
            return false;
        }
        public void RemoveAt(int index)
        {
            if (index >= Count) return;
            while (index++ != Count)
            {
                int realIndex = (head + index - 1) % capacity;
                int realIndex2 = (head + index) % capacity;
                array[realIndex] = array[realIndex2];
            }
            tail = (tail - 1) % capacity;
        }

        public T DeQueue()
        {
            if (IsEmpty) return default(T);
            T v = array[head];
            head = (head + 1) % capacity;
            return v;
        }
        public T[] DeQueue(int size)
        {
            T[] ts = new T[size];
            while (size-- > 0)
            {
                ts[size] = DeQueue();
            }
            System.Array.Reverse(ts);
            return ts;
        }
        public T StackPop()
        {
            if (IsEmpty) return default(T);
            T v = array[(tail - 1 + capacity) % capacity];
            tail = (tail - 1 + capacity) % capacity;
            return v;
        }
        public T[] StackPop(int size)
        {
            T[] ts = new T[size];
            while (size-- > 0)
            {
                ts[size] = StackPop();
            }
            Array.Reverse(ts);
            return ts;
        }

        public T StackPeek()
        {
            if (IsEmpty) return default(T);
            return array[(tail - 1 + capacity) % capacity];
        }
        public T QueuePeek()
        {
            if (IsEmpty) return default(T);
            return array[head];
        }

        public void Add(T t)
        {
            if (isFull)
            {
                T[] temp = new T[capacity + autoAllocLength];
                Array.Copy(array, temp, capacity);
                array = temp;
                capacity += autoAllocLength;
            }
            array[tail] = t;
            tail = (tail + 1) % capacity;
        }
        public void AddRange(IEnumerable<T>  ts)
        {
            while (ts.GetEnumerator().MoveNext())
            {
                Add(ts.GetEnumerator().Current);
            }
        }

        public void Insert(int pos, T val)
        {
            if (pos >= Count) Add(val);
            else
            {
                int moveLen = Count - pos;
                int curMoveIndex = 0;
                while (moveLen-- > 0)
                {
                    int realIndex = (tail - curMoveIndex + capacity) % capacity;
                    int RealIndex2 = (tail - curMoveIndex - 1 + capacity) % capacity;
                    array[realIndex] = array[RealIndex2];
                    curMoveIndex++;
                }
                if (isFull)
                {
                    T[] temp = new T[capacity + autoAllocLength];
                    System.Array.Copy(array, temp, capacity);
                    array = temp;
                    capacity += autoAllocLength;
                }
                array[(head + pos) % capacity] = val;
                tail = (tail + 1) % capacity;
            }
        }

        public bool Contains(T t)
        {
            return IndexOf(t)==-1;
        }
        public int IndexOf(T t)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(t))
                {
                    return i;
                }
            }
            return -1;
        }
        public void Clear()
        {
            head = tail = 0;
        }
        public T[] ToArray()
        {
            T[] ts = new T[Count];
            for (int i = 0; i < Count; i++)
            {
                ts[i] = this[i];
            }
            return ts;
        }

        public bool IsReadOnly { get { return false; } }
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[arrayIndex+i] = this[i];
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(ToArray());
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(ToArray());
        }
        public object Clone()
        {
            return this.ReflectionDeepCopy();
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
