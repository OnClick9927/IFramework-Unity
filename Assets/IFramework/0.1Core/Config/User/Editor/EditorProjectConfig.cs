/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEditor;
using System.Collections.Generic;
using System.Linq;
namespace IFramework
{
    public partial class EditorProjectConfig
    {
        public static string NameSpace { get { return Info.NameSpace; } }
        public static string UserName { get { return Info.UserName; } }
        public static string Version { get { return Info.Version; } }
        public static string Description { get { return Info.Description; } }
        private static ProjectConfigInfo info;
        private static ProjectConfigInfo Info
        {
            get
            {
                if (info == null) LoadProjectInfo();
                return info;
            }
        }
        public static string ProjectConfigInfoPath = FrameworkConfig.CorePath.CombinePath("Config/Resources/" + ProjectConfig.infoName + ".asset").ToRegularPath();
        private static void LoadProjectInfo()
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}",typeof(ProjectConfigInfo)), new string[] { @"Assets" });
            List<ProjectConfigInfo> stos = guids.ToList()
                .ConvertAll((guid) => { return AssetDatabase.LoadAssetAtPath<ProjectConfigInfo>(AssetDatabase.GUIDToAssetPath(guid)); });
            if (stos.Count == 0 || !AssetDatabase.GetAssetPath(stos[0]).Equals(ProjectConfigInfoPath)) CreateNewSto(stos);
            else
            {
                for (int i = 1; i < stos.Count; i++)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(stos[i]));
                info = stos[0];
            }
        }
        private static void CreateNewSto(List<ProjectConfigInfo> stos)
        {
            if (stos.Count > 0)
                stos.ReverseForEach((sto) => { AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sto)); });
            info = ScriptableObj.Create<ProjectConfigInfo>(ProjectConfigInfoPath);
        }
    }
}
