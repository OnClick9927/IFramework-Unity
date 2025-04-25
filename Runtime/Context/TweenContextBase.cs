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
    abstract class TweenContextBase : ITweenContext, IPoolObject
    {
        public void Recycle()
        {
            if (!valid) return;
            Tween.RecycleContext(this);
        }
        protected void TryRecycle()
        {
            if (!autoCycle) return;
            Recycle();
        }
        public bool autoCycle { get;private set; }
        public bool isDone { get; private set; }
        public bool canceled { get; private set; }
        public bool valid { get; set; }
        public bool paused { get; private set; }
        public float timeScale { get; private set; }
        public string id { get; private set; }
        public object owner {  get; private set; }

        public TweenContextState state { get; private set; }

        private Action<ITweenContext> onBegin;

        private Action<ITweenContext> onComplete;
        private Action<ITweenContext> onCancel;
        private Action<ITweenContext, float, float> onTick;



        public void OnBegin(Action<ITweenContext> action) => onBegin += action;
        public void OnComplete(Action<ITweenContext> action) => onComplete += action;
        public void OnCancel(Action<ITweenContext> action) => onCancel += action;
        public void OnTick(Action<ITweenContext, float, float> action) => onTick += action;
        protected virtual void Reset()
        {
            paused = false;
            isDone = false;
            canceled = false;
            onCancel = null;
            onBegin = null;
            onComplete = null;
            onTick = null;
            autoCycle = true;
            timeScale = 1f;
            id = string.Empty;
            owner = null;
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
        protected void InvokeTick(float time, float tick)
        {
            onTick?.Invoke(this, time, tick);
        }


        void IPoolObject.OnGet()
        {
            state = TweenContextState.Allocate;
            Reset();
        }

        void IPoolObject.OnSet()
        {
            state = TweenContextState.Sleep;

            Reset();
        }

        public virtual ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            this.timeScale = timeScale;
            return this;
        }

        public abstract void Complete(bool callComplete);

        public virtual void Pause()
        {
            if (!valid || paused) return;
            paused = true;
            state = TweenContextState.Pause;

        }
        public virtual void UnPause()
        {
            if (!valid || !paused) return;
            paused = false;
            state = TweenContextState.Run;

        }



        public virtual void Run()
        {
            state = TweenContextState.Run;
            paused = false;
            canceled = false;
            isDone = false;
            onBegin?.Invoke(this);
        }

        public void SetAutoCycle(bool cycle)
        {
            this.autoCycle = cycle;
        }
        public void SetId(string id)
        {
            this.id = id;
        }
        public void SetOwner(object owner)
        {
            this.owner = owner;
        }

        public abstract void Stop();

    }





}