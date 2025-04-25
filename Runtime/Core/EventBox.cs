
using System.Collections.Generic;
using System;
namespace IFramework
{
    public interface IEventsOwner { }
    public interface IEventArgs { }
    public interface IEventEntity : IDisposable { }
    public interface IEventHandler<T> where T : IEventArgs
    {
        void OnEvent(T message);
    }

    abstract class EventEntityBase : IEventEntity, IPoolObject
    {
        public IEventsOwner owner;
        public string msg;

        bool IPoolObject.valid { get; set; }

        public abstract void Invoke(IEventArgs args);
        public abstract void Dispose();
        public abstract bool Equals(string msg, Action<IEventArgs> args);

        protected virtual void Reset()
        {
            owner = null;
            msg = string.Empty;
        }


        void IPoolObject.OnGet() => Reset();
        void IPoolObject.OnSet() => Reset();
    }

    class EventEntity : EventEntityBase
    {
        public Action<IEventArgs> action;
        public override bool Equals(string msg, Action<IEventArgs> args)
        {
            return this.msg == msg && action == args;
        }

        public override void Invoke(IEventArgs args) => action?.Invoke(args);
        public override void Dispose() => Events.UnSubscribe(this);
        protected override void Reset()
        {
            action = null;
        }
    }

    class EventEntity<T> : EventEntityBase where T : IEventArgs
    {
        public Action<T> action_T;

        public override bool Equals(string msg, Action<IEventArgs> args)
        {
            return this.msg == msg && action_T.Equals(args);
        }
        public bool Equals(string msg, Action<T> args)
        {
            return this.msg == msg && action_T.Equals(args);
        }
        public override void Invoke(IEventArgs args)
        {
            action_T?.Invoke((T)args);
        }
        public override void Dispose() => Events.UnSubscribe<T>(this);
        protected override void Reset()
        {
            base.Reset();
            action_T = null;
        }
    }
    public static class Events
    {
        private static SimpleObjectPool<EventEntity> pool_0 = new SimpleObjectPool<EventEntity>();
        private static Dictionary<Type, ISimpleObjectPool> pools_1 = new Dictionary<Type, ISimpleObjectPool>();
        private static SimpleObjectPool<List<EventEntityBase>> pool_3 = new SimpleObjectPool<List<EventEntityBase>>();



        private static Dictionary<string, List<EventEntityBase>> map = new Dictionary<string, List<EventEntityBase>>();
        private static List<EventEntityBase> GetList(string msg)
        {
            List<EventEntityBase> result = null;
            if (!map.TryGetValue(msg, out result))
            {
                result = pool_3.Get();
                map.Add(msg, result);
            }
            return result;
        }
        private static List<EventEntityBase> FindList(string msg)
        {
            List<EventEntityBase> result = null;
            map.TryGetValue(msg, out result);

            return result;
        }



        private static void TryRecycleList(string key, List<EventEntityBase> list)
        {
            if (list.Count != 0) return;
            pool_3.Set(list);
            map.Remove(key);
        }

        internal static void UnSubscribe<T>(EventEntity<T> listen) where T : IEventArgs
        {
            var list = FindList(listen.msg);
            if (list == null) return;
            list.RemoveAll(x => x == listen);
            TryRecycleList(listen.msg, list);
            ISimpleObjectPool pool;
            if (!pools_1.TryGetValue(typeof(T), out pool)) return;
            var __pool = pool as SimpleObjectPool<EventEntity<T>>;
            __pool.Set(listen);
        }
        internal static void UnSubscribe(EventEntity listen)
        {
            var list = FindList(listen.msg);
            if (list == null) return;
            list.RemoveAll(x => x == listen);
            TryRecycleList(listen.msg, list);
            pool_0.Set(listen);
        }
        internal static IEventEntity Subscribe<T>(IEventHandler<T> handler) where T : IEventArgs
        {
            var type = typeof(T);
            string msg = type.Name;
            var list = GetList(msg);
            ISimpleObjectPool pool;
            if (!pools_1.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<EventEntity<T>>();
                pools_1.Add(type, pool);
            }
            var __pool = pool as SimpleObjectPool<EventEntity<T>>;
            var l = __pool.Get();
            l.msg = msg;
            l.action_T = handler.OnEvent;


            list.Add(l);
            return l;
        }
        internal static IEventEntity Subscribe(string msg, Action<IEventArgs> action)
        {
            var list = GetList(msg);
            var l = pool_0.Get();
            l.msg = msg;
            l.action = action;
            list.Add(l);
            return l;
        }








        public static void Publish(string message, IEventArgs args)
        {
            var list = FindList(message);
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Invoke(args);
            }
        }
        public static void Publish<T>(T args) where T : IEventArgs => Publish(typeof(T).Name, args);


        private static List<EventEntityBase> pairs = new List<EventEntityBase>();

        private static Dictionary<IEventsOwner, bool> help = new Dictionary<IEventsOwner, bool>();




        public static void DisposeEvents(this IEventsOwner self)
        {
            if (!help.ContainsKey(self)) return;
            for (int i = pairs.Count - 1; i >= 0; i--)
            {
                var e = pairs[i];
                if (e.owner == self)
                {
                    e.Dispose();
                    pairs.RemoveAt(i);
                }
            }
            help.Remove(self);
        }
        public static void DisposeEvent(this IEventsOwner self, IEventEntity listen)
        {
            if (!help.ContainsKey(self)) return;

            for (int i = pairs.Count - 1; i >= 0; i--)
            {
                var e = pairs[i];
                if (e == listen && e.owner == self)
                {
                    e.Dispose();
                    pairs.RemoveAt(i);
                    break;
                }
            }

        }
        public static IEventEntity SubscribeEvent(this IEventsOwner self, string msg, Action<IEventArgs> action)
        {
            EventEntityBase entity = Subscribe(msg, action) as EventEntityBase;
            entity.owner = self;
            if (!help.TryGetValue(entity.owner, out bool _))
                help.Add(entity.owner, true);

            pairs.Add(entity);
            return entity;
        }
        public static IEventEntity SubscribeEvent<T>(this IEventsOwner self, IEventHandler<T> handler) where T : IEventArgs
        {
            EventEntityBase entity = Subscribe<T>(handler) as EventEntityBase;
            entity.owner = self;
            if (!help.TryGetValue(entity.owner, out bool _))
                help.Add(entity.owner, true);

            pairs.Add(entity);
            return entity;
        }


    }
}
