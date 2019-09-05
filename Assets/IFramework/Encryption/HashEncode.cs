using System.Security.Cryptography;
using System.Text;
namespace IFramework.Encryption
{
    public class HashEncode
    {

        /// 得到随机哈希加密字符串
        public static string GetSecurity()
        {
            return HashEncoding(GetRandomValue());
        }
        /// 得到一个随机数值
        public static string GetRandomValue()
        {
            System.Random Seed = new System.Random();
            return Seed.Next(1, int.MaxValue).ToString();
        }
        /// 哈希加密一个字符串
        public static string HashEncoding(string Security)
        {
            byte[] Value;
            UnicodeEncoding Code = new UnicodeEncoding();
            byte[] Message = Code.GetBytes(Security);
            SHA512Managed Arithmetic = new SHA512Managed();
            Value = Arithmetic.ComputeHash(Message);
            Security = "";
            foreach (byte o in Value)
            {
                Security += (int)o + "O";
            }
            return Security;
        }
    }
}