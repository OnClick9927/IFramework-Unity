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
    public class UdpSocket
    {
        internal Socket sock;
        protected bool isConnected;
        protected EndPoint endPoint;
        protected byte[] recBuffer;
        internal bool Broadcast = false;

        protected int bufferSize = 4096;
        protected int receiveTimeout = 1000 * 60 * 30;
        protected int sendTimeout = 1000 * 60 * 30;
        public UdpSocket(int bufferSize, bool broadcast = false)
        {
            Broadcast = broadcast;
            this.bufferSize = bufferSize;
            this.recBuffer = new byte[bufferSize];
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
            }
            try
            {
                sock.Close();
                sock.Dispose();
            }
            catch
            { }
        }
        protected void CreateUdpSocket(int port, IPAddress ip)
        {
            if (Broadcast)
                endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            else
                endPoint = new IPEndPoint(ip, port);

            sock = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = receiveTimeout,
                SendTimeout = sendTimeout
            };
            if (Broadcast)
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            else
                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
        }
    }

}
