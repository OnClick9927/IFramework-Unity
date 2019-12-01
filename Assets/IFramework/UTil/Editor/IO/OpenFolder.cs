/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-01-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
namespace IFramework
{
	public class OpenFolder
	{
        public static void Open(string title, string folder, string defaultName)
        {
            EditorUtility.OpenFolderPanel(title, folder, defaultName);
        }
        [MenuItem("IFramework/Folder/Open DocPath")]
        public static void OpenDoc()
        {
            Open("Document", Application.persistentDataPath, "doc");
        }
        [MenuItem("IFramework/Folder/Open StreamingPath")]
        public static void OpenStreaming()
        {
            Open("Streaming", Application.streamingAssetsPath, "stream");
        }
        [MenuItem("IFramework/Folder/Open DataPath")]
        public static void OpenDataPath()
        {
            Open("DataPath", Application.dataPath, "data");
        }
#if UNITY_2018
        [MenuItem("IFramework/Folder/Open ConsoleLogPath")]
        public static void OpenConsoleLog()
        {
            Open("ConsoleLog", Application.consoleLogPath, "console");
        }
#endif
        [MenuItem("IFramework/Folder/Open TemporaryCachePath")]
        public static void OpenTemporary()
        {
            Open("Temporary", Application.temporaryCachePath, "temporary");
        }
    }
}
