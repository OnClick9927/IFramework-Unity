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
            //services
            MvcService service = new MvcService(models, ctrls);
            services.Use<IMvcService>(service);
            return services;
        }
        public static IServiceProvider EnterMvc(this IServiceProvider services) => services.EnterService<IMvcService>();
        public static IMvcService Mvc(this IServiceProvider services) =>
          services.GetRequiredService<MvcService>();
        public static T GetCtrl<T>(this IMvcService mvc) where T : CtrlBase => mvc.Values().Get<T>();
        public static T GetModel<T>(this IMvcService mvc) where T : ModelBase => mvc.Values().Get<T>();
    }
}
