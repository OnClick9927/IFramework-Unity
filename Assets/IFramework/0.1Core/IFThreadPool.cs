/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-04-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Threading;

namespace IFramework
{
    public class ThreadPool
    {
        private class IFThread : IDisposable
        {
            public Thread Thread { get { return thread; } }
            public bool IsRunning { get { return isRunning; } }
            private bool isRunning;
            private Thread thread;
            private ManualResetEvent eve;
            private IFThreadArg Arg;

            public IFThread()
            {
                eve = new ManualResetEvent(false);//为trur,一开始就可以执行
                thread = new Thread(ThreadLoop);
                thread.IsBackground = true;
                thread.Start();
            }

            private void Run()
            {
                this.eve.Set();
                isRunning = true;
            }
            private void Stop()
            {
                this.eve.Reset();
                ThreadPool.Set(Arg, this);
                isRunning = false;
            }

            public void RunTask(IFThreadArg arg)
            {
                this.Arg = arg;
                if (!isRunning)
                    Run();
            }
            private void ThreadLoop()
            {
                while (true)
                {
                    this.eve.WaitOne();
                    if (Arg.task != null)
                        Arg.task();
                    if (Arg.callback != null)
                        Arg.callback();
                    Stop();
                }
            }
            public void Dispose()
            {
                thread.Abort();
                thread.Join();
                thread = null;
            }
        }
        private class IFThreadArg : IEventArgs, IDisposable
        {
            public Action task;
            public Action callback;

            public void Dispose()
            {
                this.task = null; this.callback = null;
            }

            public IFThreadArg SetVal(Action task, Action callback)
            {
                this.task = task;
                this.callback = callback;
                return this;
            }
        }

        private class IFThreadArgPool : CacheInnerPool<IFThreadArg>
        {
            protected override IFThreadArg CreatNew(IEventArgs arg, params object[] param)
            {
                return new IFThreadArg();
            }
        }
        private class IFThreads :CacheInnerPool<IFThread>
        {

            protected override IFThread CreatNew(IEventArgs arg, params object[] param)
            {
                return new IFThread();
            }
        }

        static public bool isRunning { get; private set; }
        static private ManualResetEvent eve;
        static private Thread ctrolThread;
        static private Queue<IFThreadArg> CacheArgs;
        static private Queue<IFThreadArg> RunningArgs;
        static private CachePool<IFThread> ThreadCache;
        static private CachePool<IFThreadArg> ArgCache;

        static ThreadPool()
        {
            ThreadCache = new CachePool<IFThread>(new IFThreads(), new IFThreads());
            ArgCache = new CachePool<IFThreadArg>(new IFThreadArgPool(), new IFThreadArgPool());
            eve = new ManualResetEvent(false);

            CacheArgs = new Queue<IFThreadArg>();
            RunningArgs = new Queue<IFThreadArg>();

            ctrolThread = new Thread(ThreadLoop);
            ctrolThread.IsBackground = true;
            ctrolThread.Start();
        }
        private static void Run()
        {
            eve.Set();
            isRunning = true;
        }
        private static void Stop()
        {
            eve.Reset();
            isRunning = false;
        }
        private static void ThreadLoop()
        {
            while (true)
            {
                eve.WaitOne();
                lock (CacheArgs)
                {
                    while (CacheArgs.Count != 0)
                        RunningArgs.Enqueue(CacheArgs.Dequeue());
                }
                while (RunningArgs.Count != 0)
                    ThreadCache.Get().RunTask(RunningArgs.Dequeue());
                Thread.Sleep(5);
                Stop();

            }
        }
        private static void Set(IFThreadArg arg, IFThread th)
        {
            Log.L(ArgCache.RunningCount + "   " + ArgCache.SleepCount + "    " + ThreadCache.RunningCount + "     " + ThreadCache.SleepCount);
            ArgCache.Set(arg);
            ThreadCache.Set(th);
        }

        public static void Dispose()
        {
            ctrolThread.Abort();
            ctrolThread.Join();
            CacheArgs.Clear();
            RunningArgs.Clear();
            ThreadCache.Dispose();
            ArgCache.Dispose();

            eve = null;
            ctrolThread = null;

            RunningArgs = null;
            CacheArgs = null;
        }
        public static void RunTask(Action task, Action callback)
        {
            lock (CacheArgs)
                CacheArgs.Enqueue(ArgCache.Get().SetVal(task, callback));
            if (!isRunning)
                Run();
        }

    }

}