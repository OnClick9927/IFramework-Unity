/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Net
{
    public class CycQueue<T> : IDisposable
    {
        private T[] array = null;
        private int capacity = 4;
        private int head = 0;
        private int tail = 0;
        public int Capacity { get { return capacity; } }
        public T[] Array { get { return array; } }
        public bool IsEmpty { get { return tail == head; } }
        public int Length { get { return (tail - head + capacity) % capacity; } }
        public bool IsFull { get { return (tail + 1) % capacity == head; } }
        public CycQueue(int capacity)
        {
            if (capacity < 4) capacity = 4;
            this.capacity = capacity;
            array = new T[capacity];
        }
        public bool EnQueue(T value)
        {
            if (IsFull) return false;
            array[tail] = value;
            tail = (tail + 1) % capacity;
            return true;
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
            if (size > Length) return null;
            T[] array = new T[size];
            int index = 0;
            while (size > 0)
            {
                if (IsEmpty) return null;
                array[index] = this.array[head];
                head = (head + 1) % capacity;
                size--;
                index++;
            }
            return array;
        }
        public void Clear()
        {
            head = tail = 0;
        }
        public void Clear(int size)
        {
            int len = size <= Length ? size : Length;
            while (len > 0)
            {
                if (IsEmpty) break;
                head = (head + 1) % capacity;
                len--;
            }
        }
        public int DeSearchIndex(T value, int offset)
        {
            if (offset > Length) return -1;
            if (offset > 0)
            {
                head = (head + offset) % capacity;
            }
            while (Length > 0)
            {
                if (IsEmpty) return -1;
                if (value.Equals(array[head])) return head;
                head = (head + 1) % capacity;
            }
            return -1;
        }
        public int PeekIndex(T value, int offset)
        {
            if (offset > Length) return -1;
            int _h = (head + offset) % capacity;
            if (array[_h].Equals(value)) return _h;
            return -1;
        }
        public void Dispose()
        {
            if (array != null)
                array = null;
        }
    }
}
