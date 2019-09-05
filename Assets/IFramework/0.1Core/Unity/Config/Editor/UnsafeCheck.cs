/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.IO;


namespace IFramework
{
	 class UnsafeCheck
	{
        [InitializeOnLoadMethod]
        public static void Check()
        {
            string path = Application.dataPath;
#if UNITY_2018_1_OR_NEWER
            path = path.CombinePath("csc.rsp");
            PlayerSettings.allowUnsafeCode = true;
#else
            path = path.CombinePath("mcs.rsp");
            string content = "-unsafe";
            if (File.Exists(path) && path.ReadText(System.Text.Encoding.Default) == content) return;
                path.WriteText(content, System.Text.Encoding.Default); 
            AssetDatabase.Refresh();
            IFEditorUtil.ReOpen();
#endif

        }
	}
}
