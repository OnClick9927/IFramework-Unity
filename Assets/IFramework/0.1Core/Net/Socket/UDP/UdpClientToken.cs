/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace IFramework.Net
{
    public class UdpClientToken : UdpSocket, IDisposable
    {
        private bool _isDisposed = false;
        private int recBufferSize = 4096;
        private int maxConn = 8;
        private Encoding encoding = Encoding.UTF8;

        private LockParam lockParam = new LockParam();
        private ManualResetEvent mReset = new ManualResetEvent(false);

        private SocketEventArgPool sendArgs = null;
        private SockArgBuffers sendBuff = null;

        public int SendBufferPoolNumber { get { return sendArgs.Count; } }
        public OnReceieve onReceive { get; set; }
        public OnSendCallBack onSendCallback { get; set; }
        public OnReceivedString onRecieveString { get; set; }
        public UdpClientToken(int bufferSize, int maxConn=64) : base(bufferSize)
        {
            this.recBufferSize = bufferSize;
            this.maxConn = maxConn;

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
                if (sendBuff != null)
                {
                    sendBuff.Clear();
                    sendBuff.FreeBuffer();
                }
                SafeClose();
                _isDisposed = true;
            }
        }

        public void Disconnect()
        {
            Close();
            isConnected = false;
        }
        public bool Connect(int port, string ip )
        {
            Close();
            CreateUdpSocket(port, IPAddress.Parse(ip));
            sendArgs = new SocketEventArgPool(maxConn);
            sendBuff = new SockArgBuffers(maxConn, recBufferSize);
            for (int i = 0; i < maxConn; ++i)
            {
                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
                sendArg.Completed += IOCompleted;

                sendArg.UserToken = sock;
                sendBuff.SetBuffer(sendArg);
                sendArgs.Set(sendArg);
            }
            return true;
            //int retry = 3;
            //again:
            //try
            //{
            //    SocketAsyncEventArgs sArgs = new SocketAsyncEventArgs();
            //    sArgs.Completed += IOCompleted;
            //    sArgs.UserToken = sock;
            //    sArgs.RemoteEndPoint = endPoint;
            //    sArgs.SetBuffer(new byte[] { 0 }, 0, 1);
            //    bool rt = sock.SendToAsync(sArgs);
            //    if (rt)
            //    {
            //        StartReceive();
            //        mReset.WaitOne();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    retry -= 1;
            //    if (retry > 0)
            //    {
            //        Thread.Sleep(1000);
            //        goto again;
            //    }
            //    throw ex;
            //}
            //return isConnected;
        }
        private void Close()
        {
            if (sock!=null)
            {
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
                sock.Dispose();
                isConnected = false;
            }
        }
       

        public void StartReceive()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                SocketAsyncEventArgs recArg = new SocketAsyncEventArgs();
                recArg.Completed += IOCompleted;
                recArg.UserToken = sock;
                recArg.RemoteEndPoint = endPoint;
                recArg.SetBuffer(recBuffer, 0, recBufferSize);
                if (!sock.ReceiveFromAsync(recArg))
                {
                    ReceiveCallBack(recArg);
                }
            }
        }
        private void ReceiveCallBack(SocketAsyncEventArgs e)
        {
            SocketToken token = new SocketToken();
            token.Sock = e.UserToken as Socket;
            token.EndPoint = (IPEndPoint)e.RemoteEndPoint;
            try
            {
                if (e.SocketError != SocketError.Success || e.BytesTransferred == 0) return;
                if (isServerResponse(e)) return;
                if (onReceive != null) onReceive(token, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
                if (onRecieveString != null) onRecieveString(token, encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (!token.Sock.ReceiveFromAsync(e))
                    {
                        ReceiveCallBack(e);
                    }
                }
            }
        }
        public void ReceiveSync(BufferSegment segRecieve, Action<BufferSegment> receiveAction)
        {
            int cnt = 0;
            do
            {
                cnt = sock.ReceiveFrom(segRecieve.Buffer,
                    segRecieve.Len,
                    SocketFlags.None,
                    ref endPoint);

                if (cnt <= 0) break;

                receiveAction(segRecieve);
            } while (true);
        }
        public bool Send(BufferSegment segBuff, bool waiting = true)
        {
            try
            {
                bool isWillEvent = true;
                ArraySegment<byte>[] segItems = sendBuff.BuffToSegs(segBuff.Buffer, segBuff.Offset, segBuff.Len);
                foreach (var seg in segItems)
                {
                    SocketAsyncEventArgs sendArg = sendArgs.GetFreeArg((retry) => { return true; }, waiting);
                    if (sendArg == null)
                        throw new Exception("发送缓冲池已用完,等待回收...");
                    sendArg.RemoteEndPoint = endPoint;
                    if (!sendBuff.WriteBuffer(sendArg, seg.Array, seg.Offset, seg.Count))
                    {
                        sendArgs.Set(sendArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", sendBuff.BufferSize));
                    }
                    isWillEvent &= sock.SendToAsync(sendArg);
                    if (!isWillEvent)
                    {
                        SendCallBack(sendArg);
                    }
                }
                return isWillEvent;
            }
            catch (Exception ex)
            {
                Close();
                throw ex;
            }
        }
        public int SendSync(BufferSegment segSend, BufferSegment segRecieve)
        {
            int sent = sock.SendTo(segSend.Buffer, segSend.Offset, segSend.Len, 0, endPoint);
            if (segSend == null|| segSend.Buffer == null || segSend.Len == 0) return sent;
            /*int cnt =*/ sock.ReceiveFrom(segRecieve.Buffer, segRecieve.Offset, segRecieve.Len, SocketFlags.None, ref endPoint);
            return sent;
        }
        private void SendCallBack(SocketAsyncEventArgs e)
        {
            try
            {
              bool  sucess = e.SocketError == SocketError.Success;
                if (!isConnected && sucess)
                {
                    StartReceive();
                    isConnected = true;
                }
                if (onSendCallback != null && isClientRequest(e) == false)
                {
                    SocketToken token = new SocketToken();
                    token.Sock = e.UserToken as Socket;
                    token.EndPoint = (IPEndPoint)e.RemoteEndPoint;
                    onSendCallback(token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
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

        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ReceiveCallBack(e);
                    break;
                case SocketAsyncOperation.SendTo:
                    SendCallBack(e);
                    break;
            }
        }

        private bool isServerResponse(SocketAsyncEventArgs e)
        {
            isConnected = e.SocketError == SocketError.Success;
            if (e.BytesTransferred == 1 && e.Buffer[0] == 1)
            {
                mReset.Set();
                return true;
            }
            else return false;
        }
        private bool isClientRequest(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 1 && e.Buffer[0] == 0)
            {
                return true;
            }
            else return false;
        }
    }

}
