/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;
using IFramework;

namespace IFramework_Demo
{
    public class Panel2 :  UIPanel
    {
        protected override void OnLoad(UIEventArgs arg)
        {
            Log.L(GetType() + " OnLoad " + arg.isInspectorBtn);
            //UIManager.Instance.SetParent(this);
            gameObject.SetActive(true);
        }
        protected override void OnPop(UIEventArgs arg)
        {
            Log.L(GetType() + " OnPop " + arg.isInspectorBtn);
            gameObject.SetActive(false);

        }

        protected override void OnPress(UIEventArgs arg)
        {
            Log.L(GetType() + " OnPress " + arg.isInspectorBtn);
            gameObject.SetActive(false);

        }

        protected override void OnTop(UIEventArgs arg)
        {
            Log.L(GetType() + " OnTop " + arg.isInspectorBtn);
            gameObject.SetActive(true);

        }

        protected override void OnCacheClear(UIEventArgs arg)
        {
            Log.L(GetType() + " OnCacheClear " + arg.isInspectorBtn);
            if (UIManager.IsInUse(this)) return;
            {

            }
            Destroy(this.gameObject);
        }
    }
}
