/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using static IFramework.UI.UIModuleWindow;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace IFramework.UI
{
    [System.Serializable]
    class EditorPanelCollectionPlans
    {
        [SerializeField] private int _index = 0;

        [SerializeField] private List<EditorPanelCollectionPlan> _plans = new List<EditorPanelCollectionPlan>();
        const string res = "Resources";
        private static EditorPanelCollectionPlans _context;
        private static EditorPanelCollectionPlans context
        {
            get
            {

                if (_context == null)
                {
                    _context = EditorTools.GetFromPrefs<EditorPanelCollectionPlans>(nameof(EditorPanelCollectionPlans), false);
                    if (_context == null)
                        _context = new EditorPanelCollectionPlans();
                    if (_context._plans.Count == 0)
                        NewPlan();
                    Save();
                }
                return _context;
            }
        }


        private static void Save() => EditorTools.SaveToPrefs(_context, nameof(EditorPanelCollectionPlans), false);


        public static List<EditorPanelCollectionPlan> plans => context._plans;
        public static EditorPanelCollectionPlan plan_current => plans[planIndex];
        public static int planIndex
        {
            get => context._index;
            set
            {
                if (context._index != value)
                {
                    context._index = value;
                    Save();
                }
            }
        }

        public static void SaveCurrentPlan(string name, string GenPath, string CollectPath, string ScriptGenPath,
        string scriptName, string configName, int typeIndex)
        {
            var _plan = plan_current;
            if (_plan.name != name || _plan.ConfigGenPath != GenPath || _plan.PanelCollectPath != CollectPath
                || _plan.ConfigName != configName
                || _plan.ScriptGenPath != ScriptGenPath || _plan.ScriptName != scriptName || _plan.typeIndex != typeIndex)
            {
                _plan.name = name;
                _plan.ConfigGenPath = GenPath;
                _plan.PanelCollectPath = CollectPath;
                _plan.ScriptGenPath = ScriptGenPath;
                _plan.ScriptName = scriptName;
                _plan.typeIndex = typeIndex;
                _plan.ConfigName = configName;
                Save();
            }
        }

        public static void DeletePlan()
        {
            if (plans.Count == 1)
            {
                Debug.LogError("Must Exist One Plan");
                return;
            }
            plans.RemoveAt(planIndex);
            planIndex = 0;
        }
        public static void NewPlan()
        {
            plans.Add(new EditorPanelCollectionPlan()
            {
                name = DateTime.Now.ToString("yy_MM_dd_hh_mm_ss"),
                PanelCollectPath = "Assets",
                ConfigGenPath = "Assets",
                ScriptGenPath = "Assets",
                ConfigName = "UICollect",
                ScriptName = "PanelNames",
            });
            planIndex = plans.Count - 1;
        }
        public static void GenPlans()
        {
            for (int i = 0; i < plans.Count; i++)
                GenPlan(plans[i], Collect(plan_current));
        }

        public static void GenPlan(EditorPanelCollectionPlan plan, PanelCollection collect)
        {
            var selectType = plan.GetSelectType();
            GetUIGencode(selectType).GenPanelNames(collect, plan.ScriptGenPath, plan.ScriptName);
            GenCollectionJson(plan, collect);
        }
        private static void GenCollectionJson(EditorPanelCollectionPlan path, PanelCollection collect)
        {
            File.WriteAllText(path.collectionJsonPath, JsonUtility.ToJson(collect, true));
            AssetDatabase.Refresh();
        }


        static UIGenCode GetUIGencode(Type type) => Activator.CreateInstance(type) as UIGenCode;
        public static PanelCollection Collect(EditorPanelCollectionPlan plan)
        {
            string path = plan.collectionJsonPath;
            PanelCollection collect = null;
            if (!File.Exists(path))
                collect = new PanelCollection();
            else
                collect = JsonUtility.FromJson<PanelCollection>(File.ReadAllText(path));
            var paths = AssetDatabase.FindAssets("t:prefab", new string[] { plan.PanelCollectPath })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => AssetDatabase.LoadAssetAtPath<UIPanel>(path) != null).ToList();





            int remove = collect.datas.RemoveAll(x => !paths.Contains(x.path));
            var _new = paths.FindAll(path => !collect.datas.Any(data => data.path == path));




            _new.ForEach(path =>
            {
                var isResourcePath = path.Contains(res);
                if (isResourcePath)
                {
                    var index = path.IndexOf(res);
                    path = path.Substring(index + res.Length + 1)
                            .Replace(".prefab", "");
                }
                collect.datas.Add(new PanelCollection.Data()
                {
                    isResourcePath = isResourcePath,
                    path = path,
                    layer = 0,
                    fullScreen = false,
                });
            });
            collect.datas.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.name, y.name));
            var change = ScriptPathCollection.CollectScripPaths(collect, GetUIGencode(plan.GetSelectType()));
            if (remove != 0 || _new.Count > 0 || change)
                GenCollectionJson(plan, collect);
            return collect;
        }

    }
}
