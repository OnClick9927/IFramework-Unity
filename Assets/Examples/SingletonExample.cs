/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
namespace IFramework_Demo
{
	public class SingletonExample
	{
        public class CS_single : Singleton<CS_single>
        {

        }
        public class Mono_single : MonoSingleton<Mono_single>
        {

        }
    }
}
