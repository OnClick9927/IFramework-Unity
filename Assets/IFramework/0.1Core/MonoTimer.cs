/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
namespace IFramework
{
    [MonoSingletonPath("IFramework/Timmer")]
    public class MonoTimer:MonoSingletonPropertyClass<MonoTimer>
	{
        public enum TaskState
        {
            Running, Stoped, Paused, None
        }
        private class FrameTimeTask
        {
            public string id;
            public bool Paused { get; private set; }
            public Action Finish;
            public bool NeedSet { get; private set; }
            public void Pause() { Paused = true; }
            public void UnPause() { Paused = false; }


            private double delay;
            private int count;
            private double space;
            private Action action;

            private double startTime;
            private bool AwalysRun;
            public FrameTimeTask SetVal(double space, Action action, Action Finish, double delay, int count)
            {
                NeedSet = false;
                this.id = Guid.NewGuid().ToString();
                this.space = space;
                this.action = action;
                this.count = count;
                this.delay = delay;
                AwalysRun = count < 0;
                startTime = GetUtcFrame(DateTime.UtcNow);
                this.Finish = Finish;
                return this;
            }

            private double GetUtcFrame(DateTime time)
            {
                DateTime timeZerro = new DateTime(1970, 1, 1);
                return (time - timeZerro).TotalMilliseconds;
            }

            private DateTime LastTime;
            private DateTime CurTime;
            private double helpFrame;
            public void Tick()
            {
                if (Paused) return;
                CurTime = DateTime.UtcNow;
                if (GetUtcFrame(CurTime) - startTime < delay) return;
                helpFrame -= (CurTime - LastTime).TotalMilliseconds;
                if (helpFrame <= 0)
                {
                    count--;
                    helpFrame = space;
                    if (count <= 0 && !AwalysRun) NeedSet = true;
                    if (action != null && !NeedSet) action();
                }
                LastTime = CurTime;
            }
        }
        private class IFTimer
        {
            private class TimeTaskPool : ObjectPool<FrameTimeTask>
            {
                protected override FrameTimeTask CreatNew(IEventArgs arg, params object[] param)
                {
                    return new FrameTimeTask();
                }
            }
            private TimeTaskPool pool;
            private List<FrameTimeTask> Tasks;
            private LockParam param;
            public IFTimer() { pool = new TimeTaskPool(); Tasks = new List<FrameTimeTask>(); param = new LockParam(); }
            public void Tick()
            {
                using (new LockWait(ref param))
                {
                    Tasks.ForEach((task) => { task.Tick(); });
                    var setOnes = Tasks.FindAll((task) => { return task.NeedSet; });
                    if (setOnes != null)
                    {
                        setOnes.ForEach((setOne) => {
                            if (setOne.Finish != null)
                                setOne.Finish();
                            pool.Set(setOne);
                            Tasks.Remove(setOne);
                        });
                    }
                }
            }
            public string RunTimerTask(double space, Action action, Action finish, double delay, int count)
            {
                using (new LockWait(ref param))
                {
                    var task = pool.Get().SetVal(space, action, finish, delay, count);
                    Tasks.Add(task);
                    return task.id;
                }
            }
            public void StopTimerTask(string id)
            {
                using (new LockWait(ref param))
                {
                    var task = Tasks.Find((ta) => { return ta.id == id; });
                    if (task != null)
                    {
                        if (task.Finish != null)
                            task.Finish();
                        Tasks.Remove(task);
                        pool.Set(task);
                    }
                }
            }

            public TaskState GetTaskState(string id)
            {
                using (new LockWait(ref param))
                {
                    if (pool.Contains((task) => { return task.id == id; }))
                        return TaskState.Stoped;
                    var ta = Tasks.Find((task) => { return task.id == id; });
                    if (ta != null) return ta.Paused ? TaskState.Paused : TaskState.Running;
                    return TaskState.None;
                }
            }
            public void PauseTimerTask(string id)
            {
                using (new LockWait(ref param))
                {
                    var ta = Tasks.Find((task) => { return task.id == id; });
                    if (ta != null) ta.Pause();
                }
            }
            public void UnPauseTimerTask(string id)
            {
                using (new LockWait(ref param))
                {
                    var ta = Tasks.Find((task) => { return task.id == id; });
                    if (ta != null) ta.UnPause();
                }
            }

        }
        private IFTimer timer;
        private void Update()
        {
            timer.Tick();
        }
        public static string RunTimerTask( double space, Action action, Action onCompeleted, double delay, int count)
        {
           return Instance. timer.RunTimerTask( space, action, onCompeleted, delay, count);
        }

        public static void StopTimerTask(string id)
        {
            Instance.timer.StopTimerTask(id);
        }
        public static void PauseTimerTask(string id)
        {
            Instance.timer.PauseTimerTask(id);
        }
        public static void UnPauseTimerTask(string id)
        {
            Instance.timer.UnPauseTimerTask(id);
        }
        public static TaskState GetTaskState(string id)
        {
            return Instance.timer.GetTaskState(id);
        }

        protected override void OnSingletonInit()
        {
            timer = new IFTimer();
        }
        public override void Dispose()
        {
            base.Dispose();
            timer = null;

        }

    }
}
