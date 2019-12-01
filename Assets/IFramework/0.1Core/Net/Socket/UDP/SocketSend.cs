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

namespace IFramework.Net
{
    public class SocketSend : UdpSocket, IDisposable
    {
        private SocketEventArgPool sendArgs;
        private SockArgBuffers sendBuff;
        private bool _isDisposed;
        public event EventHandler<SocketAsyncEventArgs> SendEventHandler;

        public SocketSend(int maxCount, int bufferSize = 4096) : base(bufferSize)
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.ReceiveTimeout = receiveTimeout;
            sock.SendTimeout = sendTimeout;
            sendArgs = new SocketEventArgPool(maxCount);
            sendBuff = new SockArgBuffers(maxCount, bufferSize);
            for (int i = 0; i < maxCount; ++i)
            {
                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                socketArgs.UserToken = sock;
                socketArgs.Completed += SendCompleted;
                sendBuff.SetBuffer(socketArgs);
                sendArgs.Set(socketArgs);
            }
        }
        public SocketSend(Socket sock, int maxCount, int bufferSize = 4096) : base(bufferSize)
        {
            base.sock = sock;
            sendArgs = new SocketEventArgPool(maxCount);
            sendBuff = new SockArgBuffers(maxCount, bufferSize);
            for (int i = 0; i < maxCount; ++i)
            {
                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                socketArgs.UserToken = sock;
                socketArgs.Completed += SendCompleted;
                sendBuff.SetBuffer(socketArgs);
                sendArgs.Set(socketArgs);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool dispose)
        {
            if (_isDisposed) return;
            if (dispose)
            {
                sendArgs.Clear();
                sock.Dispose();
                sendBuff.Clear();
                _isDisposed = true;
            }
        }

        public bool Send(BufferSegment segBuff, IPEndPoint remoteEP, bool waiting)
        {
            try
            {
                bool isWillEvent = true;
                ArraySegment<byte>[] segItems = sendBuff.BuffToSegs(segBuff.Buffer, segBuff.Offset, segBuff.Len);
                foreach (var seg in segItems)
                {
                    var SendArg = sendArgs.GetFreeArg((retry) => { return true; }, waiting);
                    if (SendArg == null)
                        throw new Exception("发送缓冲池已用完,等待回收超时...");
                    SendArg.RemoteEndPoint = remoteEP;
                    //Socket s = SocketVersion(remoteEP);
                    //SendArg.UserToken = s;
                    if (!sendBuff.WriteBuffer(SendArg, seg.Array, seg.Offset, seg.Count))
                    {
                        sendArgs.Set(SendArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", sendBuff.BufferSize));
                    }
                    if (SendArg.RemoteEndPoint != null)
                    {
                        isWillEvent &= sock.SendToAsync(SendArg);
                        if (!isWillEvent)
                        {
                            SendCallBack(SendArg);
                        }
                    }
                    Thread.Sleep(5);
                }
                return isWillEvent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SendCallBack(SocketAsyncEventArgs e)
        {
            sendArgs.Set(e);
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (SendEventHandler != null)
                {
                    SendEventHandler(e.UserToken as Socket, e);
                }
            }
        }
        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.SendTo:
                case SocketAsyncOperation.SendPackets:
                case SocketAsyncOperation.Send:
                    SendCallBack(e);
                    break;
            }
        }

        public int SendSync(BufferSegment segBuff, IPEndPoint remoteEP)
        {
            return sock.SendTo(segBuff.Buffer, segBuff.Offset, segBuff.Len, SocketFlags.None, remoteEP);
        }

        //private Socket SocketVersion(IPEndPoint ips)
        //{
        //    if (ips.AddressFamily == sock.AddressFamily)
        //    {
        //        return sock;
        //    }
        //    else
        //    {
        //        sock = new Socket(ips.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        //    }
        //    return sock;
        //}
    }

}
