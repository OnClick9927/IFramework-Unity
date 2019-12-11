/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-17
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework
{
    public class LogSetting : ScriptableObject
    {
        public List<LogEliminateItem> Infos = new List<LogEliminateItem>();
        public int LogLevel;
        public int WarnningLevel;
        public int ErrLevel;
        public bool Enable = true;
        public bool LogEnable = true;
        public bool WarnningEnable = true;
        public bool ErrEnable = true;
    }
    [System.Serializable]
    public class LogEliminateItem
    {
        public string Name;
        public string Path;
        public TextAsset Text;
    }
}
