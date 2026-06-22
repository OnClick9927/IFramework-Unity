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
    public abstract class ServiceBase : IService
    {
        public string Name { get; internal set; }
        protected abstract void OnUse(IServiceCollection services);
        protected abstract void OnQuit(IServiceCollection services);

        void IService.OnQuit(IServiceCollection services)
        {
            OnQuit(services);
        }

        void IService.OnUse(IServiceCollection services)
        {
            OnUse(services);
        }
    }

}
