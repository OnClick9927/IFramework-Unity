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
	public static class UInt64Extension
	{
        public static byte[] ToBytes(this UInt64 value)
        {
            return new byte[]{
                  (byte)(value >> 56),
                  (byte)(value >> 48),
                  (byte)(value >> 40),
                  (byte)(value >> 32),
                  (byte)(value >> 24),
                  (byte)(value >> 16),
                  (byte)(value >> 8),
                  (byte)value
             };
        }

    }
}
