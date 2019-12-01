/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IFramework
{
    /// 校验相关的实用函数。
    public class Verifier
    {
        private static readonly byte[] Zero = new byte[] { 0, 0, 0, 0 };

        /// 计算二进制流的 CRC32。
        public static byte[] GetCrc32(byte[] bytes)
        {
            if (bytes == null)
            {
                return Zero;
            }
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                Crc32 calculator = new Crc32();
                byte[] result = calculator.ComputeHash(memoryStream);
                calculator.Clear();
                return result;
            }
        }

        /// 计算指定文件的 CRC32。
        public static byte[] GetCrc32(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return Zero;
            }
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Crc32 calculator = new Crc32();
                byte[] result = calculator.ComputeHash(fileStream);
                calculator.Clear();
                return result;
            }
        }

        /// 计算二进制流的 MD5。
        public static string GetMD5(byte[] bytes)
        {
            MD5 alg = new MD5CryptoServiceProvider();
            byte[] data = alg.ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
        public static string GetFileMD5(string absPath)
        {
            if (!File.Exists(absPath))
                return "";

            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            FileStream file = new FileStream(absPath, FileMode.Open);
            byte[] retVal = md5CSP.ComputeHash(file);
            file.Close();
            string result = "";

            for (int i = 0; i < retVal.Length; i++)
            {
                result += retVal[i].ToString("x2");
            }

            return result;
        }
        public static string GetStrMD5(string str)
        {
            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            byte[] retVal = md5CSP.ComputeHash(Encoding.Default.GetBytes(str));
            string retStr = "";

            for (int i = 0; i < retVal.Length; i++)
            {
                retStr += retVal[i].ToString("x2");
            }

            return retStr;
        }



        /// <summary>
        /// CRC32 算法。
        /// </summary>
        private sealed class Crc32 : HashAlgorithm
        {
            /// 默认多项式。
            public const uint DefaultPolynomial = 0xedb88320;

            /// 默认种子数。
            public const uint DefaultSeed = 0xffffffff;

            private static uint[] s_DefaultTable = null;
            private readonly uint m_Seed;
            private readonly uint[] m_Table;
            private uint m_Hash;
            /// 初始化 CRC32 类的新实例。
            public Crc32()
            {
                m_Seed = DefaultSeed;
                m_Table = InitializeTable(DefaultPolynomial);
                m_Hash = DefaultSeed;
            }

            /// 初始化 CRC32 类的新实例。
            /// <param name="polynomial">指定的多项式。</param>
            /// <param name="seed">指定的种子数。</param>
            public Crc32(uint polynomial, uint seed)
            {
                m_Seed = seed;
                m_Table = InitializeTable(polynomial);
                m_Hash = seed;
            }
            private static uint[] InitializeTable(uint polynomial)
            {
                if (s_DefaultTable != null && polynomial == DefaultPolynomial)
                {
                    return s_DefaultTable;
                }
                uint[] createTable = new uint[256];
                for (int i = 0; i < 256; i++)
                {
                    uint entry = (uint)i;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((entry & 1) == 1)
                        {
                            entry = (entry >> 1) ^ polynomial;
                        }
                        else
                        {
                            entry = entry >> 1;
                        }
                    }

                    createTable[i] = entry;
                }
                if (polynomial == DefaultPolynomial)
                {
                    s_DefaultTable = createTable;
                }
                return createTable;
            }
            private static uint CalculateHash(uint[] table, uint seed, byte[] bytes, int start, int size)
            {
                uint crc = seed;
                for (int i = start; i < size; i++)
                {
                    unchecked
                    {
                        crc = (crc >> 8) ^ table[bytes[i] ^ crc & 0xff];
                    }
                }

                return crc;
            }
            private static byte[] UInt32ToBigEndianBytes(uint x)
            {
                return new byte[] { (byte)((x >> 24) & 0xff), (byte)((x >> 16) & 0xff), (byte)((x >> 8) & 0xff), (byte)(x & 0xff) };
            }

            /// 初始化 Crc32 类的实现。
            public override void Initialize()
            {
                m_Hash = m_Seed;
            }
            /// 将写入对象的数据路由到哈希算法以计算哈希值。
            /// <param name="array">要计算其哈希代码的输入。</param>
            /// <param name="ibStart">字节数组中的偏移量，从该位置开始使用数据。</param>
            /// <param name="cbSize">字节数组中用作数据的字节数。</param>
            protected override void HashCore(byte[] array, int ibStart, int cbSize)
            {
                m_Hash = CalculateHash(m_Table, m_Hash, array, ibStart, cbSize);
            }
            /// 在加密流对象处理完最后的数据后完成哈希计算。
            protected override byte[] HashFinal()
            {
                byte[] hashBuffer = UInt32ToBigEndianBytes(~m_Hash);
                HashValue = hashBuffer;
                return hashBuffer;
            }
        }
    }
  
}
