/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
namespace IFramework
{
    [Serializable]
	public class ProjectConfigInfo:UnityEngine.ScriptableObject
	{
        public string NameSpace;
        public string UserName;
        public string Version;
        public string Description;
        public ProjectConfigInfo()
        {
            UserName = "OnClick";
            NameSpace = "IFramework_Demo";
            Version = "0.0.1";
            Description = "Description";
        }
    }
}
