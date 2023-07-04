using System;

namespace IFramework
{
    /// <summary>
    /// 模块
    /// </summary>
    public abstract class Module : Unit
    {
        /// <summary>
        /// 默认名字
        /// </summary>
        public const string defaultName = "default";


        private bool _binded;
        private int _priority;

        /// <summary>
        /// 优先级（越大释放越早释放,越小越先 update）
        /// </summary>
        public int priority { get { return _priority; } }

        /// <summary>
        /// 是否绑定了
        /// </summary>
        public bool binded { get { return _binded; } internal set { _binded = value; } }

        /// <summary>
        /// 名字
        /// </summary>
        public string name { get; set; }



        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();

        /// <summary>
        /// 阻止 New
        /// </summary>
        protected Module() { }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <param name="name">模块名称</param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static Module CreatInstance(Type type, string name = defaultName, int priority = 0)
        {
            Module moudle = Activator.CreateInstance(type) as Module;
            if (moudle != null)
            {
                moudle._binded = false;
                moudle.name = name;
                moudle._priority = moudle.OnGetDefautPriority().value + priority;
                moudle.Awake();
                if (moudle is UpdateModule)
                {
                    (moudle as UpdateModule).enable = true;
                }
            }
            else
                Log.E(string.Format("Type: {0} Non Public Ctor With 0 para Not Find", type));

            return moudle;
        }
        /// <summary>
        /// 设置优先级
        /// </summary>
        /// <returns></returns>
        protected virtual ModulePriority OnGetDefautPriority()
        {
            return ModulePriority.Custom;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="name">模块名称</param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static T CreatInstance<T>(string name = defaultName, int priority = 0) where T : Module
        {
            return CreatInstance(typeof(T), name, priority) as T;
        }





    }
}
