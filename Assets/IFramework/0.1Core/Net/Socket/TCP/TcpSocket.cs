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
    public class TcpSocket : ITcpSocket
    {
        protected Socket sock = null;
        protected bool isConnected = false;
        protected EndPoint endPoint = null;
        protected byte[] recBuffer = null;
        protected int recTimeout = 1000 * 60 * 30;
        protected int sendTimeout = 1000 * 60 * 30;
        protected int connTimeout = 1000 * 60 * 30;
        protected int bufferSize = 4096;
        public TcpSocket(int bufferSize)
        {
            this.bufferSize = bufferSize;
        }

        public Socket Sock { get { return sock; } }
        public bool IsConnected { get { return isConnected; } }
        public int RecTimeout { get { return recTimeout; } }
        public int SendTimeout { get { return sendTimeout; } }
        public int ConnTimeout { get { return connTimeout; } }
        public int BufferSize { get { return bufferSize; } }
        public EndPoint EndPoint { get { return endPoint; } }

        protected void CreateTcpSocket(int port, string ip)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            sock = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                LingerState = new LingerOption(true, 0),
                NoDelay = true,
                ReceiveTimeout = recTimeout,
                SendTimeout = sendTimeout
            };
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        protected void SafeClose()
        {
            if (sock == null) return;
            if (sock.Connected)
            {
                try
                {
                    sock.Disconnect(true);
                    sock.Shutdown(SocketShutdown.Send);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Log.L(ex.Message);
                    throw ex;
                }
            }
            try
            {
                sock.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected void SafeClose(Socket s)
        {
            if (s == null) return;
            if (s.Connected)
            {
                try
                {
                    s.Disconnect(true);
                    s.Shutdown(SocketShutdown.Send);
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
            try
            {
                s.Close();
                //s.Dispose();
                s = null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
