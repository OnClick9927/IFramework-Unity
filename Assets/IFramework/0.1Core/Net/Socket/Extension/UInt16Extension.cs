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
	public static class UInt16Extension
	{
        public static byte[] ToBytes(this UInt16 value)
        {
            return new byte[]{
                     (byte)(value >> 8),
                     (byte)value
                };
        }
    }
}
