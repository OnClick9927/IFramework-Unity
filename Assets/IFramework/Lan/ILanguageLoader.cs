/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LanguageLoaderAttribute : Attribute { }
    public interface ILanguageLoader:IDisposable
    {
        List<LanPair> Load();
    }
    [LanguageLoader]
    public class DefaltLoader : ILanguageLoader
    {
        private LanGroup lanGroup;
        public void Dispose()
        {
            Resources.UnloadAsset(lanGroup);
        }

        public List<LanPair> Load()
        {
            lanGroup = Resources.Load<LanGroup>("LanGroup");
            return lanGroup.lanPairs;
        }
    }
}
