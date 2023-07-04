using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 协程模块
    /// </summary>
    public class CoroutineModule : UpdateModule, ICoroutineModule
    {
        private List<ICoroutine> _cors;
        private Queue<ICoroutine> _recyle;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 优先级
        /// </summary>
        protected override ModulePriority OnGetDefautPriority()
        {
            return ModulePriority.Coroutine;
        }
        protected override void OnUpdate()
        {
            {
                var count = _recyle.Count;
                for (int i = 0; i < count; i++)
                {
                    var _cor = _recyle.Dequeue();
                    _cors.Remove(_cor);
                }
            }
            {
                var count = _cors.Count;
                for (int i = 0; i < count; i++)
                {
                    var _cor = _cors[i];
                    if (_cor.state == CoroutineState.Working)
                    {
                        if ((_cor as Coroutine).isDone)
                        {
                            _cor.Compelete();
                        }
                    }
                }
            }


        }
        protected override void OnDispose()
        {
            _recyle.Clear();
            _cors.Clear();
        }

        protected override void Awake()
        {
            _cors = new List<ICoroutine>();
            _recyle = new Queue<ICoroutine>();
        }
        internal void Set(Coroutine routine)
        {
            _recyle.Enqueue(routine);
        }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 创建一个携程不跑
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public ICoroutine CreateCoroutine(IEnumerator routine)
        {
            var coroutine = new Coroutine();
            coroutine._routine = routine;
            coroutine._module = this;
            PauseCoroutine(coroutine);
            return coroutine;
        }
        /// <summary>
        /// 开启一个协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public ICoroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = CreateCoroutine(routine);
            _cors.Add(coroutine);
            ResumeCoroutine(coroutine);
            return coroutine;
        }

        /// <summary>
        /// 挂起携程
        /// </summary>
        /// <param name="coroutine"></param>
        public void PauseCoroutine(ICoroutine coroutine)
        {
            coroutine.Pause();
        }
        /// <summary>
        /// 恢复运行
        /// </summary>
        /// <param name="coroutine"></param>
        public void ResumeCoroutine(ICoroutine coroutine)
        {
            coroutine.Resume();
        }

        /// <summary>
        /// 关闭一个携程
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopCoroutine(ICoroutine coroutine)
        {
            coroutine.Compelete();
        }
    }
}
