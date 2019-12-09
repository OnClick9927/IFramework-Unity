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
	public class DeleteMissingScripts
	{
        [MenuItem("IFramework/Tool/Remove Missing Scripts")]
        static void CleanupMissingScript()
        {
            GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

            int r;
            int j;
            for (int i = 0; i < pAllObjects.Length; i++)
            {
                if (pAllObjects[i].hideFlags == HideFlags.None)//HideFlags.None 获取Hierarchy面板所有Object
                {
                    var components = pAllObjects[i].GetComponents<Component>();
                    var serializedObject = new SerializedObject(pAllObjects[i]);
                    var prop = serializedObject.FindProperty("m_Component");
                    r = 0;

                    for (j = 0; j < components.Length; j++)
                    {
                        if (components[j] == null)
                        {
                            prop.DeleteArrayElementAtIndex(j - r);
                            r++;
                        }
                    }

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
