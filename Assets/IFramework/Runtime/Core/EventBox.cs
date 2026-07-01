
using System;
using System.Collections.Generic;

namespace IFramework
{
    //public interface IEventsOwner { }
    public interface IEventArgs { }
    public interface IEventHandler { }
    public interface IEventHandler<T> : IEventHandler where T : IEventArgs
    {
        void OnEvent(T message);
    }
    public interface IAsyncEventHandler<T> : IEventHandler where T : IEventArgs
    {
        AsyncTask OnEvent(T message);
    }
    abstract class EventEntityBase : IDisposable, IPoolObject
    {
        //public IEventsOwner owner;
        public string msg { get; protected set; }
        public bool valid { get; set; }

        public abstract AsyncTask Call(IEventArgs args);

        public abstract void Dispose();

        protected virtual void Reset()
        {
            //owner = null;
            msg = string.Empty;
        }


        void IPoolObject.OnGet() => Reset();
        void IPoolObject.OnSet() => Reset();
    }


    class EventHandlerEntity<T> : EventEntityBase where T : IEventArgs
    {
        bool async;
        private IEventHandler action;
        public override void Dispose() => Events.UnSubscribe(this);

        public override AsyncTask Call(IEventArgs args)
        {
            if (async)
                return (action as IAsyncEventHandler<T>)?.OnEvent((T)args);
            (action as IEventHandler<T>)?.OnEvent((T)args);
            return AsyncTask.CompletedTask;
        }

        protected override void Reset()
        {
            action = null;
        }
        public EventEntityBase SetData(bool async, string message, IEventHandler action)
        {
            this.async = async;
            this.action = action;
            this.msg = message;
            return this;
        }
    }
    class DelegateEventEntity<T> : EventEntityBase where T : Delegate
    {
        bool async;

        private T action;
        protected sealed override void Reset() => action = null;

        public EventEntityBase SetData(bool async, string message, T action)
        {
            this.async = async;
            this.action = action;
            this.msg = message;
            return this;
        }
        public override AsyncTask Call(IEventArgs args)
        {
            if (async)
                return (action as Func<IEventArgs, AsyncTask>)?.Invoke(args);
            (action as Action<IEventArgs>)?.Invoke(args);
            return AsyncTask.CompletedTask;
        }
        public override void Dispose() => Events.UnSubscribe(this);
    }


    public static class Events
    {
        private static Dictionary<string, MessageContext> map = new Dictionary<string, MessageContext>();
        class MessageContext : IPoolObject
        {
            public string message;
            private List<EventEntityBase> entities = new List<EventEntityBase>();
            bool IPoolObject.valid { get; set; }

            void IPoolObject.OnGet()
            {
                entities.Clear();
                message = string.Empty;
            }

            void IPoolObject.OnSet()
            {
            }
            public int Count => entities.Count;
            public void UnSubscribe(EventEntityBase listen)
            {
                entities.RemoveAll(x => x == listen);
            }

            public IDisposable Subscribe(EventEntityBase listen)
            {
                if (entities.Contains(listen)) return listen;
                entities.Add(listen);
                return listen;
            }

            public void Publish(IEventArgs args)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    entities[i].Call(args);
                }
            }

            public AsyncTask PublishAsync(IEventArgs args)
            {
                var array = StaticPool.GetArray<AsyncTask>(entities.Count);
                bool whenAll = false;
                for (int i = 0; i < entities.Count; i++)
                {
                    var task = entities[i].Call(args);
                    array[i] = task;
                    if (!whenAll && !task.IsCompleted)
                        whenAll = true;

                }
                if (whenAll)
                    return AsyncTask.WhenAll(array).ContinueWith(_ =>
                           {
                               StaticPool.Set<AsyncTask>(array);
                           });
                else
                {
                    StaticPool.Set<AsyncTask>(array);
                    return AsyncTask.CompletedTask;
                }
            }




        }





        private static MessageContext GetContext(string msg)
        {
            MessageContext result = null;
            if (!map.TryGetValue(msg, out result))
            {
                result = StaticPool.Get<MessageContext>();
                result.message = msg;
                map.Add(msg, result);
            }
            return result;
        }
        private static MessageContext FindContext(string msg, bool err)
        {
            map.TryGetValue(msg, out var result);
            if (err && result == null) Log.E($"Msg:{msg} None Handler");
            return result;
        }

        private static void TryRecycleList(string key, MessageContext list)
        {
            if (list.Count != 0) return;
            StaticPool.Set(list);
            map.Remove(key);
        }


        internal static void UnSubscribe<T>(T listen) where T : EventEntityBase, new()
        {
            var list = FindContext(listen.msg, false);
            if (list == null) return;
            list.UnSubscribe(listen);
            TryRecycleList(listen.msg, list);
            StaticPool.Set<T>(listen);

        }



        public static IDisposable Subscribe<T>(IEventHandler handler) where T : IEventArgs
        {
            var type = typeof(T);
            string msg = type.Name;
            return GetContext(msg).Subscribe(StaticPool.Get<EventHandlerEntity<T>>().SetData(handler is IAsyncEventHandler<T>, msg, handler));
        }
        public static IDisposable Subscribe<T>(string msg, Func<T, AsyncTask> action) where T : IEventArgs
            => GetContext(msg).Subscribe(StaticPool.Get<DelegateEventEntity<Func<T, AsyncTask>>>().SetData(true, msg, action));
        public static IDisposable Subscribe(string msg, Action<IEventArgs> action)
            => GetContext(msg).Subscribe(StaticPool.Get<DelegateEventEntity<Action<IEventArgs>>>().SetData(false, msg, action));


        public static AsyncTask PublishAsync(string message, IEventArgs args) => FindContext(message, false)?.PublishAsync(args);
        public static AsyncTask PublishAsync<T>(T args) where T : IEventArgs => PublishAsync(typeof(T).Name, args);
        public static void Publish(string message, IEventArgs args) => FindContext(message, false)?.Publish(args);
        public static void Publish<T>(T args) where T : IEventArgs => Publish(typeof(T).Name, args);







        public static T SubscribeEvent<T>(this T self, string msg, Action<IEventArgs> action)
        {
            Subscribe(msg, action).AddTo(self);
            return self;
        }

        public static T SubscribeEvent<T>(this T self, string msg, Func<IEventArgs, AsyncTask> action)
        {
            Subscribe(msg, action).AddTo(self);
            return self;
        }

        public static object SubscribeEvent<T>(this object self, IEventHandler handler) where T : IEventArgs
        {
            Subscribe<T>(handler).AddTo(self);
            return self;
        }

        private static Dictionary<string, AsyncTask> wait_map = new Dictionary<string, AsyncTask>();


        public static AsyncTask Wait(string message, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            if (wait_map.TryGetValue(message, out var result))
            {
                Log.E($"Already Exist Wait:{message}");
                return null;
            }
            var task = AsyncTask.CreateFromPool();
            token.Register(task);
            task.ContinueWith(task =>
            {
                wait_map.Remove(message);
            });
            wait_map[message] = task;
            return task;
        }
        public static AsyncTask<T> Wait<T>(string message, CancellationToken token = default) where T : IEventArgs
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            if (wait_map.TryGetValue(message, out var result))
            {
                Log.E($"Already Exist Wait:{message}");
                return null;
            }
            var task = AsyncTask<T>.CreateFromPool();
            token.Register(task);

            task.ContinueWith(task =>
            {
                wait_map.Remove(message);
            });
            wait_map[message] = task;
            return task;
        }
        public static AsyncTask<T> Wait<T>(CancellationToken token = default) where T : IEventArgs => Wait<T>(typeof(T).Name, token);

        public static void Notify(string message)
        {
            if (!wait_map.Remove(message, out var task))
                Log.E($"Notify:{message} Not Exist Wait");
            else
                task.SetResult();
        }
        public static void Notify<T>(string message, T arg) where T : IEventArgs
        {
            if (!wait_map.Remove(message, out var task))
                Log.E($"Notify:{message} Not Exist Wait");
            else
            {
                if (task is AsyncTask<T> task_T)
                    task_T.SetResult(arg);
                else
                    Log.E($"Notify:{message} Not Fit Wait {task.GetType()}");
            }
        }
        public static void Notify<T>(T arg) where T : IEventArgs => Notify(typeof(T).Name, arg);

    }


}
