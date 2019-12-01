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

namespace IFramework.Net
{
    public class SocketToken : IDisposable, IComparable<SocketToken>
    {
        private DateTime connTime;
        protected int tokenId;
        protected Socket sock;
        protected IPEndPoint endPoint;
        protected SocketAsyncEventArgs arg;
        public int TokenId { get { return tokenId; } set { tokenId = value; } }
        public Socket Sock { get { return sock; } set { sock = value; } }
        public IPEndPoint EndPoint { get { return endPoint; } set { endPoint = value; } }
        public SocketAsyncEventArgs Arg { get { return arg; } set { arg = value; } }
        public bool IsDisposed { get { return isDisposed; } }
        public DateTime ConnTime { get { return connTime; } set { connTime = value; } }

        private bool isDisposed = false;
        ~SocketToken()
        {
            Dispose(false);
        }
        public SocketToken() { }
        public SocketToken(int id) : this()
        {
            this.tokenId = id;
        }
        public void Close()
        {
            if (sock != null)
            {
                try
                {
                    if (arg.ConnectSocket != null)
                    {
                        arg.ConnectSocket.Close();
                    }
                    else if (arg.AcceptSocket != null)
                    {
                        arg.AcceptSocket.Close();
                    }
                    sock.Shutdown(SocketShutdown.Send);
                }
                catch (ObjectDisposedException) { return; }
                catch { }
                sock.Close();
            }
        }

        public int CompareTo(SocketToken token)
        {
            return this.tokenId.CompareTo(token.TokenId);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool dispose)
        {
            if (isDisposed) return;
            if (dispose)
            {
                arg.Dispose();
                sock.Dispose();
                sock = null;
                isDisposed = true;
            }
        }
    }

}
