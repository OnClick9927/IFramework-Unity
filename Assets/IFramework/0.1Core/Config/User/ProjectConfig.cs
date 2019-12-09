/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
	public partial class ProjectConfig
	{
        public const string infoName = "ProjectConfig";
        public static string NameSpace { get { return Info.NameSpace; } }
        public static string UserName { get { return Info.UserName; } }
        public static string Version { get { return Info.Version; } }
        public static string Description { get { return Info.Description; } }
        private static ProjectConfigInfo info;
        private static ProjectConfigInfo Info
        {
            get
            {
                if (info == null)
                {
                    info = UnityEngine.Resources.Load<ProjectConfigInfo>(infoName);
                }
                return info;
            }
        }

    }
}
