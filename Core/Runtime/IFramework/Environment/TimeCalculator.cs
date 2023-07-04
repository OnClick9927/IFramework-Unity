using System;
using System.Diagnostics;

namespace IFramework
{
    class TimeCalculator : Unit, ITimeCalculator
    {
        private Stopwatch sw_init;
        private Stopwatch sw_delta;

        public TimeCalculator()
        {
            deltaTime = TimeSpan.Zero;
            sw_delta = new Stopwatch();
            sw_init = new Stopwatch();
            sw_init.Start();
        }
        public TimeSpan deltaTime { get; private set; }
        public TimeSpan timeSinceInit
        {
            get
            {
                if (sw_init == null) return TimeSpan.Zero;
                sw_init.Stop();
                var span = sw_init.Elapsed;
                sw_init.Start();
                return span;
            }
        }

        public void BeginDelta()
        {
            sw_delta.Reset();
            sw_delta.Start();
        }
        public void EndDelta()
        {
            sw_delta.Stop();
            deltaTime = sw_delta.Elapsed;
        }

        protected override void OnDispose()
        {

        }
    }
}
