
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace IFramework
{
    public interface IEventsOwner { }
    public interface IEventArgs { }
    public interface IEventEntity : IDisposable
    {
        bool SetAsInvoke();
    }
    public interface IEventHandler { }
    public interface IEventHandler<T> : IEventHandler where T : IEventArgs
    {
        void OnEvent(T message);
    }
    public interface IAsyncEventHandler<T> : IEventHandler where T : IEventArgs
    {
        AsyncTask OnEvent(T message);
    }

    abstract class EventEntityBase : IEventEntity, IPoolObject
    {
        public IEventsOwner owner;
        public string msg { get; protected set; }
        public bool valid { get; set; }

        public abstract AsyncTask Call(IEventArgs args);
        public abstract void Dispose();

        protected virtual void Reset()
        {
            owner = null;
            msg = string.Empty;
        }


        void IPoolObject.OnGet() => Reset();
        void IPoolObject.OnSet() => Reset();

        bool IEventEntity.SetAsInvoke()
        {
            if (!valid) return false;
            return Events.SetAsInvoke(this);
        }
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
            public EventEntityBase invoke { get; private set; }
            private List<EventEntityBase> entities = new List<EventEntityBase>();
            bool IPoolObject.valid { get; set; }

            void IPoolObject.OnGet()
            {
                entities.Clear();
                message = string.Empty;
                invoke = null;
            }

            void IPoolObject.OnSet()
            {
            }
            public int Count => entities.Count;
            public void UnSubscribe(EventEntityBase listen)
            {
                entities.RemoveAll(x => x == listen);
                if (invoke == listen)
                    invoke = null;
            }

            public IEventEntity Subscribe(EventEntityBase listen)
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



            public AsyncTask<T> InvokeAsync<T>(IEventArgs args)
            {
                if (invoke == null)
                {
                    Log.E($"Msg:{message}-{typeof(T)} None Invoke Handler");
                    return AsyncTask<T>.CompletedTaskT;
                }
                var task = invoke.Call(args) as AsyncTask<T>;
                if (task != null)
                {
                    if (!task.IsCompleted)
                        return task.ContinueWith<AsyncTask<T>>(_ => { CallOthersSync(args); });
                    else
                        Log.E($"Msg:{message}-{typeof(T)} Call Invoke Please,The Handler is Sync ");
                }
                var type = invoke.GetType();
                if (type.IsGenericType)
                {
                    var realT = type.GetGenericArguments().First();
                    Log.L($"Msg:{message}-{typeof(T)} Not Fit Invoke Handler Type {realT} ");
                }
                else
                    Log.L($"Msg:{message}-{typeof(T)} Not Fit Invoke Handler Type Void");
                return AsyncTask<T>.CompletedTaskT;
            }
            public AsyncTask InvokeAsync(IEventArgs args)
            {
                if (invoke == null)
                {
                    Log.E($"Msg:{message} None Invoke Handler");
                    return AsyncTask.CompletedTask;
                }
                var task = invoke.Call(args);

                if (task != null)
                    if (!task.IsCompleted)
                        return task.ContinueWith(_ => { CallOthersSync(args); });
                    else
                        Log.E($"Msg:{message} Call Invoke Please,The Handler is Sync ");

                return AsyncTask.CompletedTask;
            }


            public T Invoke<T>(IEventArgs args)
            {

                if (invoke == null)
                {
                    Log.E($"Msg:{message}-{typeof(T)} None Invoke Handler");
                    return default;
                }
                var task = invoke.Call(args) as AsyncTask<T>;
                if (task != null)
                {
                    if (!task.IsCompleted)
                    {
                        Log.E($"Msg:{message}-{typeof(T)} Use InvokeAsync ");
                        return default;
                    }
                    else
                    {
                        CallOthersSync(args);
                        return task.result;
                    }
                }
                var type = invoke.GetType();
                if (type.IsGenericType)
                {
                    var realT = type.GetGenericArguments().First();
                    Log.L($"Msg:{message}-{typeof(T)} Not Fit Invoke Handler Type {realT} ");
                }
                else
                    Log.L($"Msg:{message}-{typeof(T)} Not Fit Invoke Handler Type Void");
                return default;
            }
            public void Invoke(IEventArgs args)
            {
                if (invoke == null)
                {
                    Log.E($"Msg:{message} None Invoke Handler");
                    return;
                }
                var task = invoke.Call(args);
                if (!task.IsCompleted)
                    Log.E($"Msg:{message}- Use InvokeAsync ");
                else
                    CallOthersSync(args);
            }
            private void CallOthersSync(IEventArgs args)
            {
                if (entities.Count > 1)
                {
                    for (int i = 0; i < entities.Count; i++)
                    {
                        var entity = entities[i];
                        if (entity != invoke)
                            entity.Call(args);

                    }
                }
            }



            public bool SetAsInvoke(EventEntityBase entity)
            {
                if (invoke != null && invoke != entity) return false;
                invoke = entity;
                return true;
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



        internal static IEventEntity Subscribe<T>(IEventHandler handler) where T : IEventArgs
        {
            var type = typeof(T);
            string msg = type.Name;
            return GetContext(msg).Subscribe(StaticPool.Get<EventHandlerEntity<T>>().SetData(handler is IAsyncEventHandler<T>, msg, handler));
        }


        internal static IEventEntity Subscribe<T>(string msg, Func<T, AsyncTask> action) where T : IEventArgs
            => GetContext(msg).Subscribe(StaticPool.Get<DelegateEventEntity<Func<T, AsyncTask>>>().SetData(true, msg, action));
        internal static IEventEntity Subscribe(string msg, Action<IEventArgs> action)
            => GetContext(msg).Subscribe(StaticPool.Get<DelegateEventEntity<Action<IEventArgs>>>().SetData(false, msg, action));






        public static AsyncTask InvokeAsync(string message, IEventArgs args) => FindContext(message, true)?.InvokeAsync(args);

        public static void Invoke(string message, IEventArgs args) => FindContext(message, true)?.Invoke(args);
        public static T Invoke<T>(string message, IEventArgs args)
        {
            var find = FindContext(message, true);
            if (find != null) return find.Invoke<T>(args);
            return default;
        }

        public static AsyncTask<T> InvokeAsync<T>(string message, IEventArgs args) => FindContext(message, true)?.InvokeAsync<T>(args);

        public static T Invoke<T, Arg>(Arg args) where Arg : IEventArgs => Invoke<T>(typeof(Arg).Name, args);
        public static AsyncTask<T> InvokeAsync<T, Arg>(Arg args) where Arg : IEventArgs => InvokeAsync<T>(typeof(Arg).Name, args);



        public static AsyncTask PublishAsync(string message, IEventArgs args) => FindContext(message, false)?.PublishAsync(args);
        public static AsyncTask PublishAsync<T>(T args) where T : IEventArgs => PublishAsync(typeof(T).Name, args);
        public static void Publish(string message, IEventArgs args) => FindContext(message, false)?.Publish(args);
        public static void Publish<T>(T args) where T : IEventArgs => Publish(typeof(T).Name, args);




        private static Dictionary<IEventsOwner, List<EventEntityBase>> help = new Dictionary<IEventsOwner, List<EventEntityBase>>();

        private static List<EventEntityBase> GetList(IEventsOwner msg)
        {
            List<EventEntityBase> result = null;
            if (!help.TryGetValue(msg, out result))
            {
                result = StaticPool.Get<List<EventEntityBase>>();
                help.Add(msg, result);
            }
            return result;
        }
        private static List<EventEntityBase> FindList(IEventsOwner msg)
        {
            List<EventEntityBase> result = null;
            help.TryGetValue(msg, out result);
            return result;
        }

        private static void TryRecycleList(IEventsOwner key, List<EventEntityBase> list)
        {
            if (list.Count != 0) return;
            StaticPool.Set(list);
            help.Remove(key);
        }


        public static IEventEntity SubscribeEvent(this IEventsOwner self, string msg, Action<IEventArgs> action)
        {
            EventEntityBase entity = Subscribe(msg, action) as EventEntityBase;
            entity.owner = self;
            var list = GetList(self);
            list.Add(entity);
            return entity;
        }
        public static IEventEntity SubscribeEvent(this IEventsOwner self, string msg, Func<IEventArgs, AsyncTask> action)
        {
            EventEntityBase entity = Subscribe(msg, action) as EventEntityBase;
            entity.owner = self;
            var list = GetList(self);
            list.Add(entity);
            return entity;
        }
        public static IEventEntity SubscribeEvent<T>(this IEventsOwner self, IEventHandler handler) where T : IEventArgs
        {
            EventEntityBase entity = Subscribe<T>(handler) as EventEntityBase;
            entity.owner = self;
            var list = GetList(self);
            list.Add(entity);
            return entity;
        }

        public static void DisposeEvents(this IEventsOwner self)
        {
            var list = FindList(self);

            if (list == null) return;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var e = list[i];
                if (e.owner == self)
                {
                    e.Dispose();
                    list.RemoveAt(i);
                }
            }
            TryRecycleList(self, list);
        }








        internal static bool SetAsInvoke(EventEntityBase entity)
        {
            var list = FindContext(entity.msg, false);
            if (list == null) return false;
            return list.SetAsInvoke(entity);

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
            task.AttachCancellationToken(token);
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
            task.AttachCancellationToken(token);

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
