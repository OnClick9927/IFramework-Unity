/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace IFramework
{
    public static partial class ObjectExtension
    {
        public static bool ContainsInterface(this object obj, Type type)
        {
            var objType = obj.GetType();
            return !objType.IsInterface && objType.GetInterfaces().Contains(type);
        }
        public static bool ContainsInterface(this object obj, string name)
        {
            var type = obj.GetType();
            return !type.IsInterface && type.GetInterface(name) != null;
        }
        //0.9281 
        public static T ReflectionDeepCopy<T>(this T obj)
        {
            if (obj is string || obj.GetType().IsValueType) return obj;
            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, ReflectionDeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
        public static object ReflectionDeepCopy(this object obj)
        {
            if (obj is string || obj.GetType().IsValueType) return obj;
            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, ReflectionDeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return retval;
        }
        //1.6543 public
        public static T XmlDeepCopy<T>(this T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        //1.7278 [Serializable]
        public static T BinaryDeepCopy<T>(this T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        //1.1024 [DataContract]
        public static T DataContractDeepCopy<T>(this T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = ser.ReadObject(ms);
                ms.Close();
            }
            return (T)retval;
        }
        public static bool IsNull<T>(this T selfObj) where T : class
        {
            return null == selfObj;
        }

        /// 获取对象是否是可空类型。
        public static bool IsNullable<T>(this T t) { return false; }
        public static bool IsNullable<T>(this T? t) where T : struct { return true; }


        public static T ReflectProperty<T>(this object obj, string name)
        {
            if (obj == null) return default(T);

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var p = obj.GetType().GetProperty(name, flags);
            if (p == null) return default(T);
            return (T)p.GetValue(obj, null);
        }

        public static T ReflectField<T>(this object obj, string name)
        {
            if (obj == null) return default(T);

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var f = obj.GetType().GetField(name, flags);
            if (f == null) return default(T);
            return (T)f.GetValue(obj);
        }

        public static MethodInfo ReflectMethod(this object obj, string name)
        {
            if (obj == null) return null;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var m = obj.GetType().GetMethod(name, flags);
            return m;
        }

        public static void ReflectInvokeMethod(this object obj, string name, params object[] parameters)
        {
            if (obj == null) return;

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var m = obj.GetType().GetMethod(name, flags);
            if (m == null) return;
            m.Invoke(obj, parameters);
        }

        public static T ReflectInvokeMethod<T>(this object obj, string name, params object[] parameters)
        {
            if (obj == null) return default(T);

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var m = obj.GetType().GetMethod(name, flags);
            if (m == null) return default(T);
            return (T)m.Invoke(obj, parameters);
        }

    }
}