﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    public abstract class Game : MonoBehaviour
    {
        public Modules modules { get { return Launcher.modules; } }
        private void Awake()
        {
            transform.SetParent(Launcher.Instance.transform);
            Launcher.Instance.game = this;
        }

        public abstract void Init();
        public abstract void Startup();


    }
}
