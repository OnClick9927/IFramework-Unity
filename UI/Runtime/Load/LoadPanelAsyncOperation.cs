/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using UnityEngine;

namespace IFramework.UI
{
    public class LoadPanelAsyncOperation : UIAsyncOperation<UIPanel>
    {
        public Action<string, UIPanel> callback;
        public string path;
        public RectTransform parent;
        public new void SetToDefault()
        {
            base.SetToDefault();
            path = null;
        }
    }
}
