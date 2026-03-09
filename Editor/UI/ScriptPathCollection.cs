/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using static IFramework.UI.UIEditorPrefs;

namespace IFramework.UI
{
    [System.Serializable]
    public class ScriptPathCollection
    {
        private static ScrpitSeg Get(string prefab)
        {
            var find = context.segs.Find(x => x.prefab == prefab);
            if (find == null)
            {
                find = new ScrpitSeg() { prefab = prefab };
                context.segs.Add(find);
            }
            return find;
        }
        public static ScrpitSeg GetSeg(PanelCollection.Data data) => GetSeg(data.path);

        internal static void SaveScriptsData()
        {
            UIEditorPrefs.Save();
        }

        private static ScrpitSeg GetSeg(string prefab) => Get(prefab);

        internal static bool CollectScripPaths(PanelCollection collect, UIGenCode tab)
        {
            var paths = AssetDatabase.FindAssets((tab as UIGenCode).GetScriptFitter())
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .ToList();
            bool change = false;



            for (int i = 0; i < collect.datas.Count; i++)
            {
                var data = collect.datas[i];
                var s_name = (tab as UIGenCode).GetPanelScriptName(data.name);
                var find = paths.FindAll(x => x.EndsWith("/" + s_name)) ?? new List<string>();

                var seg = GetSeg(data);
                seg.Paths = find;

            SetEmpty:
                if (string.IsNullOrEmpty(seg.ScriptPath))
                {
                    if (find.Count > 0)
                    {
                        change = true;
                        seg.ScriptPath = find[0];
                    }
                }
                else
                {
                    if (!find.Contains(seg.ScriptPath))
                    {
                        seg.ScriptPath = string.Empty;
                        change = true;
                        goto SetEmpty;
                    }
                }
            }
            SaveScriptsData();
            return change;
        }

    }
}
