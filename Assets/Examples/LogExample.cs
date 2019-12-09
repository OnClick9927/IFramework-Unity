/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using UnityEngine;

namespace IFramework_Demo
{
    internal class LogExample : MonoBehaviour
    {
        [ContextMenu("Log")]
        public void Say()
        {
            Log.L("sa         with IFramework");
            Debug.Log("515    with unity");

        }
        private void Awake()
        {
            Say();
        }
    }
}
