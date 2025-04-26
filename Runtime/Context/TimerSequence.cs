/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework
{
    class TimerSequence : TimerContextBase, ITimerGroup
    {
        public Queue<System.Func<ITimerContext>> queue = new Queue<System.Func<ITimerContext>>();
        public ITimerGroup NewContext(System.Func<ITimerContext> func)
        {
            if (func == null) return this;
            queue.Enqueue(func);
            return this;
        }

        protected override void StopChildren()
        {
            base.StopChildren();
            inner?.Cancel();

        }

        protected override void Reset()
        {
            base.Reset();
            inner = null;
            queue.Clear();
        }



   

        private ITimerContext inner;

        private void RunNext(ITimerContext context)
        {
            if (canceled || isDone) return;
            if (queue.Count > 0)
            {
                inner = queue.Dequeue().Invoke();
                if (inner != null)
                {
                    inner.OnTick(InvokeTick);
                    inner.OnCancel(RunNext);
                    inner.OnComplete(RunNext);
                    inner.SetTimeScale(timeScale);
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

        public ITimerGroup Run()
        {
            InvokeBegin();
            RunNext(null);
            return this;
        }

        public override void SetTimeScale(float timeScale)
        {
            if (!valid) return;
            base.SetTimeScale(timeScale);
            inner?.SetTimeScale(timeScale);
        }

        public override void Pause()
        {
            if (!valid) return;
            inner?.Pause();
        }

        public override void UnPause()
        {
            if (!valid) return;
            inner?.UnPause();
        }







    }

}
