﻿
using System;
using System.Reflection;
using UnityEngine;

namespace IFramework
{

    interface ISingleton 
    {

        void OnSingletonInit();
    }
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
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


        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonPath : Attribute
    {
        private string mPathInHierarchy;

        public MonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        protected static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateMonoSingleton();
                return instance;
            }
        }
        static T CreateMonoSingleton()
        {
            T instance = null;
            if (!Application.isPlaying) return instance;
            instance = UnityEngine.Object.FindObjectOfType<T>();
            if (instance != null) return instance;
            System.Type info = typeof(T);
            var attributes = info.GetCustomAttribute<MonoSingletonPath>(true);
            GameObject obj = null;
            if (attributes == null || string.IsNullOrEmpty(attributes.PathInHierarchy))
                obj = new GameObject(typeof(T).Name);
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
            instance = obj.AddComponent<T>();
            if (instance.transform.parent == null)
                UnityEngine.Object.DontDestroyOnLoad(instance.gameObject);
            instance.OnSingletonInit();
            return instance;
        }

        protected virtual void OnSingletonInit() { }

        protected virtual void OnDestroy()
        {
            instance = null;

        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

}
