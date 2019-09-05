/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
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
	internal class FrameworkConfigInfo:UnityEngine.ScriptableObject
	{
         public string FrameWorkName = "IFramework";
         public string Author= "OnClick";
         public string Version;
         public string FrameWorkPath;
         public string UnityCorePath;
         public string EditorCorePath;
        
         public string UtilPath;
         public string EditorPath;
        
        

         public string Description;


        public void OnEnable()
        {
            Version = "1.0";
            Description = FrameWorkName;
        }
       
    }
    [UnityEditor.CustomEditor(typeof(FrameworkConfigInfo))]
    class FrameworkConfigInfoEditor:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;

            base.OnInspectorGUI();
        }

    }
}
