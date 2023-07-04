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
    public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = MonoSingletonCreator.CreateMonoSingleton<T>();
                    SingletonCollection.Set(instance);
                }

                return instance;
            }
        }

        public static void Dispose()
        {
            if (MonoSingletonCreator.IsUnitTestMode)
            {
                Object.DestroyImmediate(instance.gameObject);
            }
            else
            {
                Object.DestroyImmediate(instance.gameObject);
            }

            instance = null;
        }
    }

}
