/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-31
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
namespace IFramework
{
    public static class IntExtension
	{
        public static bool isPrimeNumber(this int self)
        {
            for (int i = 2; i < Math.Sqrt(self); i++)
            {
                if (self % i == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static int Swap(this int self,ref int other)
        {
            self = self ^ other;
            other = self ^ other;
            return self ^ other;
        }
    }
}
