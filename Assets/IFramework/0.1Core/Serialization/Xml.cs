/*********************************************************************************
 *Author:       OnClick
 *Version:      1.0
 *UnityVersion: 2017.2.3p3
 *Date:         2019-01-26
 *Description:   
 *History:  
**********************************************************************************/
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace IFramework.Serialization
{
    public interface IXmlHelper
    {
        string ToXmlString<T>(T t);
        T ToObject<T>(string xmlString);
    }

    public class Xml
    {
        public static IXmlHelper helper { get; set; }
        static Xml()
        {
            helper =new DefaultXmlHelper();
        }

        public static string ToXmlString<T>(T t)
        {
            return helper.ToXmlString(t);
        }
        public static T ToObject<T>(string xmlString)
        {
            return helper.ToObject<T>(xmlString);
        }
    }
    public class DefaultXmlHelper : IXmlHelper
    {
        public T ToObject<T>(string xmlString)
        {
            using (TextReader reader = new StringReader(xmlString))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }

        }

        public string ToXmlString<T>(T t)
        {
            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                new XmlSerializer(typeof(T)).Serialize(writer, t);
            }
            return sb.ToString();
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    new XmlSerializer(typeof(T)).Serialize(ms, t);
            //    return Encoding.UTF8.GetString(ms.ToArray());
            //}
        }
    }

}