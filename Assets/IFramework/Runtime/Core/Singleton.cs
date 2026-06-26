
using System;
using System.Reflection;
using UnityEngine;

namespace IFramework
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private volatile static T instance;
        static object lockObj = new object();
        public static bool Exist => instance != null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                            instance.OnSingletonInit();
                        }
                    }
                }
                return instance;
            }
        }

        protected virtual void OnSingletonInit() { }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class DynamicMonoSingleton : Attribute
    {
        public bool DestroyOnLoad = true;
        internal string Name{ get; private set; }
        public DynamicMonoSingleton(string name = "")
        {
            Name = name;
        }

    }
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static bool Exist => instance != null;

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
            var attributes = type.GetCustomAttribute<DynamicMonoSingleton>(true);
            if (attributes == null) return null;
            var name = string.IsNullOrEmpty(attributes.Name) ? type.Name : attributes.Name;
            var instance = new GameObject(name).AddComponent<T>();
            if (attributes.DestroyOnLoad)
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
