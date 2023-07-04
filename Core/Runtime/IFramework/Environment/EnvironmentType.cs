using System;

namespace IFramework
{
    /// <summary>
    /// 环境类型
    /// </summary>
    [Flags]
    public enum EnvironmentType : int
    {
        /// <summary>
        /// 所有，配合环境初始化
        /// </summary>
        None = 1,
        /// <summary>
        /// 环境0
        /// </summary>
        Ev0 = 2,
        /// <summary>
        /// 环境1
        /// </summary>
        Ev1 = 4,
        /// <summary>
        /// 环境2
        /// </summary>
        Ev2 = 8,
        /// <summary>
        /// 环境3
        /// </summary>
        Ev3 = 16,
        /// <summary>
        /// 环境4
        /// </summary>
        Ev4 = 32,
        /// <summary>
        /// 环境5
        /// </summary>
        Ev5 = 64,
        /// <summary>
        /// 环境6
        /// </summary>
        Ev6 = 128,
        /// <summary>
        /// 环境7
        /// </summary>
        Ev7 = 256,
        /// <summary>
        /// 环境8
        /// </summary>
        Ev8 = 512,
        /// <summary>
        /// 环境9
        /// </summary>
        Ev9 = 1024,

        /// <summary>
        /// 环境10
        /// </summary>
        Ev10 = 2048,
        /// <summary>
        /// 
        /// </summary>
        Ev11 = 4096,
        /// <summary>
        /// 
        /// </summary>
        Ev12 = 8192,
  
        /// <summary>
        /// 额外的1
        /// </summary>
        Extra0 = 16384,
        /// <summary>
        /// 额外的1
        /// </summary>
        Extra1 = 32768,
        /// <summary>
        /// 额外的2
        /// </summary>
        Extra2 = 65536,
        /// <summary>
        /// 额外的3
        /// </summary>
        Extra3 = 131072,
        /// <summary>
        /// 额外的4
        /// </summary>
        Extra4 = 262144,
        /// <summary>
        /// 
        /// </summary>
        Extra5 = 524288,
    }

}
