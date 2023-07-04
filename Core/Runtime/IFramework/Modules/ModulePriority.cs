namespace IFramework
{
    /// <summary>
    /// 默认的模块的优先级
    /// </summary>
    public struct ModulePriority
    {
        /// <summary>
        /// 配置表
        /// </summary>
        public const int Config = 0;
        /// <summary>
        /// 环境等待
        /// </summary>
        public const int Loom = 10;
        /// <summary>
        /// undo
        /// </summary>
        public const int Recorder = 30;

        /// <summary>
        /// 协程
        /// </summary>
        public const int Coroutine = 70;
        /// <summary>
        /// 消息转发
        /// </summary>
        public const int Message = 120;
        /// <summary>
        /// ecs
        /// </summary>
        public const int ECS = 400;
        /// <summary>
        /// fsm
        /// </summary>
        public const int FSM = 500;
        /// <summary>
        /// 计时器
        /// </summary>
        public const int Timer = 600;
        /// <summary>
        /// 其他
        /// </summary>
        public const int Custom = 1000;
        private int _value;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public ModulePriority(int value)
        {
            _value = value;
        }
        /// <summary>
        /// 具体的值
        /// </summary>
        public int value { get { return _value; }set { _value = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ModulePriority FromValue(int value)
        {
            return new ModulePriority(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator int(ModulePriority value)
        {
            return value.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ModulePriority(int value)
        {
            return new ModulePriority(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ModulePriority operator +(ModulePriority a, ModulePriority b)
        {
            return new ModulePriority(a.value + b.value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ModulePriority operator -(ModulePriority a, ModulePriority b)
        {
            return new ModulePriority(a.value - b.value);
        }
    }
}
