/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework
{
	public static class ObserveExtension_CS
	{
        public static void Publish<T>(T t, int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            ObserveManager.Publish<T>(t, code, args, param);
        }
        public static void Publish<T>(int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            ObserveManager.Publish<T>(code, args, param);
        }

        public static bool Subscribe<T>(this IObserver self) where T : IPublisher
        {
            return ObserveManager.Subscribe<T>(self);
        }
        public static bool Subscribe(this IObserver self, Type type)
        {
            return ObserveManager.Subscribe(type, self);
        }

        public static bool Unsubscribe<T>(this IObserver self) where T : IPublisher
        {
            return ObserveManager.Unsubscribe<T>(self);
        }
        public static bool Unsubscribe(this IObserver self, Type type)
        {
            return ObserveManager.Unsubscribe(type, self);
        }
    }
}
