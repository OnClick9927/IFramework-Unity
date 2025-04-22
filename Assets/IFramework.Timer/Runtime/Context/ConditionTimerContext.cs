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
    class ConditionTimerContext : TimerContext
    {
        private void CalcNextTime(float time) => nextIntervalTime = interval + time;
        private float interval;
        private TimerFunc interverl_call;
        private bool callFirst;
        private bool resultFlag;

        public bool Condition(TimerFunc condition, float interval, bool resultFlag, bool callFirst)
        {
            if (interval <= 0)
            {
                Log.FE($"Err Param {nameof(interval)}:{interval}");
                Complete();
                return false;
            }
            this.callFirst = callFirst;
            this.resultFlag = resultFlag;
            this.interverl_call = condition;
            this.interval = interval;
            return true;
        }

        private void OnceCall(float time, float delta)
        {
            bool end = interverl_call.Invoke(time, delta);
            if (end != this.resultFlag)
                CalcNextTime(time);
            else
                Complete();
        }
        protected override void OnUpdate(float time, float delta)
        {
            if (!trick_mode_first)
            {
                if (callFirst)
                    OnceCall(time, delta);
                trick_mode_first = true;
                if (isDone) return;
                CalcNextTime(time);
            }
            if (time >= nextIntervalTime)
            {
                OnceCall(time, delta);
            }
        }
        private bool trick_mode_first;
        private float nextIntervalTime;
        protected override void Reset()
        {
            base.Reset();
            trick_mode_first = false;
            nextIntervalTime = 0;

        }
    }

}
