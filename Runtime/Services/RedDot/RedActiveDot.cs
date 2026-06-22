/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    public class RedActiveDot : RedDot
    {
        public GameObject gameObject;

        public override void FreshView(int count)
        {
            this.gameObject.SetActive(count > 0);
        }
    }
}
