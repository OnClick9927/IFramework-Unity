/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
	public static class ObserveExtension_Mono
	{
        public static T DelayPublish<T>(this T t, int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            Loom.RunOnMainThread(() => ObserveManager.Publish<T>(t, code, args, param));
            return t;
        }

    }
}
