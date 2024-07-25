/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine;

namespace IFramework.RedPoint
{
    public class RedActiveDot : RedDot
    {
        private GameObject gameObject;

        public RedActiveDot(string path, GameObject gameObject) //传入一个红点的配置路径 然后把红点对象传进去
        {
            this.gameObject = gameObject;   //对象赋值
            this.SetPath(path);             //把这个路径传入 父类RedDot 的SetPath 函数
        }

        public override void FreshView(int count)
        {
            UnityEngine.Debug.Log($"{this.path}  {count}");
            this.gameObject.SetActive(count > 0);
        }
    }

}
