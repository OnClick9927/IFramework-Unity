using System.Collections.Generic;

namespace IFramework.Recorder
{
    /// <summary>
    /// 操作记录
    /// </summary>
    public partial class OperationRecorderModule : Module, IOperationRecorderModule
    {
        private HeadState _head;
        private BaseState _current;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override ModulePriority OnGetDefautPriority()
        {
            return ModulePriority.Recorder;
        }
        protected override void Awake()
        {
            _head = Allocate<HeadState>();
            _head.SetName("head");
            _current = _head;
        }
        protected override void OnDispose()
        {
            Recyle(_head);
            _current = _head = null;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 分配状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Allocate<T>() where T : BaseState, new()
        {
            var state = new T();
            state.recorder = this;
            state.SetName(null);
            return state;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="state"></param>
        /// <param name="redo"></param>
        public void Subscribe(BaseState state, bool redo = true)
        {
            if (state.recorder == null)
                Log.E("Don't Create new State ,Use Allocate");
            if (_current == null) _current = _head;
            if (state.guid == _current.guid)
            {
                state = state.Clone() as BaseState;
                state.SetName($"{_current.name}#Clone");
            }
            if (_current.next != null)
            {
                Recyle(_current.next);
            }
            _current.next = state;
            state.front = _current;
            _current = state;
            if (redo) state.Redo();
        }
        /// <summary>
        /// 获取记录列表
        /// </summary>
        /// <param name="index">当前记录的位置</param>
        /// <returns></returns>
        public List<string> GetRecordNames(out int index)
        {
            index = 0;
            List<string> names = new List<string>();
            BaseState baseState = _head;
            while (baseState != null)
            {
                if (_current == baseState)
                {
                    index = names.Count;
                }
                names.Add(baseState.name);
                baseState = baseState.next;
            }
            return names;
        }



        Queue<BaseState> queue = new Queue<BaseState>();
        private void Recyle(BaseState state)
        {
            do
            {
                var now = state;
                state = state.next;
                now.Reset();
                queue.Enqueue(now);
            } while (state != null);
            while (queue.Count != 0)
            {
                queue.Dequeue();
            }
        }
        /// <summary>
        /// 撤回
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            if (_current == _head) return false;
            _current.Undo();
            _current = _current.front;
            return true;
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            if (_current.next == null) return false;
            _current = _current.next;
            _current.Redo();
            return true;
        }

        /// <summary>
        /// 获取当前节点的名字
        /// </summary>
        /// <returns></returns>
        public string GetCurrentRecordName()
        {
            return _current.name;
        }
    }
}
