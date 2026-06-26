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
    public static class IServiceEx
    {
        public static T GetRequiredService<T>(this IServiceProvider services, string name = "") where T : class, IService
        {
            T service = services.GetService<T>();
            if (service == null)
            {
                Log.FE($"{typeof(T)} Service is null : {name}->Please Use Method {nameof(IServiceCollection)}.{nameof(IServiceCollection.Use)}xxx");
            }
            return service;
        }
    }
}
