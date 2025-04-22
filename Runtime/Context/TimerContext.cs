/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public abstract class TimerContext : TimerContextBase, ITimerContext
    {

        private float time;
        private bool pause;
        private bool beginCalled;
        public void Update(float delta)
        {
            if (canceled || pause || isDone) return;
            if (!beginCalled)
            {
                InvokeBegin();
                beginCalled = true;
            }
            delta *= timeScale;
            time += delta;

            InvokeTick(time, delta);
            OnUpdate(time, delta);
        }




        protected abstract void OnUpdate(float time, float delta);
        public override void Pause() => pause = true;
        public override void UnPause() => pause = false;
        protected override void Reset()
        {
            base.Reset();
            this.time = 0;
            this.beginCalled = false;
        }
        public override void Complete()
        {
            if (!valid || isDone) return;
            InvokeComplete();
            //scheduler.Cycle(this);
        }

        public override void Cancel()
        {
            if (!valid || canceled) return;
            InvokeCancel();
            //scheduler.Cycle(this);

        }


    }

}
