/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Text;

namespace IFramework.Net
{
    public class DataFrame
    {
        /// <summary>
        /// 如果为true则该消息为消息尾部,如果false为零则还有后续数据包;
        /// </summary>
        private bool isEof = true;
        public bool IsEof { get { return isEof; } set { isEof = value; } }
        /// <summary>
        /// RSV1,RSV2,RSV3,各1位，用于扩展定义的,如果没有扩展约定的情况则必须为0
        /// </summary>
        public bool Rsv1 { get; set; }

        public bool Rsv2 { get; set; }

        public bool Rsv3 { get; set; }
        /// <summary>
        ///0x0表示附加数据帧
        ///0x1表示文本数据帧
        ///0x2表示二进制数据帧
        ///0x3-7暂时无定义，为以后的非控制帧保留
        ///0x8表示连接关闭
        ///0x9表示ping
        ///0xA表示pong
        ///0xB-F暂时无定义，为以后的控制帧保留
        /// </summary>
        private byte opCode = 0x01;

        public byte OpCode { get { return opCode; } set { opCode = value; } }
        /// <summary>
        /// true使用掩码解析消息
        /// </summary>
        private bool mask = false;

        public bool Mask { get { return mask; } set { mask = value; } }

        public long PayloadLength { get; set; }

        //public int Continued { get; set; }

        public byte[] MaskKey { get; set; }

        //public UInt16 MaskKeyContinued { get; set; }

        public BufferSegment Payload { get; set; }

        public byte[] EncodingToBytes()
        {
            if (Payload == null
                || Payload.Buffer.LongLength != PayloadLength)
                throw new Exception("payload buffer error");


            if (Payload.Len > 0)
            {
                PayloadLength = Payload.Len;
            }

            long headLen = (Mask ? 6 : 2);
            if (PayloadLength < 126)
            { }
            else if (PayloadLength >= 126 && PayloadLength < 127)
            {
                headLen += 2;
            }
            else if (PayloadLength >= 127)
            {
                headLen += 8;
            }

            byte[] buffer = new byte[headLen + PayloadLength];
            int pos = 0;

            buffer[pos] = (byte)(IsEof ? 128 : 0);
            buffer[pos] += OpCode;

            buffer[++pos] = (byte)(Mask ? 128 : 0);
            if (PayloadLength < 0x7e)//126
            {
                buffer[pos] += (byte)PayloadLength;
            }
            else if (PayloadLength < 0xffff)//65535
            {
                buffer[++pos] = 126;
                buffer[++pos] = (byte)(buffer.Length >> 8);
                buffer[++pos] = (byte)(buffer.Length);
            }
            else
            {
                var payLengthBytes = ((ulong)PayloadLength).ToBytes();
                buffer[++pos] = 127;

                buffer[++pos] = payLengthBytes[0];
                buffer[++pos] = payLengthBytes[1];
                buffer[++pos] = payLengthBytes[2];
                buffer[++pos] = payLengthBytes[3];
                buffer[++pos] = payLengthBytes[4];
                buffer[++pos] = payLengthBytes[5];
                buffer[++pos] = payLengthBytes[6];
                buffer[++pos] = payLengthBytes[7];
            }

            if (Mask)
            {
                buffer[++pos] = MaskKey[0];
                buffer[++pos] = MaskKey[1];
                buffer[++pos] = MaskKey[2];
                buffer[++pos] = MaskKey[3];

                for (long i = 0; i < PayloadLength; ++i)
                {
                    buffer[headLen + i] = (byte)(Payload.Buffer[i + Payload.Offset] ^ MaskKey[i % 4]);
                }
            }
            else
            {
                for (long i = 0; i < PayloadLength; ++i)
                {
                    buffer[headLen + i] = Payload.Buffer[i + Payload.Offset];
                }
            }
            return buffer;
        }

        public bool DecodingFromBytes(BufferSegment data, bool isMaskResolve = true)
        {
            if (data.Len < 4) return false;

            int pos = data.Offset;

            IsEof = (data.Buffer[pos] >> 7) == 1;
            OpCode = (byte)(data.Buffer[pos] & 0xf);

            Mask = (data.Buffer[++pos] >> 7) == 1;
            PayloadLength = (data.Buffer[pos] & 0x7f);

            //校验截取长度
            if (PayloadLength >= data.Len) return false;

            ++pos;
            //数据包长度超过126，需要解析附加数据
            if (PayloadLength < 126)
            {
                //直接等于消息长度
            }
            if (PayloadLength == 126)
            {
                PayloadLength = data.Buffer.ToUInt16(pos);// BitConverter.ToUInt16(segOffset.buffer, pos);
                pos += 2;
            }
            else if (PayloadLength == 127)
            {
                PayloadLength = (long)data.Buffer.ToUInt64(pos);
                pos += 8;
            }

            Payload = new BufferSegment()
            {
                Offset = pos,
                Buffer = data.Buffer,
                Len = (int)PayloadLength
            };

            //数据体
            if (Mask)
            {
                //获取掩码密钥
                MaskKey = new byte[4];
                MaskKey[0] = data.Buffer[pos];
                MaskKey[1] = data.Buffer[pos + 1];
                MaskKey[2] = data.Buffer[pos + 2];
                MaskKey[3] = data.Buffer[pos + 3];
                pos += 4;

                Payload.Buffer = data.Buffer;
                Payload.Offset = pos;
                if (isMaskResolve)
                {
                    long p = 0;

                    for (long i = 0; i < PayloadLength; ++i)
                    {
                        p = (long)pos + i;

                        Payload.Buffer[p] = (byte)(Payload.Buffer[p] ^ MaskKey[i % 4]);
                    }
                }
            }
            else
            {
                Payload.Buffer = data.Buffer;
                Payload.Offset = pos;
            }

            return true;
        }
    }
    public class WebsocketFrame : DataFrame
    {
        private Encoding encoding = Encoding.UTF8;
        private const string acceptMask = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";//固定字符串
        private readonly char[] splitChars = null;

        public WebsocketFrame()
        {
            splitChars = BaseInfo.SplitChars.ToCharArray();
        }

        public BufferSegment RspAcceptedFrame(AccessInfo access)
        {
            var accept = new AcceptInfo()
            {
                Connection = access.Connection,
                Upgrade = access.Upgrade,
                SecWebSocketLocation = access.Host,
                SecWebSocketOrigin = access.Origin,
                SecWebSocketAccept = (access.SecWebSocketKey + acceptMask).ToSha1Base64(encoding)
            };

            return new BufferSegment(encoding.GetBytes(accept.ToString()));
        }

        public AcceptInfo ParseAcceptedFrame(string msg)
        {
            string[] msgs = msg.Split(BaseInfo.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var acceptInfo = new AcceptInfo
            {
                HttpProto = msgs[0]
            };

            foreach (var item in msgs)
            {
                string[] kv = item.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                switch (kv[0])
                {
                    case "Upgrade":
                        acceptInfo.Upgrade = kv[1];
                        break;
                    case "Connection":
                        acceptInfo.Connection = kv[1];
                        break;
                    case "Sec-WebSocket-Accept":
                        acceptInfo.SecWebSocketAccept = kv[1];
                        break;
                    case "Sec-WebSocket-Location":
                        acceptInfo.SecWebSocketLocation = kv[1];
                        break;
                    case "Sec-WebSocket-Origin":
                        acceptInfo.SecWebSocketOrigin = kv[1];
                        break;
                }
            }
            return acceptInfo;
        }

        public BufferSegment ToSegmentFrame(string content)
        {
            var buf = encoding.GetBytes(content);
            Payload = new BufferSegment()
            {
                Buffer = buf
            };

            PayloadLength = Payload.Buffer.LongLength;

            return new BufferSegment(EncodingToBytes());
        }

        public BufferSegment ToSegmentFrame(byte[] buf, OpCodeType code = OpCodeType.Text)
        {
            OpCode = (byte)code;

            Payload = new BufferSegment()
            {
                Buffer = buf
            };
            PayloadLength = Payload.Buffer.LongLength;

            return new BufferSegment(EncodingToBytes());
        }

        public BufferSegment ToSegmentFrame(BufferSegment data, OpCodeType code = OpCodeType.Text)
        {
            OpCode = (byte)code;

            Payload = data;
            PayloadLength = Payload.Buffer.LongLength;

            return new BufferSegment(EncodingToBytes());
        }

        public AccessInfo GetHandshakePackage(BufferSegment segOffset)
        {
            string msg = encoding.GetString(segOffset.Buffer, segOffset.Offset, segOffset.Len);

            string[] items = msg.Split(BaseInfo.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (items.Length < 6)
                throw new Exception("access format error..." + msg);

            AccessInfo access = new AccessInfo()
            {
                HttpProto = items[0]
            };

            foreach (var item in items)
            {
                string[] kv = item.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                switch (kv[0])
                {
                    case "Connection":
                        access.Connection = kv[1];
                        break;
                    case "Host":
                        access.Host = kv[1];
                        break;
                    case "Origin":
                        access.Origin = kv[1];
                        break;
                    case "Upgrade":
                        access.Upgrade = kv[1];
                        break;
                    case "Sec-WebSocket-Key":
                        access.SecWebSocketKey = kv[1];
                        break;
                    case "Sec-WebSocket-Version":
                        access.SecWebSocketVersion = kv[1];
                        break;
                    case "Sec-WebSocket-Extensions":
                        access.SecWebSocketExtensions = kv[1];
                        break;
                }
            }
            return access;
        }
    }

    [Flags]
    public enum OpCodeType : byte
    {
        Attach = 0x0,
        Text = 0x1,
        Bin = 0x2,
        Close = 0x8,
        Bing = 0x9,
        Bong = 0xA,
    }
}
