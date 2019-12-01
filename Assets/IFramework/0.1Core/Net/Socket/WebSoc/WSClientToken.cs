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
using System.Threading;

namespace IFramework.Net
{
    public class WSClientToken : IDisposable
    {
        TcpClientToken token = null;
        private Encoding encoding = Encoding.UTF8;
        ManualResetEvent resetEvent = new ManualResetEvent(false);
        int waitingTimeout = 1000 * 60 * 30;
        public bool IsConnected { get; private set; }
        AcceptInfo acceptInfo = null;

        public OnDisConnect onDisConnect { get; set; }

        public OnConnect onConnect { get; set; }

        public OnReceivedString onReceivedString { get; set; }
        public OnReceieve onReceieve { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }
        public WSClientToken(int bufferSize = 4096, int blocks = 8)
        {
            token = new TcpClientToken(bufferSize, blocks);
            token.onDisConnect = new OnDisConnect(DisconnectedHandler);
            token.onReceive = new OnReceieve(ReceivedHanlder);
            token.onSendCallBack = new OnSendCallBack(SentHandler);
        }

        public void Dispose()
        {
            //resetEvent.Dispose();
            resetEvent.Close();
        }

        public static WSClientToken CreateProvider(int bufferSize = 4096, int blocks = 8)
        {
            return new WSClientToken(bufferSize, blocks);
        }

        /// <summary>
        /// wsUrl:ws://ip:port
        /// </summary>
        /// <param name="wsUrl"></param>
        /// <returns></returns>
        public bool Connect(string wsUrl)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            WSConnectionItem wsItem = new WSConnectionItem(wsUrl);

            bool isOk = token.ConnectTo(wsItem.Port, wsItem.Domain);
            if (isOk == false) throw new Exception("连接失败...");

            string req = new AccessInfo()
            {
                Host = wsItem.Host,
                Origin = "http://" + wsItem.Host,
                SecWebSocketKey = Convert.ToBase64String(encoding.GetBytes(wsUrl + rand.Next(100, 100000).ToString()))
            }.ToString();

            isOk = token.SendAsync(new BufferSegment(encoding.GetBytes(req)));

            resetEvent.WaitOne(waitingTimeout);

            return IsConnected;
        }

        public bool Connect(WSConnectionItem wsUrl)
        {
            return Connect(wsUrl);
        }

        public bool Send(string msg, bool waiting = true)
        {
            if (IsConnected == false) return false;

            var buf = new WebsocketFrame().ToSegmentFrame(msg);
            token.SendAsync(buf, waiting);
            return true;
        }

        public bool Send(BufferSegment data, bool waiting = true)
        {
            if (IsConnected == false) return false;

            token.SendAsync(data, waiting);
            return true;
        }

        public void SendPong(BufferSegment buf)
        {
            var seg = new WebsocketFrame().ToSegmentFrame(buf, OpCodeType.Bong);
            token.SendAsync(seg, true);
        }

        public void SendPing()
        {
            var buf = new WebsocketFrame().ToSegmentFrame(new byte[] { }, OpCodeType.Bing);
            token.SendAsync(buf, true);
        }

        private void DisconnectedHandler(SocketToken sToken)
        {
            IsConnected = false;
            if (onDisConnect != null) onDisConnect(sToken);
        }

        private void ReceivedHanlder(SocketToken token,BufferSegment seg)
        {
            if (IsConnected == false)
            {
                string msg = encoding.GetString(seg.Buffer, seg.Offset, seg.Len);
                acceptInfo = new WebsocketFrame().ParseAcceptedFrame(msg);

                if ((IsConnected = acceptInfo.IsHandShaked()))
                {
                    resetEvent.Set();
                    if (onConnect != null) onConnect(token, IsConnected);
                }
                else
                {
                    this.token.DisConnect();
                }
            }
            else
            {
                WebsocketFrame packet = new WebsocketFrame();
                bool isOk = packet.DecodingFromBytes(seg, true);
                if (isOk == false) return;

                if (packet.OpCode == 0x01)
                {
                    if (onReceivedString != null)
                        onReceivedString(token, encoding.GetString(packet.Payload.Buffer,
                        packet.Payload.Offset, packet.Payload.Len));

                    return;
                }
                else if (packet.OpCode == 0x08)//close
                {
                    IsConnected = false;
                    this.token.DisConnect();
                }
                else if (packet.OpCode == 0x09)//ping
                {
                    SendPong(seg);
                }
                else if (packet.OpCode == 0x0A)//pong
                {
                    SendPing();
                }

                if (onReceieve != null && packet.Payload.Len > 0)
                    onReceieve(token, packet.Payload);
            }
        }
        private void SentHandler(SocketToken token, BufferSegment seg)
        {
            if (onSendCallBack != null)
            {
                onSendCallBack(token ,seg);
            }
        }
        //private void ConnectedHandler(SocketToken sToken,bool isConnected)
        //{

        //}
    }
}
