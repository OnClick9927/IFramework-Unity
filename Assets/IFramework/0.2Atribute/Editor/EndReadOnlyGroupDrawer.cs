/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(EndReadOnlyGroupAttribute))]
    internal class EndReadOnlyGroupDrawer : DecoratorDrawer
    {

        public override float GetHeight() { return 0; }

        public override void OnGUI(Rect position)
        {
            GUI.enabled = true;
          //  EditorGUI.EndDisabledGroup();
        }

    }
}
