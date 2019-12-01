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
	public static class ByteExtension
	{
        public static UInt16 ToUInt16(this byte[] array, int offset = 0)
        {
            return (UInt16)((array[offset] << 8) | array[offset + 1]);
        }
        public static UInt32 ToUInt32(this byte[] array, int offset = 0)
        {
            return (((UInt32)array[offset] << 24)
               | ((UInt32)array[offset + 1] << 16)
               | ((UInt32)array[offset + 2] << 8)
               | array[offset + 3]);
        }
        public static UInt64 ToUInt64(this byte[] array, int offset = 0)
        {
            return (((UInt64)array[offset] << 56)
                 | ((UInt64)array[offset + 1] << 48)
                 | ((UInt64)array[offset + 2] << 40)
                 | ((UInt64)array[offset + 3] << 32)
                 | ((UInt64)array[offset + 4] << 24)
                 | ((UInt64)array[offset + 5] << 16)
                 | ((UInt64)array[offset + 6] << 8)
                 | array[offset + 7]);
        }
    }
}
