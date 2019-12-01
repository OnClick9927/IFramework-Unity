/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Net
{
    public class AccessInfo : BaseInfo
    {
        /// <summary>
        /// 连接主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 连接源
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 安全扩展
        /// </summary>
        public string SecWebSocketExtensions { get; set; }
        /// <summary>
        /// 安全密钥
        /// </summary>
        public string SecWebSocketKey { get; set; }
        /// <summary>
        /// 安全版本
        /// </summary>
        public string SecWebSocketVersion { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(HttpProto))
                HttpProto = "GET / HTTP/1.1";

            if (string.IsNullOrEmpty(SecWebSocketVersion))
                SecWebSocketVersion = "13";

            return string.Format("{0}{1}{2}{3}{4}{5}{6}",
                HttpProto + NewLine,
                "Host" + SplitChars + Host + NewLine,
                "Connection" + SplitChars + Connection + NewLine,
                "Upgrade" + SplitChars + Upgrade + NewLine,
                "Origin" + SplitChars + Origin + NewLine,
                "Sec-WebSocket-Version" + SplitChars + SecWebSocketVersion + NewLine,
                "Sec-WebSocket-Key" + SplitChars + SecWebSocketKey + NewLine + NewLine);
        }

        public bool IsHandShaked()
        {
            return string.IsNullOrEmpty(SecWebSocketKey) == false;
        }
    }

    public class BaseInfo
    {
        /// <summary>
        /// http连接协议
        /// </summary>
        public string HttpProto { get; set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 连接方式
        /// </summary>
        public string Upgrade { get; set; }

        internal const string NewLine = "\r\n";
        internal const string SplitChars = ": ";


        public BaseInfo()
        {
            Upgrade = "websocket";
            Connection = "Upgrade";
        }
    }
    public class AcceptInfo : BaseInfo
    {
        /// <summary>
        /// 接入访问验证码
        /// </summary>
        public string SecWebSocketAccept { get; set; }
        /// <summary>
        /// 客户端来源
        /// </summary>
        public string SecWebSocketLocation { get; set; }
        /// <summary>
        /// 服务端来源
        /// </summary>
        public string SecWebSocketOrigin { get; set; }


        public override string ToString()
        {
            if (string.IsNullOrEmpty(HttpProto))
                HttpProto = "HTTP/1.1 101 Switching Protocols";

            return string.Format("{0}{1}{2}{3}",
                HttpProto + NewLine,
                "Connection" + SplitChars + Connection + NewLine,
                "Upgrade" + SplitChars + Upgrade + NewLine,
                 "Sec-WebSocket-Accept" + SplitChars + SecWebSocketAccept + NewLine + NewLine//很重要，需要两个newline
                );
        }

        public bool IsHandShaked()
        {
            return string.IsNullOrEmpty(SecWebSocketAccept) == false;
        }
    }
}
