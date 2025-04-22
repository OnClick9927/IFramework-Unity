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
using static IFramework.UI.UIModuleWindow;

namespace IFramework.UI
{
    [System.Serializable]
    public class ScriptPathCollection
    {
        [System.Serializable]
        public class Seg
        {
            public string prefab;
            public string ScriptPath;
            public List<string> Paths;
        }

        [UnityEngine.SerializeField] private List<Seg> segs = new List<Seg>();
        private static ScriptPathCollection __context;
        private Seg Get(string prefab)
        {
            var find = segs.Find(x => x.prefab == prefab);
            if (find == null)
            {
                find = new Seg() { prefab = prefab };
                segs.Add(find);
            }
            return find;
        }
        private static ScriptPathCollection context_scripts
        {
            get
            {
                if (__context == null)
                {

                    __context = EditorTools.GetFromPrefs<ScriptPathCollection>(nameof(ScriptPathCollection), false);
                    if (__context == null)
                        __context = new ScriptPathCollection();
                }
                return __context;
            }
        }
        internal static void SaveScriptsData()
        {
            EditorTools.SaveToPrefs(context_scripts, nameof(ScriptPathCollection), false);
        }

        private static Seg GetSeg(string prefab) => context_scripts.Get(prefab);
        public static Seg GetSeg(PanelCollection.Data data) => GetSeg(data.path);

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
