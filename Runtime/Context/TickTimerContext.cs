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
    class TickTimerContext : TimerContext
    {
        private enum Mode
        {
            Delay,
            Tick,
            DelayAndTick
        }

        private Mode mode;
        private float delay;
        private TimerAction _delay_call_delegate;


        private int _tick_times;
        private float interval;
        private TimerAction _tick_call_delegate;

        public bool Delay(float delay, TimerAction action)
        {
            if (!CheckDelay(delay)) return false;
            mode = Mode.Delay;
            this.delay = delay;
            this._delay_call_delegate = action;
            return true;
        }

        private bool CheckDelay(float delay)
        {
            if (delay <= 0)
            {
                Log.FE($"Err Param {nameof(delay)}:{delay}");
                Complete();
                return false;
            }
            return true;
        }
        private bool CheckTick(float interval, int times)
        {
            if (times == 0 || interval <= 0)
            {
                Log.FE($"Err Param {nameof(interval)}:{interval}  {nameof(times)}:{times}");
                Complete();
                return false;
            }
            return true;
        }
        public bool Tick(float interval, int times, TimerAction action)
        {
            if (!CheckTick(interval, times)) return false;
            mode = Mode.Tick;
            this.interval = interval;
            this._tick_call_delegate = action;
            this._tick_times = times;
            return true;
        }
        public bool DelayAndTick(float delay, TimerAction delayCall, float interval, int times, TimerAction action)
        {
            bool succeed = true;
            succeed &= Delay(delay, delayCall);
            succeed &= Tick(interval, times, action);
            mode = Mode.DelayAndTick;
            return succeed;
        }



        private void CallDelay(float time, float delta)
        {

            _delay_call_delegate?.Invoke(time, delta);
            delay_called = true;
        }

        private void CallTick(float time, float delta) => _tick_call_delegate?.Invoke(time, delta);



        private bool tick_mode_first;
        private bool delay_called;
        private float nextIntervalTime;
        private int _times;
        private void CalcNextTime(float time)
        {
            if (_tick_times != -1 && _times >= _tick_times)
            {
                Complete();
                return;
            }
            _times++;
            nextIntervalTime = interval + time;
        }
        protected override void OnUpdate(float time, float delta)
        {
            if (mode == Mode.Tick && !tick_mode_first)
            {
                tick_mode_first = true;
                CalcNextTime(time);
            }
            if (mode == Mode.DelayAndTick || mode == Mode.Delay)
            {
                if (!delay_called && time >= delay)
                {
                    CallDelay(time, delta);
                    if (mode == Mode.Delay)
                        Complete();
                    else
                        CalcNextTime(time);
                }
            }

            if (mode == Mode.Tick || (mode == Mode.DelayAndTick && delay_called))
            {
                if (time >= nextIntervalTime)
                {
                    CallTick(time, delta);
                    CalcNextTime(time);
                }
            }

        }

        protected override void Reset()
        {
            base.Reset();
            tick_mode_first = false;
            nextIntervalTime = 0;
            _times = 0;
            delay_called = false;
        }
    }

}
