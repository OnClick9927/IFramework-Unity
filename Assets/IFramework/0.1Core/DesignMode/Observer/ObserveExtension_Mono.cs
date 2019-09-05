/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
	public class ObserveExtension_Mono
	{
        public static void DelayPublish<T>(T t, int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            Loom.RunOnMainThread(() => ObserveManager.Publish<T>(t, code, args, param));
        }
        public static void DelayPublish<T>(int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            Loom.RunOnMainThread(() => ObserveManager.Publish<T>(code, args, param));
        }
    }
}
