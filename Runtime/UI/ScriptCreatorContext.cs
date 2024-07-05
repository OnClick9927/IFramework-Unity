/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework.UI
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class ScriptCreatorContext : MonoBehaviour
    {
        [HideInInspector] public bool containsChildren;
        [HideInInspector] public List<string> ignorePaths = new List<string>();
    }
}
