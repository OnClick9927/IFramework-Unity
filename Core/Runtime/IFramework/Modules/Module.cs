using System;
using System.Reflection;

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

        /// <summary>
        /// 绑定模块容器
        /// </summary>
        /// <param name="container"></param>
        public void Bind(IModules container)
        {
            if (this._container != null)
            {
                Log.E(string.Format("Have Bind One Container chunck: You Can UnBind First"));
                return;
            }

            if ((container as Modules).SubscribeModule(this))
            {
                this._binded = true;
                this._container = container;
            }

        }
        /// <summary>
        /// 解除绑定模块容器
        /// </summary>
        /// <param name="dispose"></param>
        public void UnBind(bool dispose = true)
        {
            if (!binded) return;
            if (binded && this._container != null)
            {
                (this._container as Modules).UnSubscribeBindModule(this);
                this._binded = false;
                this._container = null;
            }
            if (dispose)
                Dispose();
        }

        private IModules _container;
        private bool _binded;
        private int _priority;

        /// <summary>
        /// 优先级（越大释放越早释放,越小越先 update）
        /// </summary>
        public int priority { get { return _priority; } }

        /// <summary>
        /// 是否绑定了
        /// </summary>
        public bool binded { get { return _binded; } }
        /// <summary>
        /// 模块所处的容器
        /// </summary>
        public IModules container { get { return _container; } }
        /// <summary>
        /// 名字
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            UnBind(false);
            base.Dispose();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();

    }
}
