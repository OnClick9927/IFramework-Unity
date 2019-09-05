/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.Example
{
    internal class LogExample : MonoBehaviour
    {
        [ContextMenu("Hhh")]
        public void Say()
        {
            Log.L("sa");
            Debug.Log("515");

        }
        private void Awake()
        {
            Say();
        }
    }
}
