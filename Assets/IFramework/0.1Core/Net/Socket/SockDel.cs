/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Net
{
    public delegate void OnConnect(SocketToken token, bool connected);

    public delegate void OnReceivedString(SocketToken sToken, string content);
    public delegate void OnReceieve(SocketToken token,BufferSegment seg);
    public delegate void OnSendCallBack(SocketToken token, BufferSegment seg);

    public delegate void OnAccept(SocketToken token);
    public delegate void OnDisConnect(SocketToken token);

}
