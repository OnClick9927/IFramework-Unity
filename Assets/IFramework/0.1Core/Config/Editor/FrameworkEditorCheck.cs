/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-09
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    class FrameworkEditorCheck
    {
        [InitializeOnLoadMethod]
        public static void Check()
        {
            Framework.Init();
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
