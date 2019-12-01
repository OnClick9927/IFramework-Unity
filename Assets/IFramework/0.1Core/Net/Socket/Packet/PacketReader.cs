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
    public class PacketReader
    {
        private PacketQueue packetQueue;
        private LockParam lockParam;
        public int Count { get { return packetQueue.Count; } }
        public PacketReader(int capacity = 128)
        {
            if (capacity < 128) capacity = 128;
            capacity += 1;
            packetQueue = new PacketQueue(capacity);
            lockParam = new LockParam();
        }
        public bool Set(byte[] buff, int offset, int size)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return packetQueue.Set(buff, offset, size);
            }
        }
        public List<Packet> Get()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return packetQueue.Get();
            }
        }
        public void Clear()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                packetQueue.Clear();
            }
        }
    }

}
