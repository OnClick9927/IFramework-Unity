/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
namespace IFramework
{
    [ScriptedImporter(1, "lua")]
    public class LuaImporter : ScriptedImporter
    {
       
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllText(ctx.assetPath);
            TextAsset asset = new TextAsset(text);
            ctx.AddObjectToAsset("text", asset);
            ctx.SetMainObject(asset);
        }
    }
}
#endif