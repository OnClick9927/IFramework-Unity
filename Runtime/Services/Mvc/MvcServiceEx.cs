/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
namespace IFramework
{
    public static class MvcServiceEx
    {
        public static IServiceCollection UseMvc(this IServiceCollection services, IReadOnlyList<ModelBase> models, IReadOnlyList<CtrlBase> ctrls)
        {
            MvcService service = new MvcService(models, ctrls);
            services.UseService(service);
            return services;
        }
        public static IReadOnlyList<ModelBase> GetModels(this IServiceCollection services)
        {
            var service = services.GetService<MvcService>();
            return service?.models;
        }
        public static IReadOnlyList<CtrlBase> GetCtrls(this IServiceCollection services)
        {
            var service = services.GetService<MvcService>();
            return service?.ctrls;
        }
        public static T GetCtrl<T>(this IServiceCollection services) where T : CtrlBase
        {
            var service = services.GetValue<T>(string.Empty);
            return service;
        }
        public static T GetModel<T>(this IServiceCollection services) where T : ModelBase
        {
            var service = services.GetValue<T>(string.Empty);
            return service;
        }
    }
}
