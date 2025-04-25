/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    [MonoSingletonPath(nameof(IFramework.TimeEx))]
    [UnityEngine.AddComponentMenu("")]
    class TimerScheduler_Runtime : MonoSingleton<TimerScheduler_Runtime>
    {
        public TimerScheduler scheduler;

        protected override void OnSingletonInit()
        {
            base.OnSingletonInit();
            scheduler = new TimerScheduler();
        }
        private void Update()
        {
            scheduler.Update();
        }
        protected override void OnDestroy()
        {
            scheduler.KillTimers();
            base.OnDestroy();
        }


    }

}
