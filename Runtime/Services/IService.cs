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
    interface IService
    {
        string Name { get; }
        void OnQuit(IServiceCollection services);
        void OnUse(IServiceCollection services);
    }

}
