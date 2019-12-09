/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using UnityEngine;
namespace IFramework_Demo
{
	public class LanExample:MonoBehaviour
	{
        [LanguageKey]
        public string key;
        LanCtrl.LanObserver observer;
        private void Awake()
        {
            observer= LanCtrl.CreatLanObserver(key, SystemLanguage.Icelandic).ObserveEvent((key, lan, val) => { Log.E(val); });
        }
        int index;
        private void Update()
        {
            index = ++index % 40;
            LanCtrl.Lan = (SystemLanguage)index;
        }
        private void OnDestroy()
        {
            observer.Dispose();
        }
    }
}
