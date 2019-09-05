/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
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
        public bool IsUseNameSpace;
        public string NameSpace;
        public string UserName;
        public string Version;
        public string Description;
        public ProjectConfigInfo()
        {
            UserName = "OnClick";
            IsUseNameSpace = true;
            NameSpace = "IFramework";
            Version = "1.0";
            Description = "Description";
        }
    }
}
