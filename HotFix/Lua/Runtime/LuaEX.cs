/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.341
 *UnityVersion:   2019.4.22f1c1
 *Date:           2021-08-26
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IFramework.Hotfix.Lua
{
    public static partial class LuaEX
    {
        public struct LuaTaskAwaiter : IAwaiter, ICriticalNotifyCompletion
        {
            private ILuaTask task;

            public LuaTaskAwaiter(ILuaTask task)
            {
                this.task = task ?? throw new ArgumentNullException("task");
            }

            public bool IsCompleted => task.IsCompleted;

            public void GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");

                if (task.GetException() != null)
                    throw new Exception(task.GetException());
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                    throw new ArgumentNullException("continuation");

                task.OnCompleted(continuation);
            }
        }

        public struct LuaTaskAwaiter<TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion
        {
            private ILuaTask<TResult> task;

            public LuaTaskAwaiter(ILuaTask<TResult> task)
            {
                this.task = task ?? throw new ArgumentNullException("task");
            }

            public bool IsCompleted => task.IsCompleted;

            public TResult GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");

                if (task.GetException() != null)
                    throw new Exception(task.GetException());

                return task.GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                    throw new ArgumentNullException("continuation");

                task.OnCompleted(continuation);
            }
        }
    }
    partial class LuaEX
    {
        public static IAwaiter GetAwaiter(this ILuaTask target)
        {
            return new LuaTaskAwaiter(target);
        }
        public static IAwaiter<T> GetAwaiter<T>(this ILuaTask<T> target)
        {
            return new LuaTaskAwaiter<T>(target);
        }
        public static bool IsNull(this UnityEngine.Object o)
        {
            return o == null;
        }



      
    }
}
