using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;

namespace IFramework
{
    public interface IAwaitable<out TAwaiter> where TAwaiter : IAwaiter { IAwaiter GetAwaiter(); }


    public interface IAwaitable<out TAwaiter, out TResult> where TAwaiter : IAwaiter<TResult> { IAwaiter<TResult> GetAwaiter(); }
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
            task = AsyncTask.AllocatePoolTask<AsyncTask>()
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
            tcs = AsyncTask.AllocatePoolTask<AsyncTask<T>>()
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
        private static AsyncTask _compeledTask = new AsyncTask() { IsCompleted = true };

        public static AsyncTask CompletedTask => _compeledTask;


        private bool fromPool = false;

        public event Action completed;
        public Exception exception { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            if (IsCompleted || IsCanceled) return;
            IsCanceled = true;
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
        public AsyncTask ContinueWith(Action<AsyncTask> continuationAction)
        {
            completed += () => continuationAction?.Invoke(this);
            return this;
        }






        internal static T AllocatePoolTask<T>() where T : AsyncTask, new()
        {
            var task = StaticPool<T>.Get();
            task.ResetFromPool();
            task.fromPool = true;
            return task;
        }
        internal virtual void BackToPool() => StaticPool<AsyncTask>.Set(this);
        internal virtual void ResetFromPool()
        {
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
            AsyncTask<T> wait = AllocatePoolTask<AsyncTask<T>>();
            if (tasks != null && tasks.Count() != 0)
            {
                foreach (var task in tasks)
                {
                    if (task.IsCompleted)
                        wait.SetResult(task.result);
                    else
                        task.ContinueWith(_ => { wait.SetResult((_ as AsyncTask<T>).result); });
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
            AsyncTask result = AsyncTask.AllocatePoolTask<AsyncTask>();
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
            AsyncTask wait = AllocatePoolTask<AsyncTask>();
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
            AsyncTask task = AllocatePoolTask<AsyncTask>();

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




    }
    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder<>))]
    public class AsyncTask<T> : AsyncTask
    {
        internal override void ResetFromPool()
        {
            base.ResetFromPool();
            result = default;
        }
        internal override void BackToPool() => StaticPool<AsyncTask<T>>.Set(this);
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
    }

    public static class AsyncTaskEx
    {

        public static IAwaiter GetAwaiter(this AsyncTask task)
        {
            return new AsyncTaskAwaiter(task);
        }
        public static IAwaiter<T> GetAwaiter<T>(this AsyncTask<T> task)
        {
            return new AsyncTaskAwaiter<T>(task);
        }
        private struct AsyncTaskAwaiter : IAwaiter, ICriticalNotifyCompletion
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
        private struct AsyncTaskAwaiter<T> : IAwaiter<T>, ICriticalNotifyCompletion
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
}
