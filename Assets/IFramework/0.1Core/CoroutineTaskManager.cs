/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-05
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;

namespace IFramework
{
    public class CoroutineTask 
    {
        public bool Running { get { return state.Running; } }
        public bool Paused { get { return state.Paused; } }
        public bool Stopped { get { return state.Stopped; } }

        internal CoroutineTask SetVal(IEnumerator coroutine, Action onCompeleted, bool autoStart = true)
        {
            if (onCompeleted==null)
                onCompeleted = Stop; 
            else
            onCompeleted += Stop;
            state = state.SetVal(coroutine, onCompeleted);
            if (autoStart)
                Start();
            return this;
        }
        public bool HasRecycled { get { return CoroutineTaskManager.HasRecycled(this); } }
        private bool CheckLeagal()
        {
            if (HasRecycled)
            {
                Log.E("Dont Do That ,Reget One Please");
            }
            return !HasRecycled;
        }
        public void Start()
        {
            if (!CheckLeagal()) return;
            if (Running)
            {
                Log.E("Has been Start");
            }
            else
            {
                state.Start();
            }
        }
        public void Stop()
        {
            if (!CheckLeagal()) return;
            if (Stopped)
            {
                Log.E("Has been Stoped");
            }
            state.Stop(); CoroutineTaskManager.StopTask(this);
        }
        public void Pause()
        {
            if (!CheckLeagal()) return;
            state.Pause();
        }
        public void UnPause()
        {
            if (!CheckLeagal()) return;
            state.Unpause();
        }
        private CoroutineTaskManager.TaskState state = new CoroutineTaskManager.TaskState();
    }
    [MonoSingletonPath("IFramework/CoroutineTask")]
    public class CoroutineTaskManager : MonoSingletonPropertyClass<CoroutineTaskManager>
    {
        private class TaskCreater : ObjectPool<CoroutineTask>
        {
            protected override CoroutineTask CreatNew(IEventArgs arg, params object[] param)
            {
                return new CoroutineTask();
            }
        }
        private TaskCreater taskCreater;

        internal class TaskState
        {
            public bool Running { get { return running; } }
            public bool Paused { get { return paused; } }
            public bool Stopped { get { return stopped; } }

            private Action Finished;

            private IEnumerator coroutine;
            private bool running;
            private bool paused;
            private bool stopped;

            public TaskState SetVal(IEnumerator c, Action Finished) { this.Finished = Finished; coroutine = c; return this; }
            public void Pause() { paused = true; }
            public void Unpause() { paused = false; }
            public void Start()
            {
                stopped = false;
                running = true;
                Instance.StartCoroutine(CallWrapper());
            }

            public void Stop()
            {
                stopped = true;
                running = false;
            }

            IEnumerator CallWrapper()
            {
                yield return null;
                IEnumerator e = coroutine;
                while (running)
                {
                    if (paused)
                        yield return null;
                    else
                    {
                        if (e != null && e.MoveNext())
                        {
                            yield return e.Current;
                        }
                        else
                        {
                            running = false;
                        }
                    }
                }
                if (Finished != null)
                    Finished();
            }
        }
        internal static void StopTask(CoroutineTask task)
        {
            Instance.taskCreater.Set(task);
        }


        public static bool HasRecycled(CoroutineTask task)
        {
            return Instance.taskCreater.Contains(task);
        }
        public static CoroutineTask CreateTask(IEnumerator coroutine, Action onCompeleted, bool autoStart = true)
        {

            return Instance.taskCreater.Get().SetVal(coroutine, onCompeleted, autoStart);
        }


        public static void KillTask(CoroutineTask t, float second)
        {
            CreateTask(TaskKiller(second, t), null, true);
        }
        private static IEnumerator TaskKiller(float delay, CoroutineTask t)
        {
            yield return new WaitForSeconds(delay);
            t.Stop();
        }
        public static CoroutineTask CreateTimeLimitedTask(IEnumerator coroutine, Action onCompeleted, float time, bool autoStart = true)
        {
            CoroutineTask t = CreateTask(coroutine, onCompeleted, autoStart);
            CreateTask(WaittimeTaskKiller(time, t), null, true);
            return t;
        }
        private static IEnumerator WaittimeTaskKiller(float delay, CoroutineTask t)
        {
            while (!t.Running)
            {
                yield return 0;
            }
            yield return new WaitForSeconds(delay);
            t.Stop();
        }
        public static CoroutineTask CreateConditionTask(IEnumerator coroutine, Action onCompeleted, Func<bool> condition, bool autoStart = true)
        {
            CoroutineTask t = CreateTask(coroutine, onCompeleted, autoStart);
            CreateTask(ConditionTaskKiller(t,condition), null, true);
            return t;
        }
        private static IEnumerator ConditionTaskKiller(CoroutineTask t, Func<bool> condition)
        {
            while (!t.Running)
            {
                yield return 0;
            }
            while (condition.Invoke())
            {
                yield return 0;
            }
            t.Stop();
        }



       protected override void OnSingletonInit()
        {
            taskCreater = new TaskCreater();
        }

        public override void Dispose()
        {
            taskCreater.Dispose();
        }
        private void OnDestroy()
        {
            Dispose();
        }
    }
}
