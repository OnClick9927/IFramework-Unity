/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Singleton
{
    public abstract class MonoSingletonPropertyClass<T> : UnityEngine.MonoBehaviour, ISingleton where T : MonoSingletonPropertyClass<T>
    {
        protected static T Instance { get { return MonoSingletonProperty<T>.Instance; } }
        protected MonoSingletonPropertyClass() { }
        protected abstract void OnSingletonInit();
        public virtual void Dispose()
        {
            MonoSingletonProperty<T>.Dispose();
        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }
}
