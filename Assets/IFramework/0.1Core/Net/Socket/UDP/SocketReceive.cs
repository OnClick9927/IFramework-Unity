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

namespace IFramework.Net
{
    public class SocketReceive : UdpSocket, IDisposable
    {
        private SocketAsyncEventArgs recArgs;
        //private LockParam lockParam = new LockParam();
        private bool isStoped = false;
        private bool _isDisposed = false;
        public event EventHandler<SocketAsyncEventArgs> OnReceived;
        public SocketReceive(int port, int bufferSize,bool broadcast=false) : base(bufferSize,broadcast)
        {
            CreateUdpSocket(port, IPAddress.Any);
            sock.Bind(endPoint);

            recArgs = new SocketAsyncEventArgs();
            recArgs.UserToken = sock;
            recArgs.RemoteEndPoint = sock.LocalEndPoint;
            recArgs.Completed += RecCompleted;
            recArgs.SetBuffer(recBuffer, 0, bufferSize);
        }
        public SocketReceive(Socket sock, int bufferSize) : base(bufferSize)
        {
            this.sock = sock;
            recArgs = new SocketAsyncEventArgs();
            recArgs.UserToken = sock;
            recArgs.RemoteEndPoint = sock.LocalEndPoint;
            recArgs.Completed += RecCompleted;
            recArgs.SetBuffer(recBuffer, 0, bufferSize);
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
                isStoped = true;
                _isDisposed = true;
                sock.Dispose();
                recArgs.Dispose();
            }
        }

        public void StartReceive()
        {
            bool rt = sock.ReceiveFromAsync(recArgs);
            if (rt == false)
            {
                ReceiveCallBack(recArgs);
            }
        }
        private void ReceiveCallBack(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (OnReceived != null)
                {
                    OnReceived(e.UserToken as Socket, e);
                }
            }
            if (isStoped) return;
            StartReceive();
        }
        private void RecCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ReceiveCallBack(e);
                    break;
            }
        }

        public void StopReceive()
        {
            isStoped = true;
            sock.Dispose();
            if (recArgs != null)
            {
                recArgs.Dispose();
            }
        }
    }

}
