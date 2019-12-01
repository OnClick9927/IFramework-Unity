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
    public class TcpServerToken : TcpSocket, IServerToken, IDisposable
    {
        private bool isRunning;
        private int curConCount;
        private int maxConCount;
        private bool _isDisposed = false;
        private int offsetNumber = 2;

        private LockParam lockParam;
        private Semaphore countSema;
        private SocketEventArgPool sendArgs;
        private SocketEventArgPool receiveArgs;
        private SockArgBuffers recBuffMgr;
        private SockArgBuffers sendBuffMgr;

        public int CurConCount { get { return curConCount; } }
        public int MaxConCount { get { return maxConCount; } }
        public bool IsRunning { get { return isRunning; } }
        private Encoding encoding = Encoding.UTF8;

        public OnReceivedString onRecieveString { get; set; }
        public OnAccept onAcccept { get; set; }
        public OnReceieve onReceive { get; set; }
        public OnDisConnect onDisConnect { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }

        public TcpServerToken(int maxConCount, int bufferSize) : base(bufferSize)
        {
            this.maxConCount = maxConCount;
            this.bufferSize = bufferSize;
            recBuffMgr = new SockArgBuffers(maxConCount + offsetNumber, bufferSize);
            sendBuffMgr = new SockArgBuffers(maxConCount + offsetNumber, bufferSize);
            sendArgs = new SocketEventArgPool(maxConCount + offsetNumber);
            receiveArgs = new SocketEventArgPool(maxConCount + offsetNumber);
            lockParam = new LockParam();
            countSema = new Semaphore(maxConCount + offsetNumber, maxConCount + offsetNumber);
        }
        private void InitArgs()
        {
            receiveArgs.Clear();
            sendArgs.Clear();
            sendBuffMgr.FreeBuffer();
            recBuffMgr.FreeBuffer();
            for (int i = 0; i < maxConCount + offsetNumber; i++)
            {
                SocketAsyncEventArgs senArg = new SocketAsyncEventArgs()
                {
                    DisconnectReuseSocket = true,
                    SocketError = SocketError.NotInitialized
                };
                senArg.Completed += IOCompleted;
                senArg.UserToken = new SocketToken(i);
                sendArgs.Set(senArg);
                sendBuffMgr.SetBuffer(senArg);

                SocketAsyncEventArgs recArg = new SocketAsyncEventArgs()
                {
                    DisconnectReuseSocket = true,
                    SocketError = SocketError.SocketError
                };
                recArg.Completed += IOCompleted;
                SocketToken token = new SocketToken(i)
                {
                    Arg = recArg
                };
                recArg.UserToken = token;
                recBuffMgr.SetBuffer(recArg);
                receiveArgs.Set(recArg);
            }
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
                SafeClose();
                sendBuffMgr.Clear();
                recBuffMgr.Clear();
                _isDisposed = true;
                countSema.Close();
                //countSema.Dispose();                       //.net4.X,ok
                sendArgs.Dispose();
                receiveArgs.Dispose();
            }
        }

        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    AcceptCallBack(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    DisconnectCallBack(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ReceiveCallBack(e);
                    break;
                case SocketAsyncOperation.Send:
                    SendCallBack(e);
                    break;
            }
        }

        public bool Start(int port, string ip = "0.0.0.0")
        {
            Stop();
            InitArgs();
            int errCount = 0;
            reStart:
            try
            {
                using (LockWait wait = new LockWait(ref lockParam))
                {
                    CreateTcpSocket(port, ip);
                    sock.Bind(EndPoint);
                    sock.Listen(maxConCount);
                    isRunning = true;
                }
                StartAcceptAsync(null);
                return true;
            }
            catch (Exception)
            {
                SafeClose();
                errCount++;
                if (errCount >= 3)
                {
                    throw;
                }
                else
                {
                    Thread.Sleep(1000);
                    goto reStart;
                }
            }
        }
        public void Stop()
        {
            try
            {
                using (LockWait wait = new LockWait(ref lockParam))
                {
                    if (curConCount > 0)
                    {
                        if (countSema != null)
                            countSema.Release(curConCount);
                        curConCount = 0;
                    }
                    SafeClose();
                    isRunning = false;
                }
            }
            catch (Exception) { }
        }
        private void StartAcceptAsync(SocketAsyncEventArgs e)
        {
            if (!isRunning || sock == null)
            {
                isRunning = false;
                return;
            }
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.DisconnectReuseSocket = true;
                e.UserToken = new SocketToken(-255);
                e.Completed += IOCompleted;
            }
            else
            {
                e.AcceptSocket = null;
            }
            countSema.WaitOne();
            if (!sock.AcceptAsync(e))
            {
                AcceptCallBack(e);
            }
        }

        private void AcceptCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                if (!isRunning /*|| curConCount >= maxConCount*/ || e.SocketError != SocketError.Success)
                {
                    //DisconnectCallBack(e);
                    //if (e.SocketError != SocketError.ConnectionReset)
                    //{
                    //    return;
                    //}
                    DisposeSocketArgs(e);
                    return;
                }
                SocketAsyncEventArgs recArg = receiveArgs.GetFreeArg((retry) => { return true; }, false);
                if (recArg == null)
                {
                    DisposeSocketArgs(e);
                    throw new Exception(string.Format("已经达到最大连接数max:{0};used:{1}", maxConCount, curConCount));
                }
                Interlocked.Increment(ref curConCount);
                SocketToken token = ((SocketToken)recArg.UserToken);
                token.Sock = e.AcceptSocket;
                token.EndPoint = (IPEndPoint)e.AcceptSocket.RemoteEndPoint;
                token.Arg = recArg;
                token.ConnTime = DateTime.Now;
                recArg.UserToken = token;
                if (e.AcceptSocket.Connected)
                {
                    if (curConCount > maxConCount)
                    {
                        token.Sock.Shutdown(SocketShutdown.Send);
                        token.Sock.Close();
                        receiveArgs.Set(recArg);
                        Interlocked.Decrement(ref curConCount);
                        countSema.Release();
                    }
                    else
                    {
                        if (onAcccept != null) onAcccept(token);
                        if (!token.Sock.ReceiveAsync(recArg))
                        {
                            ReceiveCallBack(recArg);
                        }
                    }
                }
                else
                {
                    DisconnectCallBack(recArg);
                }
                if (!isRunning) return;
            }
            catch (Exception exc)
            {
                Log.L(exc);
                throw;
            }
            finally
            {
                //if (e.SocketError != SocketError.ConnectionReset)
                //{
                StartAcceptAsync(e);
                //}
            }
        }
        private void DisposeSocketArgs(SocketAsyncEventArgs e)
        {
            SocketToken token = e.UserToken as SocketToken;
            if (token != null) token.Close();
            e.Dispose();
        }

        private void ReceiveCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
                {
                    DisconnectCallBack(e);
                    return;
                }
                SocketToken token = e.UserToken as SocketToken;
                if (onReceive != null) onReceive(token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
                if (onRecieveString != null) onRecieveString(token, encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));
                if (token.Sock.Connected)
                {
                    if (!token.Sock.ReceiveAsync(e))
                    {
                        ReceiveCallBack(e);
                    }
                }
                else
                {
                    DisconnectCallBack(e);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DisConnectAsync(SocketToken token)
        {
            try
            {
                if (token == null || token.Sock == null || token.Arg == null) return;
                if (token.Sock.Connected)
                    token.Sock.Shutdown(SocketShutdown.Send);
                SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                arg.DisconnectReuseSocket = true;
                arg.SocketError = SocketError.SocketError;
                arg.UserToken = null;
                arg.Completed += IOCompleted;
                if (token.Sock.DisconnectAsync(arg) == false)
                {
                    DisconnectCallBack(token.Arg);
                }
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DisconnectCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                SocketToken token = e.UserToken as SocketToken;
                if (token != null)
                {
                    if (token.TokenId == -255) return;
                    countSema.Release();
                    Interlocked.Decrement(ref curConCount);
                    if (onDisConnect != null)
                    {
                        onDisConnect(token);
                    }
                    token.Close();
                    receiveArgs.Set(e);
                }
                else
                {
                    throw new Exception("UserToken is Null,May From Accept");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int SendSync(SocketToken token, BufferSegment seg)
        {
            return token.Sock.Send(seg.Buffer, seg.Offset, seg.Len, SocketFlags.None);
        }
        public bool SendAsync(SocketToken token, BufferSegment seg, bool wait = false)
        {
            try
            {
                if (!token.Sock.Connected) return false;
                bool isWillEvent = true;
                ArraySegment<byte>[] segs = sendBuffMgr.BuffToSegs(seg.Buffer, seg.Offset, seg.Len);
                for (int i = 0; i < segs.Length; i++)
                {
                    SocketAsyncEventArgs senArg = GetFreeSendArg(wait, token.Sock);
                    if (senArg == null) return false;
                    senArg.UserToken = token;
                    if (!sendBuffMgr.WriteBuffer(senArg, segs[i].Array, segs[i].Offset, segs[i].Count))
                    {
                        sendArgs.Set(senArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", sendBuffMgr.BufferSize));
                    }
                    if (!token.Sock.Connected) return false;
                    isWillEvent &= token.Sock.SendAsync(senArg);
                    if (!isWillEvent)
                    {
                        SendCallBack(senArg);
                    }
                    if (sendArgs.Count < (sendArgs.Capcity >> 2))
                        Thread.Sleep(5);
                }
                return isWillEvent;
            }
            catch (Exception ex)
            {
                Close(token);
                throw ex;
            }
        }
        private SocketAsyncEventArgs GetFreeSendArg(bool wait, Socket sock)
        {
            SocketAsyncEventArgs tArgs = sendArgs.GetFreeArg((retry) =>
            {
                if (sock.Connected == false) return false;
                return true;
            }, wait);
            if (sock.Connected == false)
                return null;
            if (tArgs == null)
                throw new Exception("发送缓冲池已用完,等待回收超时...");
            return tArgs;
        }
        public void Close(SocketToken token)
        {
            DisConnectAsync(token);
        }

        private void SendCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    SocketToken token = e.UserToken as SocketToken;
                    if (onSendCallBack != null)
                    {
                        onSendCallBack(token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sendArgs.Set(e);
            }
        }
    }

}
