/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    class TweenSequence : TweenContextBase, ITweenGroup
    {
        private List<Func<ITweenContext>> list = new List<Func<ITweenContext>>();
        private Queue<Func<ITweenContext>> _queue = new Queue<Func<ITweenContext>>();

        public ITweenGroup NewContext(Func<ITweenContext> func)
        {
            if (func == null) return this;
            list.Add(func);
            return this;
        }
        private ITweenContext inner;
        public override void Stop()
        {
            inner?.Stop();
            inner = null;
            TryRecycle();
        }
        private void Cancel()
        {
            if (canceled) return;
            InvokeCancel();
            inner?.Cancel();
            TryRecycle();

        }
        private void Complete()
        {
            if (isDone) return;
            InvokeComplete();
            TryRecycle();
        }
        protected override void Reset()
        {
            base.Reset();
            inner = null;
            list.Clear();
            _queue.Clear();
        }

        public override void Complete(bool callComplete)
        {
            if (callComplete)
                Complete();
            else
                Cancel();
        }





        private void RunNext(ITweenContext context)
        {
            if (canceled || isDone) return;

            if (_queue.Count > 0)
            {
                inner = _queue.Dequeue().Invoke();
                if (inner != null)
                {
                    inner.OnTick(_OnTick);
                    inner.OnCancel(RunNext);
                    inner.OnComplete(RunNext);
                    inner?.SetTimeScale(this.timeScale);
                }
                else
                {
                    RunNext(context);
                }
            }
            else
            {
                Complete();
            }
        }

        private void _OnTick(ITweenContext context, float time, float delta)
        {
            InvokeTick(time, delta);
        }

        public override void Run()
        {
            base.Run();
            _queue.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var func = list[i];
                _queue.Enqueue(func);
            }
            RunNext(null);
        }

        public override ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            base.SetTimeScale(timeScale);
            inner?.SetTimeScale(timeScale);
            return this;
        }

        public override void Pause()
        {
            if (!valid || paused) return;
            base.Pause();
            inner?.Pause();
        }

        public override void UnPause()
        {
            if (!valid || !paused) return;
            base.UnPause();
            inner?.UnPause();
        }



    }





}