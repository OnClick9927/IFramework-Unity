/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
namespace IFramework
{
    [Serializable]
    class FrameworkConfigInfo : ScriptableObject
    {
        public string FrameWorkPath;
        public string CorePath;
        public string CoreEditorPath;
        public string UtilPath;
        public string EditorPath;

    }
    [UnityEditor.CustomEditor(typeof(FrameworkConfigInfo))]
    class FrameworkConfigInfoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;

            base.OnInspectorGUI();
        }

    }
}
