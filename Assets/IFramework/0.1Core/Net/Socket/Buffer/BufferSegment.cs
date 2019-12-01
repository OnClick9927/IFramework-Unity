/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-26
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Net
{
    public class BufferSegment
    {
        private byte[] buffer;
        private int offset;
        private int len;
        public byte[] Buffer { get { return buffer; } set { buffer = value; } }
        public int Offset { get { return offset; } set { offset = value; } }
        public int Len { get { return len; } set { len = value; } }
        public BufferSegment() { }
        public BufferSegment(byte[] buffer) : this(buffer, 0, buffer.Length) { }
        public BufferSegment(byte[] buffer, int len) : this(buffer, 0, len) { }
        public BufferSegment(byte[] buffer, int offset, int Len)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.len = Len;
        }
    }

}
