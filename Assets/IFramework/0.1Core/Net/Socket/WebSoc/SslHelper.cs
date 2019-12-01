/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace IFramework.Net
{
    public class SslHelper
    {
        //private string certificate = string.Empty;
        X509Certificate serverCertificate = null;
        private int bufSize = 4096;

        public SslHelper(string certificateFile)
        {
            serverCertificate = X509Certificate.CreateFromCertFile(certificateFile);
        }

        public byte[] DeCryptFromServer(byte[] buffer, int offset, int size)
        {
            using (SslStream sslStream = new SslStream(new MemoryStream(buffer, offset, size), false))
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
                List<byte> msg = new List<byte>(bufSize);
                int b = -1;
                {
                    b = sslStream.ReadByte();
                    if (b != -1) msg.Add((byte)b);
                } while (b != -1) ;

                return msg.ToArray();
            }
        }

        public byte[] EnCryptFromServer()
        {
            return null;
        }

        public byte[] DeCryptFromClient()
        {
            return null;
        }

        public byte[] EnCryptFromClient()
        {
            return null;
        }
    }
}
