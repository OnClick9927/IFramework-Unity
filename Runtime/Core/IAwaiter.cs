using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
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
            task = new AsyncTask()
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
            tcs = new AsyncTask<T>()
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
        }
        internal void SetException(Exception exception)
        {
            this.exception = exception;
            Log.Exception(exception);
            CallComplete();
        }
        public virtual void SetResult()
        {
            CallComplete();
        }
        public static async AsyncTask WaitAll(params AsyncTask[] tasks)
        {
            if (tasks != null)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    var task = tasks[i];
                    if (!task.IsCompleted)
                        await task;
                }
            }
        }
        public static AsyncTask Delay(float second)
        {
            AsyncTask task = new AsyncTask();
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
            {
                Launcher.BindUpdate(Update);
            }
            return task;
        }

    }
    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder<>))]
    public class AsyncTask<T> : AsyncTask
    {
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
