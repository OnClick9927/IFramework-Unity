/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.UI
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/ScriptMark")]
    public class ScriptMark : MonoBehaviour
    {
        [HideInInspector] public string fieldName;
        [HideInInspector] public string fieldType;
    }
}
