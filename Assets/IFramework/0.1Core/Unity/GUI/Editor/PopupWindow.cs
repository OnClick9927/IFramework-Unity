/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
    public class IFPopupWindow : PopupWindowContent
    {
        public Vector2 windowSize;
        public override void OnClose()
        {
           
        }
        public override Vector2 GetWindowSize()
        {
            return windowSize;
        }
        public override void OnOpen()
        {
           
        }
        public override void OnGUI(Rect rect)
        {

        }
    }
    
}
