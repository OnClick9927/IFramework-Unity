/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        protected static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MonoSingletonCreator.CreateMonoSingleton<T>();
                    SingletonCollection.Set(instance);
                }
                return instance;
            }
        }

        protected virtual void OnSingletonInit() { }
        public virtual void Dispose()
        {

            Log.L("Dispose");

            if (MonoSingletonCreator.IsUnitTestMode)
            {
                var curTrans = transform;
                do
                {
                    var parent = curTrans.parent;
                    DestroyImmediate(curTrans.gameObject);
                    curTrans = parent;
                } while (curTrans != null);
                instance = null;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            instance = null;

        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

}
