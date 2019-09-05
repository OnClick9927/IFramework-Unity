/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace IFramework.Example
{
	public class SocExample
	{
        void packetTest()
        {
            Packet pack = new Packet();
            pack.Head = new PacketHeader();
            pack.Head.PackCount = 1;
            pack.Head.PackID = 11;
            pack.Head.PackType = 2;
            pack.MsgBuff = Encoding.Default.GetBytes("haha");
            byte[] buf = pack.Pack();

            Packet pac = new Packet();
            pac.UnPack(buf, 0, buf.Length);
            PacketReader p = new PacketReader(128);
            p.Set(buf, 0, buf.Length);
            p.Set(buf, 0, buf.Length);

            List<Packet> ps = p.Get();
            Log.L(ps.Count);
        }
        void Tcp()
        {
            TcpServerToken token = new TcpServerToken(2, 2048);
            token.onAcccept += (tok) =>
            {
                Log.L(token.CurConCount);
                Log.L("Accept " + tok.EndPoint);

            };
            token.onReceive += (tok, seg) =>
            {
                //Log.I("Rec " + tok.EndPoint);
                byte[] buffer = new byte[seg.Len];
                Array.Copy(seg.Buffer, seg.Offset, buffer, 0, seg.Len);
                Log.L(Encoding.UTF8.GetString(buffer) + " SS ");
                token.SendAsync( tok,seg, true);
            };

            token.onDisConnect += (tok) =>
            {
                Log.L("Dis " + tok.EndPoint);
                Log.L(token.CurConCount);
            };
            token.Start(8888);
            // token.Stop();
            TcpClientToken c = new TcpClientToken(2048, 12);
            c.onReceive += (tok, seg) =>
            {
                //Log.I("Rec " + tok.EndPoint);
                byte[] buffer = new byte[seg.Len];
                Array.Copy(seg.Buffer, seg.Offset, buffer, 0, seg.Len);
                Thread.Sleep(1000);
                Log.L(Encoding.UTF8.GetString(buffer) + " cc ");
                c.SendAsync(seg, true);
            };
            c.ConnectAsync(8888, "127.0.0.1");
            Log.L(c.IsConnected);
            byte[] bu = Encoding.UTF8.GetBytes("123");
            c.SendAsync(new BufferSegment( bu, 0, bu.Length));
            Thread.Sleep(1000);

            c.DisConnect();
            while (true)
            {
                if (c.IsConnected)
                {
                    c.SendAsync(new BufferSegment( bu, 0, bu.Length));
                }
                Thread.Sleep(100);
            }
        }
        void Udp()
        {
            UdpServerToken s = new UdpServerToken(4096,32);
            s.onReceive += (tok,seg) =>
            {
                byte[] buffer = new byte[seg.Len];
                Array.Copy(seg.Buffer, seg.Offset, buffer, 0, seg.Len);
                Log.L("SS Rec " + tok.EndPoint + " " + Encoding.UTF8.GetString(buffer));
                s.Send(seg, tok.EndPoint);
            };

            s.Start(8888);
            // s.Stop();
            UdpClientToken c = new UdpClientToken(2048, 10);
            c.onReceive += (tok,seg) =>
            {
                byte[] buffer = new byte[seg.Len];
                Array.Copy(seg.Buffer, seg.Offset, buffer, 0, seg.Len);
                Log.L("CC Rec" + Encoding.UTF8.GetString(buffer));
                c.Send(seg);
            };
            bool con = c.Connect(8888, "127.0.0.1");
            byte[] buff = Encoding.UTF8.GetBytes("12323");
            if (con)
            {
                c.Send(new BufferSegment( buff, 0, buff.Length));
            }

        }
        private void Start()
        {
            packetTest();
        }

        private static void WebSocketDemo()
        {
            WSServerToken wsService = new WSServerToken();
            wsService.onAccept = new OnAccept((SocketToken sToken) => {
                Console.WriteLine("accepted:" + sToken.EndPoint);
            });
            wsService.onDisConnect = new OnDisConnect((SocketToken sToken) => {
                Console.WriteLine("disconnect:" + sToken.EndPoint.ToString());
            });
            wsService.onRecieveString = new OnReceivedString((SocketToken sToken, string content) => {

                Console.WriteLine("receive:" + content);
                wsService.Send(sToken, "hello websocket client! you said:" + content);

            });
            wsService.onReceieve = new OnReceieve((SocketToken token, BufferSegment session) => {
                Console.WriteLine("receive bytes:" + session.Len);
            });
            bool isOk = wsService.Start(65531);
            if (isOk)
            {
                Console.WriteLine("waiting for accept...");

                WSClientToken client = new WSClientToken();
                client.onConnect = new OnConnect((SocketToken sToken, bool isConnected) => {
                    Console.WriteLine("connected websocket server...");
                });
                client.onReceivedString = new OnReceivedString((SocketToken sToken, string msg) => {
                    Console.WriteLine(msg);
                });

                isOk = client.Connect("ws://127.0.0.1:65531");
                if (isOk)
                {
                    client.Send("hello websocket");
                }
            }
        }
    }
}
