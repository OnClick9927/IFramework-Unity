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

        public RedActiveDot(string path, GameObject gameObject) 
        {
            this.gameObject = gameObject;   
            this.SetPath(path);             
        }

        public override void FreshView(int count)
        {
            this.gameObject.SetActive(count > 0);
        }
    }

}
