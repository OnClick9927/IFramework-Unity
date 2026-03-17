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
        internal static T CreateCompleteTask<T>() where T : AsyncTask, new() => new T() { IsCompleted = true };
        private static AsyncTask _compeledTask = CreateCompleteTask<AsyncTask>();

        public static AsyncTask CompletedTask => _compeledTask;


        private bool fromPool = false;

        public event Action completed;
        public event Action canceled;
        public Exception exception { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            if (IsCompleted || IsCanceled) return;
            IsCanceled = true;
            canceled?.Invoke();
            canceled = null;
            CallComplete();
        }
        protected void CallComplete()
        {
            if (IsCompleted) return;
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
            canceled = null;
            completed = null;
            exception = null;
            IsCompleted = false;
            IsCanceled = false;
        }






        public static AsyncTask WhenAny(params AsyncTask[] tasks) => _WhenAny(tasks);
        public static AsyncTask WhenAny(IEnumerable<AsyncTask> tasks) => _WhenAny(tasks);
        public static AsyncTask WhenAll(params AsyncTask[] tasks) => _WhenAll(tasks);
        public static AsyncTask WhenAll(IEnumerable<AsyncTask> tasks) => _WhenAll(tasks);
        public static AsyncTask<T> WhenAny<T>(IEnumerable<AsyncTask<T>> tasks)
        {
            AsyncTask<T> wait = AsyncTask<T>.CreateFromPool();
            if (tasks != null && tasks.Count() != 0)
            {
                foreach (var task in tasks)
                {
                    if (task.IsCompleted)
                        wait.SetResult(task.result);
                    else
                        task.ContinueWith<AsyncTask<T>>(_ => { wait.SetResult(_.result); });
                }
            }
            else
            {
                wait.SetResult(default);
            }
            return wait;
        }

        private static AsyncTask _WhenAll(IEnumerable<AsyncTask> tasks)
        {
            int count = tasks != null ? tasks.Count() : 0;
            AsyncTask result = AsyncTask.CreateFromPool();
            if (count == 0)
                result.SetResult();
            else
            {
                int index = 0;
                void CallAdd(AsyncTask prev)
                {
                    index++;
                    if (index >= count)
                        result.SetResult();
                }
                foreach (var task in tasks)
                {
                    if (!task.IsCompleted)
                        task.ContinueWith(CallAdd);
                    else
                        CallAdd(task);
                }
            }
            return result;
        }
        private static AsyncTask _WhenAny(IEnumerable<AsyncTask> tasks)
        {
            AsyncTask wait = AsyncTask.CreateFromPool();
            if (tasks != null && tasks.Count() != 0)
            {
                foreach (var task in tasks)
                {
                    if (task.IsCompleted)
                        wait.SetResult();
                    else
                        task.ContinueWith(_ => { wait.SetResult(); });
                }
            }
            else
            {
                wait.SetResult();
            }
            return wait;
        }

        public static AsyncTask Delay(float second, bool editor = false)
        {
            AsyncTask task = AsyncTask.CreateFromPool();

            editor |= !Application.isPlaying;



            if (!editor)
            {
                float end = Time.time + second;
                void Update()
                {
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
                async void EditorWait()
                {
                    await System.Threading.Tasks.Task.Delay((int)(second * 1000));
                    task.SetResult();
                }
                EditorWait();
#endif
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

}
