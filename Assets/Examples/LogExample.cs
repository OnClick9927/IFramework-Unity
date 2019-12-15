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
    internal class LogExample : MonoBehaviour,ILog
    {
        [ContextMenu("Log ")]
        public void Say()
        {
            string world = "Hello  World";
            //Log.L(world);
            //world.Log();
            //this.Log(world);
            //Log.W(world);
            //Log.E(world);


            Log.L("sa         with IFramework");
            Debug.Log("515    with unity");

        }
        private void Awake()
        {
            Say();
        }
    }
}
