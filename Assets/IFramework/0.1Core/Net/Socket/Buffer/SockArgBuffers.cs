/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace IFramework.Net
{
    public class SockArgBuffers
    {
        private int totalSize = 0;
        private int curOffset = 0;
        private int bufferSize = 2048;
        private LockParam lockParam = new LockParam();
        private byte[] buff = null;
        private Queue<int> freeOffsets = null;
        public int BufferSize { get { return bufferSize; } }
        public SockArgBuffers(int maxCounts, int bufferSize)
        {
            if (bufferSize < 4) bufferSize = 4;
            this.bufferSize = bufferSize;
            this.curOffset = 0;
            totalSize = maxCounts * bufferSize;
            buff = new byte[totalSize];
            freeOffsets = new Queue<int>(maxCounts);
        }
        public void FreeBuffer()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                curOffset = 0;
                freeOffsets.Clear();
            }
        }
        public void Clear()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                freeOffsets.Clear();
            }
        }

        public bool SetBuffer(SocketAsyncEventArgs agrs)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                if (freeOffsets.Count > 0)
                {
                    agrs.SetBuffer(this.buff, this.freeOffsets.Dequeue(), bufferSize);
                }
                else
                {
                    if ((totalSize - bufferSize) < curOffset) return false;
                    agrs.SetBuffer(this.buff, this.curOffset, this.bufferSize);
                    this.curOffset += this.bufferSize;
                }
                return true;
            }
        }
        public bool WriteBuffer(SocketAsyncEventArgs agrs, byte[] buffer, int offset, int len)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                if (agrs.Offset + len > this.buff.Length) return false;
                if (len > bufferSize) return false;
                Buffer.BlockCopy(buffer, offset, this.buff, agrs.Offset, len);
                agrs.SetBuffer(this.buff, agrs.Offset, len);
                return true;
            }
        }
        public void FreeBuffer(SocketAsyncEventArgs arg)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                this.freeOffsets.Enqueue(arg.Offset);
                arg.SetBuffer(null, 0, 0);
            }
        }
        public ArraySegment<byte>[] BuffToSegs(byte[] buffer, int offset, int len)
        {
            if (len <= bufferSize)
                return new ArraySegment<byte>[] { new ArraySegment<byte>(buffer, offset, len) };
            int bSize = bufferSize;
            int bCnt = len / bufferSize;
            int bOffset = 0;
            bool isRem = false;
            if (len % bufferSize != 0)
            {
                isRem = true;
                bCnt += 1;
            }
            ArraySegment<byte>[] segItems = new ArraySegment<byte>[bCnt];
            for (int i = 0; i < bCnt; ++i)
            {
                bOffset = i * bufferSize;
                if (i == (bCnt - 1) && isRem)
                {
                    bSize = len - bOffset;
                }
                segItems[i] = new ArraySegment<byte>(buffer, offset + bOffset, bSize);
            }
            return segItems;
        }
    }

}
