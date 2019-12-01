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
    public class Packet
    {
        public const int HelpBuffLen = 13;
        private static byte packFlag = 0xfe;
        private static byte subFlag = 0xfd;
        public static byte PackFlag { get { return packFlag; } }
        public static byte SubFlag { get { return subFlag; } }
        public PacketHeader Head { get; set; }
        public byte[] MsgBuff { get; set; }
        public byte[] Pack()
        {
            int plen = MsgBuff.Length;
            Head.MsgBuffLen = (UInt32)plen;
            byte[] buffer = new byte[HelpBuffLen + plen];
            buffer[0] = PackFlag;
            buffer[1] = (byte)(Head.PackID >> 24);
            buffer[2] = (byte)(Head.PackID >> 16);
            buffer[3] = (byte)(Head.PackID >> 8);
            buffer[4] = (byte)Head.PackID;
            buffer[5] = Head.PackType;
            buffer[6] = (byte)(Head.PackCount >> 8);
            buffer[7] = (byte)Head.PackCount;

            buffer[8] = (byte)(Head.MsgBuffLen >> 24);
            buffer[9] = (byte)(Head.MsgBuffLen >> 16);
            buffer[10] = (byte)(Head.MsgBuffLen >> 8);
            buffer[11] = (byte)(Head.MsgBuffLen);

            Buffer.BlockCopy(MsgBuff, 0, buffer, HelpBuffLen - 1, plen);
            buffer[buffer.Length - 1] = PackFlag;
            return Escape(buffer);
        }
        private unsafe byte[] Escape(byte[] buffer)
        {
            int pktCnt =0, subCnt = 0;
            /*var tCnt = */CheckEscapeFlagCount(buffer,out pktCnt,out subCnt);
            if (/*(tCnt.Item1 + tCnt.Item2) == 0*/ pktCnt+subCnt==0) return buffer;
            int plen = buffer.Length - 2;
            byte[] rBuffer = new byte[buffer.Length + pktCnt+subCnt /*tCnt.Item1 + tCnt.Item2*/];
            rBuffer[0] = buffer[0];//起始标识位
            fixed (byte* dst = &(rBuffer[1]), src = &(buffer[1]))
            {
                byte* _dst = dst;
                byte* _src = src;
                //消息头和消息体
                while (plen > 0)
                {
                    if (*_src == PackFlag)
                    {
                        *_dst = SubFlag;
                        *(_dst + 1) = 0x01;
                        _dst += 2;
                    }
                    else if (*_src == SubFlag)
                    {
                        *_dst = SubFlag;
                        *(_dst + 1) = 0x02;
                        _dst += 2;
                    }
                    else
                    {
                        *_dst = *_src;
                        _dst += 1;
                    }
                    _src += 1;
                    plen -= 1;
                }
                //结束标志位
                *_dst = *_src;
                return rBuffer;
            }
        }
        private unsafe void CheckEscapeFlagCount(byte[] buffer,out int pktCnt,out int subCnt)
        {
            int len = buffer.Length - 2;//去头尾标识位
                                        /* int*/
            pktCnt = 0;/*,*/ subCnt = 0;
            fixed (byte* src = &(buffer[1]))
            {
                byte* _src = src;
                do
                {
                    if (*_src == PackFlag)
                    {
                        ++pktCnt;
                    }
                    else if (*_src == SubFlag)
                    {
                        ++subCnt;
                    }
                    _src += 1;
                    --len;
                } while (len > 0);
            }
           // return Tuple.Create(pktCnt, subCnt);
        }
        public bool UnPack(byte[] buffer, int offset, int size)
        {
            if (buffer.Length < HelpBuffLen || buffer.Length < (offset + size))
                return false;// throw new Exception("{61573C82-F128-4ADE-A6AA-88004EB0EBBE}:有效字节长度过短");
            //还原转义并且过滤标志位,9为去掉标识位的head长度
            byte[] dst = Restore(buffer, offset, size);
            if (dst.Length < HelpBuffLen - 2)
                return false;//throw new Exception("{4DE7D881-0C40-4C09-8337-CE06CC2761FF}:转义还原数组溢出"+dst.Length);
            uint plen = dst.ToUInt32(7);
            if (plen > dst.Length - HelpBuffLen + 2)
                return false;
            if (Head == null)
                Head = new PacketHeader();
            Head.PackID = dst.ToUInt32(0);
            Head.PackType = dst[4];
            //if (Head == null)
            //    Head = new PacketAttribute();
            Head.PackCount = dst.ToUInt16(5);
            Head.MsgBuffLen = plen;// dst.ToUInt32(5);
            MsgBuff = new byte[Head.MsgBuffLen];
            Buffer.BlockCopy(dst, HelpBuffLen - 2, MsgBuff, 0, MsgBuff.Length);
            return true;
        }
        private unsafe byte[] Restore(byte[] buffer, int offset, int size)
        {
            int pkgCnt = 0, subCnt = 0;
            /*var tCnt =*/ CheckRestoreFlagCount(buffer, offset, size,out pkgCnt,out subCnt);
            if (/*(tCnt.Item1 + tCnt.Item2)*/pkgCnt+ subCnt == 0)
            {
                byte[] buff = new byte[size - 2];
                if (buffer.Length < offset + 1 + buff.Length)
                    throw new Exception("{FE815CC3-EA7D-49BF-89ED-E1B63D812D4F}:偏移长度溢出");
                Buffer.BlockCopy(buffer, offset + 1, buff, 0, buff.Length);
                return buff;
            }
            int pLen = size - 2;//去掉标志位后的长度
            byte[] rBuffer = new byte[pLen - pkgCnt- subCnt/*tCnt.Item1 - tCnt.Item2*/];
            fixed (byte* dst = rBuffer, src = &(buffer[offset + 1]))
            {
                byte* _src = src;
                byte* _dst = dst;
                //开始标志位
                //*(dst+0) = *(src + offset);
                //消息头和消息体
                while (pLen >= 0)
                {
                    if (*(_src) == SubFlag && *(_src + 1) == 0x01)
                    {
                        *(_dst) = PackFlag;
                        _src += 2;
                        _dst += 1;
                        pLen -= 2;
                    }
                    else if (*(_src) == SubFlag && *(_src + 1) == 0x02)
                    {
                        *(_dst) = SubFlag;
                        _src += 2;
                        _dst += 1;
                        pLen -= 2;
                    }
                    else
                    {
                        *(_dst) = *(_src);
                        _src += 1;
                        _dst += 1;
                        pLen -= 1;
                    }
                }
                return rBuffer;
            }
        }
        private unsafe void CheckRestoreFlagCount(byte[] buffer, int offset, int size, out int pkgCnt, out int subCnt)
        {
            int len = size - 2;
            //int i = offset + 1;
            //int pkgCnt = 0, subCnt = 0;
            pkgCnt = subCnt = 0;
            fixed (byte* src = &(buffer[offset + 1]))
            {
                byte* _src = src;
                do
                {
                    if (*_src == SubFlag && *(_src + 1) == 0x01)
                    {
                        ++pkgCnt;
                        _src += 2;
                        len -= 2;
                    }
                    else if (*_src == SubFlag && *(_src + 1) == 0x02)
                    {
                        ++subCnt;
                        _src += 2;
                        len -= 2;
                    }
                    else
                    {
                        _src += 1;
                        --len;
                    }
                } while (len > 0);
            }
            //return Tuple.Create(pkgCnt, subCnt);
        }
    }

}
