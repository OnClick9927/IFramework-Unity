
using System.Collections.Generic;
using System;
namespace IFramework
{
    public interface IEventArgs { }
    public interface IEventEntity : IDisposable { }
    public interface IEventHandler<T> where T : IEventArgs
    {
        void OnEvent(T message);
    }

    abstract class EventEntityBase : IEventEntity
    {
        public string msg;
        public abstract void Invoke(IEventArgs args);
        public void Dispose() => Events.UnSubscribe(this);
        public abstract bool Equals(string msg, Action<IEventArgs> args);
    }

    class EventEntity : EventEntityBase
    {
        public Action<IEventArgs> action;

        public override bool Equals(string msg, Action<IEventArgs> args)
        {
            return this.msg == msg && action == args;
        }

        public override void Invoke(IEventArgs args) => action?.Invoke(args);
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
    }
    public class Events
    {
        private static Dictionary<string, List<EventEntityBase>> map = new Dictionary<string, List<EventEntityBase>>();
        private static List<EventEntityBase> GetList(string msg)
        {
            List<EventEntityBase> result = null;
            if (!map.TryGetValue(msg, out result))
            {
                result = new List<EventEntityBase>();
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
        internal static void UnSubscribe(EventEntityBase listen)
        {
            var list = FindList(listen.msg);
            if (list == null) return;
            list.RemoveAll(x => x == listen);

        }
        internal static IEventEntity Subscribe<T>(IEventHandler<T> handler) where T : IEventArgs
        {
            string msg = typeof(T).Name;
            var list = GetList(msg);

            var l = new EventEntity<T>()
            {
                msg = msg,
                action_T = handler.OnEvent
            };
            list.Add(l);
            return l;
        }
        internal static IEventEntity Subscribe(string msg, Action<IEventArgs> action)
        {
            var list = GetList(msg);

            var l = new EventEntity()
            {
                msg = msg,
                action = action
            };
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

    }
    public class EventBox : IDisposable
    {
        public List<IEventEntity> pairs = new List<IEventEntity>();
        public void Dispose()
        {
            for (int i = pairs.Count - 1; i >= 0; i--)
            {
                pairs[i].Dispose();
                pairs.RemoveAt(i);
            }
        }

        public IEventEntity Subscribe<T>(IEventHandler<T> handler) where T : IEventArgs
        {
            var entity = Events.Subscribe<T>(handler);
            pairs.Add(entity);
            return entity;
        }

        public IEventEntity Subscribe(string msg, Action<IEventArgs> action)
        {
            var entity = Events.Subscribe(msg, action);
            pairs.Add(entity);
            return entity;
        }
        public void UnSubscribe<T>(IEventHandler<T> handler) where T : IEventArgs
        {
            var find = pairs.Find(x =>
            {
                EventEntity<T> e = x as EventEntity<T>;
                return e != null && e.Equals(typeof(T).Name, handler.OnEvent);
            }
           );
            UnSubscribe(find);
        }
        public void UnSubscribe(string msg, Action<IEventArgs> action)
        {
            var find = pairs.Find(x =>
            {
                EventEntityBase e = x as EventEntityBase;
                return e != null && e.Equals(msg, action);
            }
           );
            UnSubscribe(find);
        }
        public void UnSubscribe(IEventEntity entity)
        {
            if (entity == null) return;
            entity.Dispose();
            pairs.Remove(entity);
        }
    }
}
