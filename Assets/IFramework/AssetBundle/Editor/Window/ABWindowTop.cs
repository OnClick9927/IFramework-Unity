/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System;
using UnityEngine;

namespace IFramework.AB
{
    internal enum AbWindowSelectIonType
    {
        Collect,
        Build
    }
    internal class ABWindowTop 
    {
        private AbWindowSelectIonType type;
        private void DrawTopButton(Rect rect)
        {
            type=(AbWindowSelectIonType) FreshToolBar.OnGUI((bo) => {

                if (bo) ABWindow.Instance.ReFreashValue();
            }, !ABWindow.Instance.IsShowCollect, rect, (int)type, Enum.GetNames(typeof(AbWindowSelectIonType)));

            switch (type)
            {
                case AbWindowSelectIonType.Collect:
                    ABWindow.Instance.IsShowCollect  = true;
                    break;
                case AbWindowSelectIonType.Build:
                    ABWindow.Instance.IsShowCollect = false;
                    break;
            }
          
        }
        public void OnGUI( Rect worldRect)
        {
            DrawTopButton(worldRect);
        }

       
    }

}
