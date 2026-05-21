using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;

namespace IFramework
{
    public interface IAwaiter : INotifyCompletion, ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }
    public interface IAwaiter<out TResult> : INotifyCompletion, ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }
        TResult GetResult();
    }

    public struct AsyncTaskMethodBuilder
    {
        private AsyncTask task;

        [DebuggerHidden]
        public static AsyncTaskMethodBuilder Create() => new AsyncTaskMethodBuilder()
        {
            task = AsyncTask.CreateFromPool()
        };

        [DebuggerHidden]
        public AsyncTask Task => this.task;

        [DebuggerHidden]
        public void SetException(Exception exception) => this.task.SetException(exception);

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult() => this.task.SetResult();

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine => awaiter.OnCompleted(stateMachine.MoveNext);

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine => awaiter.OnCompleted(stateMachine.MoveNext);

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }

    public struct AsyncTaskMethodBuilder<T>
    {
        private AsyncTask<T> tcs;

        // 1. Static Create method.
        [DebuggerHidden]
        public static AsyncTaskMethodBuilder<T> Create() => new AsyncTaskMethodBuilder<T>()
        {
            tcs = AsyncTask<T>.CreateFromPool()
        };

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public AsyncTask<T> Task => this.tcs;

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception) => this.tcs.SetException(exception);

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult(T result) => this.tcs.SetResult(result);

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine => awaiter.OnCompleted(stateMachine.MoveNext);

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine => awaiter.OnCompleted(stateMachine.MoveNext);

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }

    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder))]
    public class AsyncTask
    {
        private bool log = true;
        internal static T CreateCompleteTask<T>() where T : AsyncTask, new() => new T() { IsCompleted = true };
        internal static T CreateCanceledTask<T>() where T : AsyncTask, new()
        {
            T task = new T() { log = false };
            task.Cancel(default);
            return task;
        }

        private static AsyncTask _canceledTask = CreateCanceledTask<AsyncTask>();

        private static AsyncTask _compeledTask = CreateCompleteTask<AsyncTask>();

        public static AsyncTask CanceledTask => _canceledTask;

        public static AsyncTask CompletedTask => _compeledTask;


        private bool fromPool = false;

        public event Action completed;
        public event Action canceled;
        public Exception exception { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsCanceled => exception is AsyncTaskCanceledException;

        public void Cancel(CancellationToken token = default)
        {
            if (IsCompleted || IsCanceled) return;
            canceled?.Invoke();
            canceled = null;
            SetException(new AsyncTaskCanceledException(token));
        }
        protected void CallComplete()
        {
            if (IsCompleted) return;
            _tokenRegistration.Dispose();
            IsCompleted = true;
            completed?.Invoke();
            completed = null;
            if (!fromPool) return;
            //ResetToPool();
            BackToPool();
        }

        internal void SetException(Exception exception)
        {
            this.exception = exception;
            if (log)
                Log.Exception(exception);
            CallComplete();
        }
        public virtual void SetResult() => CallComplete();
        [DebuggerHidden]
        public void Coroutine() { }
        public AsyncTask ContinueWith(Action<AsyncTask> action)
        {
            completed += () => action?.Invoke(this);
            return this;
        }

        public T ContinueWith<T>(Action<T> action) where T : AsyncTask
        {
            completed += () => action?.Invoke(this as T);
            return this as T;
        }
        public AsyncTask CancelWith(Action<AsyncTask> action)
        {
            canceled += () => action?.Invoke(this);
            return this;
        }
        public T CancelWith<T>(Action<T> action) where T : AsyncTask
        {
            canceled += () => action?.Invoke(this as T);
            return this as T;
        }

        //private CancellationToken _attachedToken;
        private CancellationTokenRegistration _tokenRegistration;
        public void AttachCancellationToken(CancellationToken token)
        {
            if (IsCompleted) return;
            //_attachedToken = token;
            _tokenRegistration.Dispose();
            if (token.IsCancellationRequested)
            {
                Cancel(token);
                return;
            }
            _tokenRegistration = token.Register(() =>
            {
                if (!IsCompleted)
                    Cancel(token);
            });
        }


        protected static T AllocatePoolTask<T>() where T : AsyncTask, new()
        {
            var task = StaticPool.Get<T>();
            task.ResetFromPool();
            task.fromPool = true;
            return task;
        }
        protected static void SetToPool<T>(T task) where T : AsyncTask, new()
        {
            StaticPool.Set<T>(task);
        }

        public static AsyncTask CreateFromPool() => AllocatePoolTask<AsyncTask>();
        protected virtual void BackToPool() => SetToPool(this);
        protected virtual void ResetFromPool()
        {
            _tokenRegistration.Dispose();
            canceled = null;
            completed = null;
            exception = null;
            IsCompleted = false;
            //_attachedToken = default;

        }





        public static AsyncTask WhenAny(params AsyncTask[] tasks) => _WhenAny(tasks);
        public static AsyncTask WhenAll(params AsyncTask[] tasks) => _WhenAll(tasks);
        public static AsyncTask WhenAny(CancellationToken token = default, params AsyncTask[] tasks) => _WhenAny(tasks, token);
        public static AsyncTask WhenAll(CancellationToken token = default, params AsyncTask[] tasks) => _WhenAll(tasks, token);
        public static AsyncTask WhenAny(IEnumerable<AsyncTask> tasks, CancellationToken token = default) => _WhenAny(tasks, token);
        public static AsyncTask WhenAll(IEnumerable<AsyncTask> tasks, CancellationToken token = default) => _WhenAll(tasks, token);
        public static AsyncTask<T> WhenAny<T>(IEnumerable<AsyncTask<T>> tasks, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return AsyncTask<T>.CanceledTaskT;
            int count = tasks != null ? tasks.Count() : 0;
            var result = AsyncTask<T>.CreateFromPool();
            if (count == 0)
            {
                result.SetResult();
                return result;
            }

            bool completed = false;

            void OnFirstComplete(AsyncTask<T> t)
            {
                if (completed) return;
                completed = true;
                if (token.IsCancellationRequested)
                    result.Cancel(token);
                else if (t.exception != null)
                    result.SetException(t.exception);
                else
                    result.SetResult(t.result);
            }

            foreach (var t in tasks)
            {
                if (t.IsCompleted)
                {
                    OnFirstComplete(t);
                    return result;
                }
                t.ContinueWith<AsyncTask<T>>(OnFirstComplete);
            }

            token.Register(() =>
            {
                if (!result.IsCompleted)
                    result.Cancel(token);
            });

            return result;

        }

        private static AsyncTask _WhenAll(IEnumerable<AsyncTask> tasks, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return CanceledTask;

            int count = tasks != null ? tasks.Count() : 0;
            AsyncTask result = AsyncTask.CreateFromPool();
            if (count == 0)
            {
                result.SetResult();
                return result;
            }
            int remaining = count;
            bool canceled = false;
            void OnTaskComplete(AsyncTask t)
            {
                if (canceled) return;
                if (token.IsCancellationRequested)
                {
                    canceled = true;
                    result.Cancel(token);
                    return;
                }
                if (t.exception != null && !(t.exception is AsyncTaskCanceledException))
                {
                    canceled = true;
                    result.SetException(t.exception);
                    return;
                }
                remaining--;
                if (remaining == 0)
                    result.SetResult();
            }

            foreach (var t in tasks)
            {
                if (t.IsCompleted)
                    OnTaskComplete(t);
                else
                    t.ContinueWith(OnTaskComplete);
            }

            // 外部取消处理
            token.Register(() =>
            {
                if (!result.IsCompleted)
                    result.Cancel(token);
            });

            return result;
        }
        private static AsyncTask _WhenAny(IEnumerable<AsyncTask> tasks, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return CanceledTask;
            int count = tasks != null ? tasks.Count() : 0;
            var result = CreateFromPool();
            if (count == 0)
            {
                result.SetResult();
                return result;
            }

            bool completed = false;

            void OnFirstComplete(AsyncTask t)
            {
                if (completed) return;
                completed = true;
                if (token.IsCancellationRequested)
                    result.Cancel(token);
                else if (t.exception != null)
                    result.SetException(t.exception);
                else
                    result.SetResult();
            }

            foreach (var t in tasks)
            {
                if (t.IsCompleted)
                {
                    OnFirstComplete(t);
                    return result;
                }
                t.ContinueWith(OnFirstComplete);
            }

            token.Register(() =>
            {
                if (!result.IsCompleted)
                    result.Cancel(token);
            });

            return result;
        }

        public static AsyncTask Delay(float second, CancellationToken token = default, bool editor = false)
        {
            if (token.IsCancellationRequested)
                return CanceledTask;

            AsyncTask task = AsyncTask.CreateFromPool();

            editor |= !Application.isPlaying;



            if (!editor)
            {
                float end = Time.time + second;
                void Update()
                {
                    if (token.IsCancellationRequested)
                    {
                        Launcher.UnBindUpdate(Update);
                        task.Cancel(token);
                    }
                    if (end <= Time.time)
                    {
                        Launcher.UnBindUpdate(Update);
                        task.SetResult();
                    }
                }
                if (Application.isPlaying)
                    Launcher.BindUpdate(Update);
            }
            else
            {
#if UNITY_EDITOR
                float end = (float)UnityEditor.EditorApplication.timeSinceStartup + second;
                void Update()
                {
                    if (token.IsCancellationRequested)
                    {
                        UnityEditor.EditorApplication.update -= Update;
                        task.Cancel(token);
                    }
                    if (end <= UnityEditor.EditorApplication.timeSinceStartup)
                    {
                        UnityEditor.EditorApplication.update -= Update;
                        task.SetResult();
                    }
                }
                UnityEditor.EditorApplication.update += Update;
#endif
            }


            return task;
        }
        public static AsyncTask Run(Action<CancellationToken> action, CancellationToken token = default)
        {
            var task = CreateFromPool();
            token.ThrowIfCancellationRequested();
            try
            {
                action(token);
                task.SetResult();
            }
            catch (AsyncTaskCanceledException e)
            {
                task.SetException(e);
            }
            catch (Exception e)
            {
                task.SetException(e);
            }
            return task;
        }
        public static AsyncTask<T> Run<T>(Func<CancellationToken, T> func, CancellationToken cancellationToken = default)
        {
            var task = AsyncTask<T>.CreateFromPool();
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var value = func(cancellationToken);
                task.SetResult(value);
            }
            catch (AsyncTaskCanceledException e)
            {
                task.SetException(e);
            }
            catch (Exception e)
            {
                task.SetException(e);
            }
            return task;
        }
        public IAwaiter GetAwaiter() => new AsyncTaskAwaiter(this);


    }
    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder<>))]
    public class AsyncTask<T> : AsyncTask
    {
        private static AsyncTask<T> _compeledTask = CreateCompleteTask<AsyncTask<T>>();

        public static AsyncTask<T> CompletedTaskT => _compeledTask;
        private static AsyncTask<T> _canceledTask = CreateCanceledTask<AsyncTask<T>>();


        public static AsyncTask<T> CanceledTaskT => _canceledTask;

        protected override void ResetFromPool()
        {
            base.ResetFromPool();
            result = default;
        }
        public new static AsyncTask<T> CreateFromPool() => AllocatePoolTask<AsyncTask<T>>();
        protected override void BackToPool() => SetToPool(this);



        public T result { get; private set; }
        public void SetResult(T result)
        {
            this.result = result;
            CallComplete();
        }
        public override void SetResult()
        {
            this.SetResult(default);
        }
        public new IAwaiter<T> GetAwaiter() => new AsyncTaskAwaiter<T>(this);

    }
    struct AsyncTaskAwaiter : IAwaiter, ICriticalNotifyCompletion
    {
        private AsyncTask task;
        private Queue<Action> calls;
        public AsyncTaskAwaiter(AsyncTask task)
        {
            if (task == null) throw new ArgumentNullException("task");
            this.task = task;
            calls = new Queue<Action>();
            this.task.completed += Task_completed;
        }

        private void Task_completed()
        {
            while (calls.Count != 0)
            {
                calls.Dequeue()?.Invoke();
            }
        }

        public bool IsCompleted => task.IsCompleted;

        public void GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");
            calls.Enqueue(continuation);
        }


    }
    struct AsyncTaskAwaiter<T> : IAwaiter<T>, ICriticalNotifyCompletion
    {
        private AsyncTask<T> task;
        private Queue<Action> calls;
        public AsyncTaskAwaiter(AsyncTask<T> task)
        {
            if (task == null) throw new ArgumentNullException("task");
            this.task = task;
            calls = new Queue<Action>();
            this.task.completed += Task_completed;
        }

        private void Task_completed()
        {
            while (calls.Count != 0)
            {
                calls.Dequeue()?.Invoke();
            }
        }

        public bool IsCompleted => task.IsCompleted;

        public T GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");
            return task.result;
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");
            calls.Enqueue(continuation);
        }


    }





    public class AsyncTaskCanceledException : Exception
    {
        public CancellationToken Token { get; }
        public AsyncTaskCanceledException() : base("Operation was canceled.") { }
        public AsyncTaskCanceledException(CancellationToken token) : base("Operation was canceled.") => Token = token;
    }
    public struct CancellationToken
    {
        private readonly CancellationTokenSource _source;
        internal CancellationToken(CancellationTokenSource source) => _source = source;
        public string userData => _source?.userData;
        public bool IsCancellationRequested => _source != null && _source.IsCancellationRequested;
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
                throw new AsyncTaskCanceledException(this);
        }
        public CancellationTokenRegistration Register(Action callback)
        {
            if (_source == null) return default;
            return _source.Register(callback);
        }
    }
    public class CancellationTokenSource
    {
        public string userData { get; private set; }
        private bool _canceled;
        private readonly List<Action> _callbacks = new List<Action>();

        public bool IsCancellationRequested => _canceled;
        public CancellationToken Token => new CancellationToken(this);

        public void Cancel()
        {
            if (_canceled) return;
            _canceled = true;
            if (_callbacks.Count == 0) return;
            for (int i = 0; i < _callbacks.Count; i++)
            {
                var cb = _callbacks[i];
                cb?.Invoke();
            }
            _callbacks.Clear();
        }

        internal CancellationTokenRegistration Register(Action callback)
        {
            if (_canceled)
            {
                callback?.Invoke();
                return default;
            }
            _callbacks.Add(callback);
            return new CancellationTokenRegistration(this, callback);
        }

        internal void Unregister(Action callback) => _callbacks.Remove(callback);
    }
    public struct CancellationTokenRegistration : IDisposable
    {
        private readonly CancellationTokenSource _source;
        private readonly Action _callback;
        internal CancellationTokenRegistration(CancellationTokenSource source, Action callback)
        {
            _source = source;
            _callback = callback;
        }
        public void Dispose() => _source?.Unregister(_callback);
    }
}
