/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace IFramework.Net
{
    public class WSServerToken : IDisposable
    {
        private Encoding encoding = Encoding.UTF8;
        TcpServerToken token = null;
        List<ConnectionInfo> ConnectionPool = null;
        Timer threadingTimer = null;
        int timeout = 1000 * 60 * 6;
        object lockobject = new object();

        public OnReceivedString onRecieveString { get; set; }
        public OnReceieve onReceieve { get; set; }
        public OnAccept onAccept { get; set; }
        public OnDisConnect onDisConnect { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }

        public WSServerToken(int maxCon = 32, int bufferSize = 4096)
        {
            ConnectionPool = new List<ConnectionInfo>(maxCon);

            token = new TcpServerToken(maxCon, bufferSize);
            //serverProvider.AcceptedCallback = new OnAcceptedHandler(AcceptedHandler);
            token.onDisConnect = new OnDisConnect(OnDisConnect);
            token.onReceive = new OnReceieve(OnReceieve);
            token.onSendCallBack = new OnSendCallBack(OnSendCallBack);

            threadingTimer = new Timer(new TimerCallback(TimingEvent), null, -1, -1);
        }

        public static WSServerToken CreateProvider(int maxConnections = 32, int bufferSize = 4096)
        {
            return new WSServerToken(maxConnections, bufferSize);
        }

        public void Dispose()
        {
            threadingTimer.Dispose();
        }

        private void TimingEvent(object obj)
        {
            lock (lockobject)
            {
                var items = ConnectionPool.FindAll(x => DateTime.Now.Subtract(x.ConnectedTime).TotalMilliseconds >= (timeout >> 1));

                foreach (var node in items)
                {
                    if (DateTime.Now.Subtract(node.ConnectedTime).TotalMilliseconds >= timeout)
                    {
                        CloseAndRemove(node);
                        continue;
                    }
                    SendPing(node.sToken);
                }
            }
        }

        public bool Start(int port, string ip = "0.0.0.0")
        {
            bool isOk = token.Start(port, ip);
            if (isOk)
            {
                threadingTimer.Change(timeout >> 1, timeout);
            }
            return isOk;
        }

        public void Stop()
        {
            threadingTimer.Change(-1, -1);
            lock (lockobject)
            {
                foreach (var node in ConnectionPool)
                {
                    CloseAndRemove(node);
                }
            }
        }

        public void Close(SocketToken sToken)
        {
            token.Close(sToken);
        }

        public bool Send(SocketToken sToken, string content, bool waiting = true)
        {
            var buffer = new WebsocketFrame().ToSegmentFrame(content);

            return token.SendAsync(sToken,buffer, waiting);
        }

        public bool Send(SocketToken token, BufferSegment seg, bool waiting = true)
        {
            return this.token.SendAsync(token, seg, waiting);
        }

        private void SendPing(SocketToken sToken)
        {
            token.SendAsync(sToken,new BufferSegment( new byte[] { 0x89, 0x00 }));
        }

        private void SendPong(SocketToken token, BufferSegment seg)
        {
            var buffer = new WebsocketFrame().ToSegmentFrame(seg, OpCodeType.Bong);

            this.token.SendAsync(token, buffer);
        }

        //private void AcceptedHandler(SocketToken sToken)
        //{

        //}

        private void OnDisConnect(SocketToken sToken)
        {
            // ConnectionPool.Remove(new ConnectionInfo() { sToken = sToken });
            Remove(sToken);

            if (onDisConnect != null) onDisConnect(sToken);
        }

        private void RefreshTimeout(SocketToken sToken)
        {
            foreach (var item in ConnectionPool)
            {
                if (item.sToken.TokenId == sToken.TokenId)
                {
                    item.ConnectedTime = DateTime.Now;
                    break;
                }
            }
        }

        private void OnReceieve(SocketToken token, BufferSegment seg)
        {
            var connection = ConnectionPool.Find(x => x.sToken.TokenId == token.TokenId);
            if (connection == null)
            {
                connection = new ConnectionInfo() { sToken = token };

                ConnectionPool.Add(connection);
            }

            if (connection.IsHandShaked == false)
            {
                var serverFrame = new WebsocketFrame();

                var access = serverFrame.GetHandshakePackage(seg);
                connection.IsHandShaked = access.IsHandShaked();

                if (connection.IsHandShaked == false)
                {
                    CloseAndRemove(connection);
                    return;
                }
                connection.ConnectedTime = DateTime.Now;

                var rsp = serverFrame.RspAcceptedFrame(access);

                this.token.SendAsync(token, rsp);

                connection.accessInfo = access;

                if (onAccept != null) onAccept(token);
            }
            else
            {
                RefreshTimeout(token);

                WebsocketFrame packet = new WebsocketFrame();
                bool isOk = packet.DecodingFromBytes(seg, true);
                if (isOk == false) return;

                if (packet.OpCode == 0x01)//text
                {
                    if (onRecieveString != null)
                        onRecieveString(token, encoding.GetString(seg.Buffer,
                        seg.Offset, seg.Len));

                    return;
                }
                else if (packet.OpCode == 0x08)//close
                {
                    CloseAndRemove(connection);
                    return;
                }
                else if (packet.OpCode == 0x09)//ping
                {
                    SendPong(token,seg);
                }
                else if (packet.OpCode == 0x0A)//pong
                {
                    //  SendPing(session.sToken);
                }

                if (onReceieve != null && packet.Payload.Len > 0)
                    onReceieve(token, packet.Payload);
            }
        }

        private void OnSendCallBack(SocketToken token, BufferSegment seg)
        {
            if (onSendCallBack != null)
            {
                onSendCallBack(token,seg);
            }
        }

        private void CloseAndRemove(ConnectionInfo connection)
        {
            bool isOk = Remove(connection);
            if (isOk)
            {
                token.Close(connection.sToken);
            }
        }

        private bool Remove(ConnectionInfo info)
        {

            return ConnectionPool.Remove(info);

        }

        private bool Remove(SocketToken sToken)
        {

            return ConnectionPool.RemoveAll(x => x.sToken.TokenId == sToken.TokenId) > 0;

        }

        internal class ConnectionInfo : IComparable<SocketToken>
        {
            public SocketToken sToken { get; set; }

            public bool IsHandShaked { get; set; }

            public AccessInfo accessInfo { get; set; }
            private DateTime conTime = DateTime.MinValue;
            public DateTime ConnectedTime { get { return conTime; } set { conTime = value; } }
            public int CompareTo(SocketToken info)
            {
                return sToken.TokenId - info.TokenId;
            }
        }
    }
}
