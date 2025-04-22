
using System;
using System.Reflection;
using UnityEngine;

namespace IFramework
{
    public abstract class Singleton<T>  where T : Singleton<T>, new()
    {
        private volatile static T _instance;
        static object lockObj = new object();

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            _instance.OnSingletonInit();
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void OnSingletonInit() { }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonPath : Attribute
    {
        public MonoSingletonPath(string pathInHierarchy)
        {
            PathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get;private set;
        }
    }
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateMonoSingleton();
                return instance;
            }
        }

        protected virtual void Awake()
        {
            SetInstance(this as T);
        }
        private void SetInstance(T value)
        {
            if (instance == null)
            {
                instance = value;
                value.OnSingletonInit();
            }
        }
        static T CreateMonoSingleton()
        {
            if (!Application.isPlaying) return default;
            System.Type type = typeof(T);
            var attributes = type.GetCustomAttribute<MonoSingletonPath>(true);
            GameObject obj = null;
            if (attributes == null || string.IsNullOrEmpty(attributes.PathInHierarchy))
                obj = new GameObject(type.Name);
            else
            {
                var path = attributes.PathInHierarchy;
                var subPath = path.Split('/');
                for (int i = 0; i < subPath.Length; i++)
                {
                    GameObject client = null;
                    if (obj == null)
                        client = GameObject.Find(subPath[i]);
                    else
                    {
                        var child = obj.transform.Find(subPath[i]);
                        if (child != null)
                            client = child.gameObject;
                    }
                    if (client == null)
                        client = new GameObject(subPath[i]);
                    if (obj != null)
                        client.transform.SetParent(obj.transform);
                    if (i == 0)
                        GameObject.DontDestroyOnLoad(client);
                    obj = client;
                }
            }
            var instance = obj.AddComponent<T>();
            if (instance.transform.parent == null)
                DontDestroyOnLoad(instance.gameObject);
            return instance;
        }

        protected virtual void OnSingletonInit() { }

        protected virtual void OnDestroy()
        {
            instance = null;

        }

    }

}
