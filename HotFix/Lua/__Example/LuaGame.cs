/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.115
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.Hotfix.Lua
{
    public class AssetsLoader : IXLuaLoader
    {
        public static string projectScriptsPath
        {
            get { return Application.dataPath.CombinePath("Project/Lua"); }
        }
        public byte[] load(ref string path)
        {
            if (path.EndsWith(".lua"))
                path = path.Replace(".lua", "");
            path = path.Replace(".", "/");
            string filepath = $"{path}.lua";
            var textAsset = Resources.Load<TextAsset>(filepath);
            if (textAsset != null)
                return textAsset.bytes;
            filepath = projectScriptsPath.CombinePath(filepath + ".txt").ToAssetsPath();
#if UNITY_EDITOR
            var handle = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(filepath);
            return handle.bytes;
#endif
            return null;
        }
    }

    public class LuaGame : Game
    {
        public class UnityModules
        {
            public XLuaModule Lua { get { return Launcher.modules.GetModule<XLuaModule>(); } }
        }

        public UnityModules unityModules = new UnityModules();

        public override void Init()
        {

        }
        public override void Startup()
        {
            unityModules.Lua.AddLoader(new AssetsLoader());
            new XluaMain(unityModules.Lua);
        }
    }
}
