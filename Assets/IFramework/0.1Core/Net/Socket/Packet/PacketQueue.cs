/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework.Net
{
    public class PacketQueue
    {
        private CycQueue<byte> bucket = null;
        private Packet pkg = null;
        public PacketQueue(int maxCount)
        {
            bucket = new CycQueue<byte>(maxCount);
            pkg = new Packet();
        }
        public int Count { get { return bucket.Length; } }
        public bool Set(byte[] buffer, int offset, int size)
        {
            if (bucket.Capacity - bucket.Length < size)
                return false;
            for (int i = 0; i < size; ++i)
            {
                bool rt = bucket.EnQueue(buffer[i + offset]);
                if (rt == false)
                    return false;
            }
            return true;
        }
        public List<Packet> Get()
        {
            int _head = -1, _pos = 0;
            List<Packet> pkgs = new List<Packet>(2);
            again:
            _head = bucket.DeSearchIndex(Packet.PackFlag, _pos);
            if (_head == -1) return pkgs;
            int peek = bucket.PeekIndex(Packet.PackFlag, 1);
            if (peek >= 0)
            {
                _pos = 1;
                goto again;
            }
            //数据包长度
            int pkgLength = CheckCompletePackageLength(bucket.Array, _head);
            if (pkgLength == 0) return pkgs;
            //读取完整包并移出队列
            byte[] array = bucket.DeQueue(pkgLength);
            if (array == null) return pkgs;
            //解析
            bool rt = pkg.UnPack(array, 0, array.Length);
            if (rt)
            {
                pkgs.Add(pkg);
            }
            if (bucket.Length > 0)
            {
                _pos = 0;
                goto again;
            }
            return pkgs;
        }
        private unsafe int CheckCompletePackageLength(byte[] buff, int offset)
        {
            fixed (byte* src = buff)
            {
                int head = offset;
                int cnt = 0;
                byte flag = 0;
                do
                {
                    if (*(src + head) == Packet.PackFlag)
                    {
                        ++flag;
                        if (flag == 2) return cnt + 1;
                    }
                    head = (head + 1) % bucket.Capacity;
                    ++cnt;
                }
                while (cnt <= bucket.Length);
                cnt = 0;
                return cnt;
            }
        }
        public void Clear()
        {
            bucket.Clear();
        }
    }

}
