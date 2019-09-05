/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace IFramework
{
    [MonoSingletonPath("IFramework/Loom")]
    public class Loom : MonoSingletonPropertyClass<Loom>
    {
        private struct DelayedTask
        {
            public float time;
            public Action action;
            public DelayedTask(float time, Action action)
            {
                this.time = time;
                this.action = action;
            }

        }
        protected override void OnSingletonInit()
        {
            base.OnSingletonInit();
            semaphore = new Semaphore(MaxThreadCount, MaxThreadCount);
            tasks = new List<DelayedTask>();
            delayedTasks = new List<DelayedTask>();
            NoDelayTasks = new List<DelayedTask>();
        }
        public override void Dispose()
        {
            NoDelayTasks.Clear();
            tasks.Clear();
            delayedTasks.Clear();
            semaphore.Close();
        }


        private static int MaxThreadCount = 8;
        private static int ThreadCount;
        private Semaphore semaphore;
        private List<DelayedTask> tasks;
        private List<DelayedTask> delayedTasks;
        private List<DelayedTask> NoDelayTasks;

        private static double GetUtcFrame(DateTime time)
        {
            DateTime timeZerro =new DateTime(1970, 1, 1);
            return (time - timeZerro).TotalMilliseconds;
        }
        public static void RunOnMainThread(Action action, float time = 0.0f)
        {
            if (time != 0)
            {
                lock (Instance.delayedTasks)
                {
                   Instance. delayedTasks.Add(new DelayedTask((float)GetUtcFrame(DateTime.Now) + time, action));
                }
            }
            else
            {
                lock (Instance.NoDelayTasks) 
                {
                    Instance.NoDelayTasks.Add(new DelayedTask(0, action));
                }
            }
        }

        public static void RunOnSubThread(Action action)
        {
            Instance.semaphore.WaitOne();
            Interlocked.Increment(ref ThreadCount);
            System.Threading.ThreadPool.QueueUserWorkItem((ar) => {

                try
                {
                    action();
                }
                catch { }
                finally
                {
                    Interlocked.Decrement(ref ThreadCount);
                }
            });
        }

        private void Update()
        {
            lock (NoDelayTasks)
            {
                while (NoDelayTasks.Count > 0)
                {
                    NoDelayTasks.Dequeue().action();
                }
            }
            lock (tasks)
            {
                lock (delayedTasks)
                {
                    var temp = delayedTasks.Where(d => d.time <= (float)GetUtcFrame(DateTime.Now)).ToList();
                    tasks.AddRange(temp);
                    temp.ForEach((task) => { delayedTasks.Remove(task); });
                }
                while (tasks.Count != 0)
                {
                    tasks.Dequeue().action();
                }
            }
           
        }
    }
}
