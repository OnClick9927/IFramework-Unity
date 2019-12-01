/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Net
{
    public class WSConnectionItem
    {
        public WSConnectionItem()
        { }

        public WSConnectionItem(string wsUrl)
        {
            string[] urlParams = wsUrl.Split(':');
            if (urlParams.Length < 3)
                throw new Exception("wsUrl is error format.for example as ws://localhost:80");

            Proto = urlParams[0];
            Domain = urlParams[1].Replace("//", "");
            Port = int.Parse(urlParams[2]);

            Host = Domain + ":" + Port;
        }
        private string proto = "ws";
        public string Proto { get { return proto; } set { proto = value; } }

        public string Domain { get; set; }
        private int port = 65531;

        public int Port { get { return port; } set { port = value; } }

        public string Host { get; private set; }
    }
}