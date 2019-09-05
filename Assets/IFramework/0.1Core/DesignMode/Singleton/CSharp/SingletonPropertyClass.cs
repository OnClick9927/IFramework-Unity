/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public abstract class SingletonPropertyClass<T> : ISingleton where T : SingletonPropertyClass<T>
    {
        protected static T Instance { get { return SingletonProperty<T>.Instance; } }
        protected SingletonPropertyClass() { }
        protected virtual void OnSingletonInit()
        {

        }
        public virtual void Dispose()
        {
            SingletonProperty<T>.Dispose();
        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

}
