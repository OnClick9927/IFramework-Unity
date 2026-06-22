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
    public interface IServiceCollection
    {
        string Name {  get; }
        T UseService<T>(T service, string name = "") where T : ServiceBase;
        IReadOnlyList<ServiceBase> GetServices<T>() where T : ServiceBase;
        T GetService<T>(string name = "") where T : ServiceBase;
    }

}
