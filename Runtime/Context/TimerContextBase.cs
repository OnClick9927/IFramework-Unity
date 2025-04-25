/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework
{
    public abstract class TimerContextBase : ITimerContext, IPoolObject
    {
        //public string guid { get; private set; } = Guid.NewGuid().ToString();

        public bool isDone { get; private set; }
        public bool canceled { get; private set; }
        public bool valid { get; set; }

        public string id { get; private set; }
        public object owner { get; private set; }

        internal TimerScheduler scheduler;

        //public bool valid { get; internal set; }
        private Action<ITimerContext> onComplete;
        private Action<ITimerContext> onCancel;
        private Action<ITimerContext> onBegin;

        private TimerAction onTick;
        protected float timeScale { get; private set; }
        public void OnComplete(Action<ITimerContext> action) => onComplete += action;
        public void OnCancel(Action<ITimerContext> action) => onCancel += action;
        public void OnTick(TimerAction action) => onTick += action;
        public void OnBegin(Action<ITimerContext> action) => onBegin += action;

        protected void InvokeBegin()
        {
            onBegin?.Invoke(this);

        }
        protected void InvokeTick(float time, float delta)
        {
            onTick?.Invoke(time, delta);

        }
        protected void InvokeCancel()
        {
            if (canceled) return;

            canceled = true;
            onCancel?.Invoke(this);
        }
        protected void SetCancel()
        {
            if (canceled) return;
            canceled = true;

        }
        protected void InvokeComplete()
        {
            if (isDone) return;
            isDone = true;
            onComplete?.Invoke(this);
        }

        protected virtual void Reset()
        {
            id = string.Empty;
            owner = null;
            onBegin = null;
            onCancel = null;
            onComplete = null;
            onTick = null;




            isDone = false;
            canceled = false;

            timeScale = 1;
        }


        public abstract void Stop();
        public abstract void Cancel();
        protected void Complete()
        {
            if (isDone) return;
            InvokeComplete();
            if (this is ITimerGroup)
                scheduler.Cycle(this);

        }


        public virtual void SetTimeScale(float timeScale)
        {
            if (!valid) return;
            this.timeScale = timeScale;
        }
        public void SetId(string id)
        {
            if (!valid) return;
            this.id = id;
        }
        public void SetOwner(object owner)
        {
            if (!valid) return;
            this.owner = owner;
        }
        public abstract void Pause();

        public abstract void UnPause();






        void IPoolObject.OnGet()
        {
            Reset();
        }

        void IPoolObject.OnSet()
        {
            Reset();
        }
    }

}
