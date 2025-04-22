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
    class TimerParallel : TimerContextBase, ITimerGroup
    {

        private Queue<TimerContextCreate> queue = new Queue<TimerContextCreate>();
        private List<ITimerContext> contexts = new List<ITimerContext>();

        public ITimerGroup NewContext(TimerContextCreate func)
        {
            if (func == null) return this;
            queue.Enqueue(func);
            return this;
        }


        public override void Cancel()
        {
            if (canceled) return;
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Cancel();
            }
            InvokeCancel();
            scheduler.Cycle(this);
        }

        protected override void Reset()
        {
            base.Reset();
            this._delta = this._time = -1;
            queue.Clear();
            contexts.Clear();
        }



        public override void Complete()
        {
            if (isDone) return;
            InvokeComplete();

            scheduler.Cycle(this);

        }



        private void OnContextEnd(ITimerContext context)
        {
            if (canceled || isDone) return;
            if (contexts.Count > 0)
                contexts.Remove(context);
            if (contexts.Count == 0)
                Complete();
        }

        public ITimerGroup Run()
        {

            InvokeBegin();
            if (queue.Count > 0)
                while (queue.Count > 0)
                {
                    var context = queue.Dequeue().Invoke(this.scheduler);
                    context.OnCancel(OnContextEnd);
                    context.OnComplete(OnContextEnd);
                    context.OnTick(_OnTick);
                    context.SetTimeScale(timeScale);

                    contexts.Add(context);
                }
            else
                Complete();
            return this;
        }

        private float _time, _delta;
        private void _OnTick(float time, float delta)
        {
            if (_time != time || _delta != delta)
            {
                this._time = time;
                this._delta = delta;
                InvokeTick(this._time, this._delta);

            }
        }

        public override void SetTimeScale(float timeScale)
        {
            if (!valid) return;
            base.SetTimeScale(timeScale);
            for (var i = 0; i < contexts.Count; i++)
                contexts[i].SetTimeScale(timeScale);
        }

        public override void Pause()
        {
            if (!valid) return;
            for (var i = 0; i < contexts.Count; i++)
                contexts[i].Pause();
        }

        public override void UnPause()
        {
            if (!valid) return;
            for (var i = 0; i < contexts.Count; i++)
                contexts[i].UnPause();
        }
    }

}
