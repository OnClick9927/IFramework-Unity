using System;
using System.Collections;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 协程 模拟
    /// </summary>
    internal class Coroutine : YieldInstruction, ICoroutine
    {
        private Coroutine _innerAction;
        private CoroutineState _state;
        internal CoroutineModule _module;
        internal IEnumerator _routine;
        internal event Action onCompelete;
        public CoroutineState state { get { return _state; } }

        /// <summary>
        /// 协程完成时候回调
        /// </summary>
        /// <summary>
        /// 手动结束协程
        /// </summary>
        public void Compelete()
        {
            if (_innerAction != null)
            {
                _innerAction.Compelete();
            }
            if (onCompelete != null)
                onCompelete();

            onCompelete = null;
            _innerAction = null;
            _routine = null;
            _state = CoroutineState.Rest;

            _module.Set(this);
        }

        public CoroutineAwaiter GetAwaiter()
        {
            return new CoroutineAwaiter(this);
        }

        protected override bool IsCompelete()
        {
            if (state != CoroutineState.Working) return false;
            if (_innerAction == null)
            {
                if (!_routine.MoveNext())
                {
                    return true;
                }
                if (_routine.Current != null)
                {
                    IEnumerator ie = null;
                    if (_routine.Current is YieldInstruction)
                        ie = (_routine.Current as YieldInstruction).AsEnumerator();
                    else if (_routine.Current is IEnumerator)
                        ie = _routine.Current as IEnumerator;
                    if (ie != null)
                    {
                        _innerAction = _module.CreateCoroutine(ie) as Coroutine;
                        _innerAction.Resume();
                    }
                }
            }
            if (_innerAction != null)
            {
                if (_innerAction.isDone)
                {
                    _innerAction.Compelete();
                    _innerAction = null;
                }
            }

            return false;
        }


        public void Pause()
        {
            _state = CoroutineState.Yied;
        }
        public void Resume()
        {
            _state = CoroutineState.Working;
        }
    }
}
