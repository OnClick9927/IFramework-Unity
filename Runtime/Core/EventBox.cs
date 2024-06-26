
using System.Collections.Generic;
using System;
using static IFramework.Events;
namespace IFramework
{
    public interface IEventArgs
    {
    }
    public class Events
    {
        public class EventEntity : IDisposable
        {
            public Action<IEventArgs> action;
            public string msg;
            public void Dispose()
            {
                UnSubscribe(this);
            }
        }
        private static Dictionary<string, List<EventEntity>> map = new Dictionary<string, List<EventEntity>>();

        private static void UnSubscribe(EventEntity listen)
        {
            if (map.ContainsKey(listen.msg))
            {
                map[listen.msg].RemoveAll(x => x == listen);
            }
        }
        public static EventEntity Subscribe(string msg, Action<IEventArgs> action)
        {
            if (!map.ContainsKey(msg))
                map.Add(msg, new List<EventEntity>());

            var l = new EventEntity()
            {
                msg = msg,
                action = action
            };
            map[msg].Add(l);
            return l;
        }
        public static void Publish(string message, IEventArgs args)
        {
            if (!map.ContainsKey(message)) return;
            var list = map[message];
            for (int i = 0; i < list.Count; i++)
            {
                (list[i] as EventEntity).action.Invoke(args);
            }

        }
    }
    public class EventBox : IDisposable
    {
        public List<Events.EventEntity> pairs = new List<Events.EventEntity>();
        public void Dispose()
        {
            for (int i = pairs.Count - 1; i >= 0; i--)
            {
                pairs[i].Dispose();
                pairs.RemoveAt(i);
            }
        }

        public EventEntity Subscribe(string msg, Action<IEventArgs> action)
        {
            var entity = Events.Subscribe(msg, action);
            pairs.Add(entity);
            return entity;
        }

        public void UnSubscribe(string msg, Action<IEventArgs> action)
        {
            var find = pairs.Find(x => x.msg == msg && x.action == action);
            UnSubscribe(find);
        }
        public void UnSubscribe(EventEntity entity)
        {
            if (entity == null) return;
            entity.Dispose();
            pairs.Remove(entity);
        }


    }
}
