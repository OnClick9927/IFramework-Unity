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
    [MonoSingletonPath(nameof(IFramework.Tween))]
    [UnityEngine.AddComponentMenu("")]
    class TweenScheduler_Runtime : MonoSingleton<TweenScheduler_Runtime>
    {
        public TweenScheduler scheduler;

        protected override void OnSingletonInit()
        {
            base.OnSingletonInit();
            scheduler = new TweenScheduler();
        }
        private void Update()
        {
            scheduler.Update();
        }
        protected override void OnDestroy()
        {
            scheduler.CancelAllTween();
            base.OnDestroy();
        }


    }





}