/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework
{
    public interface IValueService:IService
    {
        object Get(Type type, string name);
        void Inject(object inject);
        void Register<TBaseType, TType>() where TType : TBaseType, new();
        object Register(Type type, object instance, string name);
    }
}