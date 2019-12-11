/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-11
 *Description:    Description
 *History:        2019-12-11--
*********************************************************************************/
using System.Collections;
using IFramework;
using IFramework.Utility;

namespace IFramework_Demo
{
    class TestLife : FrameworkLifeObject
    {
        protected override void OnFrameworkDispose()
        {
            base.OnFrameworkDispose();
            Log.L("OnFrameworkDispose");

        }
        protected override void OnFrameworkInit()
        {
            base.OnFrameworkInit();
            Log.L("OnFrameworkInit");
            Coroutine.StartCoroutine(wait2());
        }
        IEnumerator wait()
        {
            Log.L("wait Go");
            yield return new WaitForSeconds(2);
            Log.L("wait end");

        }
        IEnumerator wait1()
        {
            Log.L("wait1 Go");
            yield return wait();
            Log.L("wait1 end");

        }
        IEnumerator wait2()
        {
            Log.L("wait2 Go");
            yield return wait1();
            Log.L("wait2 end");
            Log.L("wait1 Go");
            yield return wait();
            Log.L("wait1 end");
        }
        protected override void OnDispose()
        {
            base.OnDispose();
        }
        protected override void OnUpdate()
        {
        }
    }

    public class ExampleFrameworkLifeTime : UnityEngine.MonoBehaviour
    {
        void Start()
        {
            TestLife tt = new TestLife();
            Framework.Reinit();

        }

        // Update is called once per frame
        void Update()
        {
            Framework.Update();
        }
        private void OnDestroy()
        {

            Framework.Dispose();
        }
    }
}
