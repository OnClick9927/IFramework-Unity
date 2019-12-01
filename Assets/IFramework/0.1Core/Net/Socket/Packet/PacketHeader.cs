/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Net
{
    public class PacketHeader
    {
        public UInt32 PackID { get; set; }
        public byte PackType { get; set; }
        public UInt16 PackCount { get; set; } /*= 1;*/
        public UInt32 MsgBuffLen { get; internal set; }
    }
}
