/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
namespace IFramework
{
    public class ScriptFinder
    {
        [MenuItem("CONTEXT/MonoBehaviour/IFramework.FindScript")]
        public static void FindScript( MenuCommand command)
        {
            Selection.activeObject = MonoScript.FromMonoBehaviour(command.context as MonoBehaviour);
        }
	}
}
