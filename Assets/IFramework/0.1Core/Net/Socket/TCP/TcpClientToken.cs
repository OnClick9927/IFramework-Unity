/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace IFramework.Net
{
    public enum ClientChannelType
    {
        Async = 0,
        AsyncWait = 1,
        Sync = 2
    }
    public class TcpClientToken : TcpSocket, IClientToken, IDisposable
    {
        private ClientChannelType channelType;
        private bool _isDisposed;
        private int bufferNumber = 8;
        private LockParam lockParam = new LockParam();
        private SocketEventArgPool sendArgs;
        private SockArgBuffers sendBuffMgr;
        public int BufferNumber { get { return bufferNumber; } }
        private AutoResetEvent autoReset = new AutoResetEvent(false);

        private Encoding encoding = Encoding.UTF8;

        public OnReceivedString onRecieveString { get; set; }
        public OnConnect onConnect { get; set; }
        public OnDisConnect onDisConnect { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }
        public OnReceieve onReceive { get; set; }

        public TcpClientToken(int bufferSize, int bufferNumber) : base(bufferSize)
        {
            this.recBuffer = new byte[bufferSize];
            this.bufferNumber = bufferNumber;
            sendArgs = new SocketEventArgPool(bufferNumber);
            sendBuffMgr = new SockArgBuffers(bufferNumber, bufferSize);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            if (isDisposing)
            {
                sendArgs.Clear();
                if (sendBuffMgr != null)
                {
                    sendBuffMgr.Clear();
                    sendBuffMgr.FreeBuffer();
                }

                _isDisposed = true;
                SafeClose();
            }
        }

        private void Close()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                sendArgs.Clear();
                if (sendBuffMgr != null)
                {
                    sendBuffMgr.Clear();
                    sendBuffMgr.FreeBuffer();
                }
                SafeClose();
                isConnected = false;
            }
        }

        public bool ConnectTo(int port, string ip)
        {
            try
            {
                if (isConnected == false || sock == null || sock.Connected == false)
                {
                    Close();
                }
                isConnected = false;
                channelType = ClientChannelType.AsyncWait;
                using (LockWait lwait = new LockWait(ref lockParam))
                {
                    CreateTcpSocket(port, ip);
                    //连接事件绑定
                    var sArgs = new SocketAsyncEventArgs
                    {
                        RemoteEndPoint = endPoint,
                        UserToken = new SocketToken() { Sock = sock }
                    };
                    sArgs.AcceptSocket = sock;
                    sArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IOCompleted);
                    if (!sock.ConnectAsync(sArgs))
                    {
                        ConnectCallback(sArgs);
                    }
                }
                autoReset.WaitOne(connTimeout);
                isConnected = sock.Connected;
                return isConnected;
            }
            catch (Exception ex)
            {
                Close();
                throw ex;
            }
        }
        public bool ConnectSync(int port, string ip)
        {
            if (IsConnected == false || sock == null || sock.Connected == false)
            {
                Close();
            }
            isConnected = false;
            channelType = ClientChannelType.Sync;
            int retry = 3;
            using (LockWait wait = new LockWait(ref lockParam))
            {
                CreateTcpSocket(port, ip);
            }
            while (retry > 0)
            {
                try
                {
                    --retry;
                    sock.Connect(endPoint);
                    isConnected = true;
                    return true;
                }
                catch (Exception ex)
                {
                    Close();
                    if (retry <= 0) throw ex;
                    Thread.Sleep(1000);
                }
            }
            return false;
        }
        public void ConnectAsync(int port, string ip)
        {
            try
            {
                if (IsConnected == false || sock == null || sock.Connected == false)
                {
                    Close();
                }
                isConnected = false;
                channelType = ClientChannelType.Async;
                using (LockWait wait = new LockWait(ref lockParam))
                {
                    CreateTcpSocket(port, ip);
                    SocketAsyncEventArgs conArg = new SocketAsyncEventArgs
                    {
                        RemoteEndPoint = endPoint,
                        UserToken = new SocketToken(-1) { Sock = sock }
                    };
                    conArg.AcceptSocket = sock;
                    conArg.Completed += new EventHandler<SocketAsyncEventArgs>(IOCompleted);
                    if (!sock.ConnectAsync(conArg))
                    {
                        ConnectCallback(conArg);
                    }
                }
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }
        private void ConnectCallback(SocketAsyncEventArgs e)
        {
            try
            {
                isConnected = (e.SocketError == SocketError.Success);
                if (isConnected)
                {
                    using (LockWait wait = new LockWait(ref lockParam))
                    {
                        InitializePool(bufferNumber);
                    }
                    e.SetBuffer(recBuffer, 0, BufferSize);
                    if (onConnect != null)
                    {
                        SocketToken token = e.UserToken as SocketToken;
                        token.EndPoint = (IPEndPoint)e.RemoteEndPoint;
                        onConnect(token, isConnected);
                    }
                    if (!e.AcceptSocket.ReceiveAsync(e))
                    {
                        ReceiveCallback(e);
                    }
                }
                else
                {
                    DisconnectAsync(e);
                }
                if (channelType == ClientChannelType.AsyncWait)
                    autoReset.Set();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int SendSync(BufferSegment sendbuff, BufferSegment recBuff)
        {
            if (channelType != ClientChannelType.Sync)
            {
                throw new Exception("需要使用同步连接...ConnectSync");
            }
            int sent = sock.Send(sendbuff.Buffer, sendbuff.Offset, sendbuff.Len, SocketFlags.None);
            if (recBuff.Buffer == null || recBuff.Len == 0) return sent;
            /*int cnt =*/ sock.Receive(recBuff.Buffer, recBuff.Offset, recBuff.Len, 0);
            return sent;
        }
        public void SendFile(string filename)
        {
            sock.SendFile(filename);
        }

        private SocketAsyncEventArgs GetFreeSendArg(bool wait)
        {
            SocketAsyncEventArgs senArg = sendArgs.GetFreeArg((retry) =>
            {
                return !(IsConnected == false || sock == null || sock.Connected == false);
            }, wait);
            if (IsConnected == false) return null;
            if (senArg == null)
                throw new Exception("发送缓冲池已用完,等待回收超时...");
            return senArg;
        }
        public bool SendAsync(BufferSegment segBuff, bool wait = true)
        {
            try
            {
                if (isConnected == false || sock == null || sock.Connected == false)
                {
                    Close();
                    return false;
                }
                ArraySegment<byte>[] segs = sendBuffMgr.BuffToSegs(segBuff.Buffer, segBuff.Offset, segBuff.Len);
                bool isWillEvent = true;
                foreach (var seg in segs)
                {
                    SocketAsyncEventArgs senArg = GetFreeSendArg(wait);
                    if (senArg == null) return false;
                    if (!sendBuffMgr.WriteBuffer(senArg, seg.Array, seg.Offset, seg.Count))
                    {
                        sendArgs.Set(senArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", sendBuffMgr.BufferSize));
                    }
                    if (senArg.UserToken == null)
                        ((SocketToken)senArg.UserToken).Sock = sock;
                    if (IsConnected == false || sock == null || sock.Connected == false)
                    {
                        Close();
                        return false;
                    }
                    isWillEvent &= sock.SendAsync(senArg);
                    if (!isWillEvent)//can't trigger the io complated event to do
                    {
                        SendCallback(senArg);
                    }
                    if (sendArgs.Count < (sendArgs.Capcity >> 2))
                        Thread.Sleep(2);
                }
                return isWillEvent;
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }
        public int SendSync(BufferSegment segBuff)
        {
            return sock.Send(segBuff.Buffer, segBuff.Offset, segBuff.Len, SocketFlags.None);
        }
        private void SendCallback(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (onSendCallBack != null)
                    {
                        SocketToken token = e.UserToken as SocketToken;
                        token.EndPoint = (IPEndPoint)e.RemoteEndPoint;
                        onSendCallBack( token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
                    }
                }
                else
                {
                    DisconnectAsync(e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sendArgs.Set(e);
            }
        }

        private void DisconnectAsync(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.AcceptSocket == null) return;

                bool willRaiseEvent = false;
                if (e.AcceptSocket != null && e.AcceptSocket.Connected)
                    willRaiseEvent = e.AcceptSocket.DisconnectAsync(e);

                if (!willRaiseEvent)
                {
                    DisconnectCallback(e);
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void DisconnectCallback(SocketAsyncEventArgs e)
        {
            try
            {
                isConnected = (e.SocketError == SocketError.Success);
                if (isConnected)
                {
                    Close();
                }
                if (onDisConnect != null)
                {
                    SocketToken token = e.UserToken as SocketToken;
                    token.EndPoint = (IPEndPoint)e.RemoteEndPoint;
                    onDisConnect(token);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void ReceiveSync(byte[] recbuff, int reclen, Action<byte[],int> receivedAction)
        {
            if (channelType != ClientChannelType.Sync)
            {
                throw new Exception("需要使用同步连接...ConnectSync");
            }
            int cnt = 0;
            do
            {
                if (sock.Connected == false) break;
                cnt = sock.Receive(recbuff, reclen, 0);
                if (cnt <= 0) break;
                receivedAction(recbuff,reclen);

            } while (true);
        }

        private void ReceiveCallback(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success || e.AcceptSocket.Connected == false)
            {
                DisconnectAsync(e);
                return;
            }
            SocketToken token = e.UserToken as SocketToken;
            token.EndPoint = (IPEndPoint)e.RemoteEndPoint;
            if (onReceive != null) onReceive(token, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
            if (onRecieveString != null) onRecieveString(token, encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));

            if (!Sock.Connected) return;
            if (!e.AcceptSocket.ReceiveAsync(e))
            {
                ReceiveCallback(e);
            }
        }
        private void InitializePool(int bufferNumber)
        {
            sendArgs.Clear();
            sendBuffMgr.Clear();
            sendBuffMgr.FreeBuffer();
            sendBuffMgr = new SockArgBuffers(bufferNumber, bufferSize);
            sendArgs = new SocketEventArgPool(bufferNumber);
            for (int i = 0; i < bufferNumber; ++i)
            {
                SocketAsyncEventArgs tArgs = new SocketAsyncEventArgs()
                {
                    DisconnectReuseSocket = true
                };
                tArgs.Completed += IOCompleted;
                tArgs.UserToken = new SocketToken(i)
                {
                    Sock = sock,
                    TokenId = i
                };
                sendBuffMgr.SetBuffer(tArgs);
                sendArgs.Set(tArgs);
            }
        }
        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ConnectCallback(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    DisconnectCallback(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ReceiveCallback(e);
                    break;
                case SocketAsyncOperation.Send:
                    SendCallback(e);
                    break;
            }
        }
        public void DisConnect()
        {
            isConnected = false;
            Close();
        }

    }
}
