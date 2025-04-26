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
    class TweenParallel : TweenContextBase, ITweenGroup
    {
        public List<Func<ITweenContext>> list = new List<Func<ITweenContext>>();
        private List<ITweenContext> contexts = new List<ITweenContext>();


        public ITweenGroup NewContext(Func<ITweenContext> func)
        {
            if (func == null) return this;
            list.Add(func);
            return this;
        }
        protected override void StopChildren()
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Stop();
            }
        }

        protected override void Reset()
        {
            base.Reset();
            this._time = this._delta = -1;
            list.Clear();
            contexts.Clear();
        }

        private void OnContextEnd(ITweenContext context)
        {

            if (canceled || isDone) return;
            if (contexts.Count > 0)
                contexts.Remove(context);
            if (contexts.Count == 0)
                Complete();
        }
        private float _time, _delta;
        private void _OnTick(ITweenContext context, float time, float delta)
        {
            if (_time != time || _delta != delta)
            {
                this._time = time;
                this._delta = delta;
                InvokeTick(time, delta);
            }
        }

        public override void Run()
        {
            base.Run();
            contexts.Clear();
            if (list.Count > 0)
                for (int i = 0; i < list.Count; i++)
                {
                    var func = list[i];
                    var context = func.Invoke();
                    context.OnCancel(OnContextEnd);
                    context.OnComplete(OnContextEnd);
                    context.OnTick(_OnTick);
                    context.SetTimeScale(timeScale);
                    contexts.Add(context);
                }
            else
                Complete();
        }

        public override ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            base.SetTimeScale(timeScale);
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.SetTimeScale(timeScale);
            }
            return this;
        }

        public override void Pause()
        {
            if (!valid || paused) return;
            base.Pause();
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Pause();
            }
        }

        public override void UnPause()
        {
            if (!valid || !paused) return;
            base.UnPause();
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.UnPause();
            }
        }
    }





}