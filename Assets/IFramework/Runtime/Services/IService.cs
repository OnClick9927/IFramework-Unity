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
    public interface IService : IServiceProvider
    {
        string Name { get; set; }

        internal void OnEnter(IServiceCollection serviceCollection);
        internal IServiceCollection GetServices();

        internal void OnQuit(IServiceCollection services);
        internal void OnUse(IServiceCollection services);
    }

}
