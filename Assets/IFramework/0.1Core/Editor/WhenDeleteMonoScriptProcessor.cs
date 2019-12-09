/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.14f1
 *Date:           2019-09-11
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace IFramework
{
	class WhenDeleteMonoScriptProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
        {
            if (!AssetPath.EndsWith(".cs")) return AssetDeleteResult.DidNotDelete;
            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetPath);
            if (monoScript == null) return AssetDeleteResult.DidNotDelete;
            Type spType = monoScript.GetClass();
            if (spType == null || !spType.IsSubclassOf(typeof(MonoBehaviour))) return AssetDeleteResult.DidNotDelete;

            MonoBehaviour[] monos = Object.FindObjectsOfType(spType) as MonoBehaviour[];
            monos.ForEach((m) =>
            {
                Object.DestroyImmediate(m);
            });
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { @"Assets" });
            if (guids == null || guids.Length <= 0) return AssetDeleteResult.DidNotDelete;
            guids.ToList()
                 .ConvertAll((guid) => { return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)); })
                 .ForEach((o) => {
                     var cps = o.GetComponentsInChildren(spType, true);
                     if (cps != null && cps.Length > 0)
                     {
                         cps.ForEach((c) =>
                         {
                             Object.DestroyImmediate(c, true);
                         });
                         AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(o));
                         EditorUtility.SetDirty(o);
                     }

                 });
                     AssetDatabase.SaveAssets();
                     AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            return AssetDeleteResult.DidNotDelete;
        }

    }
}
