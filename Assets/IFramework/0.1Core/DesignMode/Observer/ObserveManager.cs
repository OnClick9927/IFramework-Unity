/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    public interface IObserver
    {
        void Listen(IPublisher  publisher, Type eventType, int code, IEventArgs args, params object[] param);
    }
    public interface IPublisher
    {

    }
    public class ObserveManager :SingletonPropertyClass<ObserveManager>
    {
        private interface IObserve : IDisposable
        {
            Type ObserveType { get; }
            bool Subscribe(IObserver observer);
            bool Unsubscribe(IObserver observer);
            void Publish(IPublisher  publisher, int code, IEventArgs args, params object[] param);
        }
        private class Observe : IObserve
        {
            private LockParam para = new LockParam();
            private readonly Type observeType;
            public Type ObserveType { get { return observeType; } }

            public Observe(Type observeType) { this.observeType = observeType; }
            private List<IObserver> observers = new List<IObserver>();
            public bool Subscribe(IObserver observer)
            {
                using (new LockWait(ref para))
                {
                    if (observers.Contains(observer)) return false;
                    else observers.Add(observer); return true;
                }
            }
            public bool Unsubscribe(IObserver observer)
            {
                using (new LockWait(ref para))
                {
                    if (!observers.Contains(observer)) return false;
                    else observers.Remove(observer); return true;
                }
            }

            public void Publish(IPublisher  publisher, int code, IEventArgs args, params object[] param)
            {
                using (new LockWait(ref para))
                {
                    observers.ForEach((observer) => {
                        if (observer != null) observer.Listen(publisher, observeType, code, args, param);
                        else observers.Remove(observer);
                    });
                }
            }

            public void Dispose()
            {
                using (new LockWait(ref para))
                {
                    observers.Clear();
                    observers = null;
                }
            }
        }


        private ObserveManager() { }

        private Dictionary<Type, Observe> observes;
        protected override void OnSingletonInit()
        {
            observes = new Dictionary<Type, Observe>();
        }

        public override void Dispose()
        {
            base.Dispose();
            var temp = observes.Values.GetEnumerator();
            while (temp.MoveNext())
            {
                temp.Current.Dispose();
            }
            temp.Dispose();
            observes.Clear();
            observes = null;
        }

        public static void Publish<T>(int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            if (!Instance.observes.ContainsKey(typeof(T)))
                Log.E("Non Such Type :  " + typeof(T));
            else
                Instance.observes[typeof(T)].Publish(null, code, args, param);
        }
        public static void Publish<T>(T t, int code, IEventArgs args, params object[] param)where T:IPublisher
        {
            if (!Instance.observes.ContainsKey(typeof(T)))
                Log.E("Non Such Type :  " + typeof(T));
            else
                Instance.observes[typeof(T)].Publish(t, code, args, param);
        }



        public static bool Subscribe(Type type, IObserver observer)
        {
            if (!Instance.observes.ContainsKey(type))
                Instance.observes.Add(type, new Observe(type));
            return Instance.observes[type].Subscribe(observer);
        }
        public static bool Subscribe<T>(IObserver observer) where T : IPublisher
        {
            return Subscribe(typeof(T), observer);
        }


        public static bool Unsubscribe(Type type, IObserver observer)
        {
            if (!Instance.observes.ContainsKey(type)) return false;
            return Instance.observes[type].Unsubscribe(observer);
        }
        public static bool Unsubscribe<T>(IObserver observer) where T : IPublisher
        {
            return Unsubscribe(typeof(T), observer);
        }
    }
    
}