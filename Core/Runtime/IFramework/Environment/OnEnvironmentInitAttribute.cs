using System;

namespace IFramework
{
    /// <summary>
    /// 环境初始化时候调用被标记的静态类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OnEnvironmentInitAttribute : Attribute
    {
        /// <summary>
        /// 配合初始化的版本 0，
        /// 默认初始化，其他自行规定，用于区分环境，
        /// 一般某个环境特有的静态类和环境编号一致
        /// </summary>
        public EnvironmentType type { get; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="type"></param>
        public OnEnvironmentInitAttribute(EnvironmentType type = EnvironmentType.None)
        {
            this.type = type;
        }
    }


}
