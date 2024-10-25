/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IFramework.Lua
{
    [CreateAssetMenu]
    public class LuaEnvConfig : UnityEngine.ScriptableObject
    {
        public string rootPath;
    }


}
