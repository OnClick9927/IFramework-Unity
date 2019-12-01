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
using System.Text;

namespace IFramework.Net
{
    public class UdpServerToken : UdpSocket, IDisposable
    {
        private SocketReceive socketRecieve = null;
        private SocketSend socketSend = null;
        private bool _isDisposed = false;
        public OnReceieve onReceive { get; set; }
        public OnSendCallBack onSendCallback { get; set; }
        public OnDisConnect onDisconnect { get; set; }
        public OnReceivedString onRecieveString { get; set; }
        private Encoding encoding = Encoding.UTF8;
        private int recBufferSize = 4096;
        private int maxConn = 8;
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
                socketRecieve.Dispose();
                socketSend.Dispose();
                _isDisposed = true;
            }
        }
        public UdpServerToken(int maxConn, int recBufferSize, bool Broadcast = false) : base(recBufferSize, Broadcast)
        {
            this.recBufferSize = recBufferSize;
            this.maxConn = maxConn;
        }
        public void Start(int port)
        {
            socketRecieve = new SocketReceive(port, maxConn, Broadcast);
            socketRecieve.OnReceived += ReceiveCallBack;
            socketRecieve.StartReceive();
            socketSend = new SocketSend(socketRecieve.sock, maxConn, recBufferSize);
            socketSend.SendEventHandler += SendCallBack;
        }
        public void Stop()
        {
            if (socketSend != null)
            {
                socketSend.Dispose();
            }
            if (socketRecieve != null)
            {
                socketRecieve.StopReceive();
            }
        }

        public bool Send(BufferSegment seg, IPEndPoint remoteEP, bool waiting = true)
        {
            return socketSend.Send(seg, remoteEP, waiting);
        }
        public int SendSync(IPEndPoint remoteEP, BufferSegment seg)
        {
            return socketSend.SendSync(seg, remoteEP);
        }
        private void SendCallBack(object sender, SocketAsyncEventArgs e)
        {
            if (onSendCallback != null && isServerResponse(e) == false)
            {
                onSendCallback(new SocketToken()
                {
                    EndPoint = (IPEndPoint)e.RemoteEndPoint
                }, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
            }
        }

        private void ReceiveCallBack(object sender, SocketAsyncEventArgs e)
        {
            //if (isClientRequest(e)) return;
            SocketToken token = new SocketToken()
            {
                EndPoint = (IPEndPoint)e.RemoteEndPoint
            };
            if (onReceive != null)
            {
                //if (e.BytesTransferred > 0)
                {
                    onReceive(token, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
                }
            }
            if (onRecieveString != null && e.BytesTransferred > 0)
            {
                onRecieveString(token, encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));
            }
        }

        private bool isClientRequest(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 1 && e.Buffer[0] == 0)
            {
                socketSend.Send(new BufferSegment(new byte[] { 1 }, 0, 1), (IPEndPoint)e.RemoteEndPoint, true);
                return true;
            }
            else return false;
        }

        private bool isServerResponse(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 1 && e.Buffer[0] == 1)
            {
                return true;
            }
            else return false;
        }

    }

}