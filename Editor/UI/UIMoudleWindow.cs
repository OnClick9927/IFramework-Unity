/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

using System.IO;
using static IFramework.EditorTools;

namespace IFramework.UI
{

    [EditorWindowCache("UIModule")]
    public partial class UIModuleWindow : EditorWindow
    {

        public class UICollectData
        {

            [System.Serializable]
            public class Plan
            {
                public string ConfigGenPath;
                public string PanelCollectPath;
                public string ScriptGenPath;
                public string ScriptName;
                public string name;

                private static string[] _typeNames, _shortTypes;
                public static string[] typeNames
                {
                    get
                    {
                        if (_typeNames == null)
                            Enable();
                        return _typeNames;
                    }
                }
                public static string[] shortTypes
                {
                    get
                    {
                        if (_shortTypes == null)
                            Enable();
                        return _shortTypes;
                    }
                }
                public static Type[] __types;
                public static Type[] types
                {
                    get
                    {
                        if (__types == null)
                        {
                            Enable();
                        }
                        return __types;
                    }
                }
                public int typeIndex;
                public static Type baseType = typeof(UIGenCode);
                public string ConfigName;

                private static void Enable()
                {
                    var list = EditorTools.GetSubTypesInAssemblies(baseType)
                   .Where(type => !type.IsAbstract);
                    __types = list.ToArray();
                    _typeNames = list.Select(type => type.FullName).ToArray();
                    _shortTypes = list.Select(type => type.Name).ToArray();
                }
                public Type GetSelectType()
                {
                    var type_str = typeNames[typeIndex];
                    Type type = types.FirstOrDefault(x => x.FullName == type_str);

                    return type;
                }


            }


            [System.Serializable]
            private class Plans
            {
                public int index = 0;

                public List<Plan> plans = new List<Plan>();
            }
            const string res = "Resources";

            public static List<Plan> plans { get => context.plans; }

            private static Plans _context;
            private static void SavePlansData()
            {
                EditorTools.SaveToPrefs(_context, nameof(Plans), false);
            }
            private static Plans context
            {
                get
                {

                    if (_context == null)
                    {
                        _context = EditorTools.GetFromPrefs<Plans>(nameof(Plans), false);
                        if (_context == null)
                            _context = new Plans();
                        if (_context.plans.Count == 0)
                            NewPlan();
                        SavePlansData();
                    }
                    return _context;
                }
            }


            public static Plan plan => plans[planIndex];
            public static int planIndex
            {
                get => context.index;
            }
            public static void SetPlanIndex(int value)
            {
                if (context.index != value)
                {
                    context.index = value;
                    SavePlansData();
                }
            }

            internal static void DeletePlan()
            {
                if (plans.Count == 1)
                {
                    window.ShowNotification(new GUIContent("Must Exist One Plan"));
                    return;
                }
                plans.RemoveAt(planIndex);
                SetPlanIndex(0);
                SavePlansData();
            }
            internal static void NewPlan()
            {
                plans.Add(new Plan()
                {
                    name = DateTime.Now.ToString("yy_MM_dd_hh_mm_ss"),
                    PanelCollectPath = "Assets",
                    ConfigGenPath = "Assets",
                    ScriptGenPath = "Assets",
                    ConfigName = "UICollect",
                    ScriptName = "PanelNames",
                });
                SetPlanIndex(plans.Count - 1);
                SavePlansData();
            }

            private static string UICollectPath { get { return plan.ConfigGenPath.CombinePath($"{plan.ConfigName}.json"); } }


            public static void SavePlans()
            {
                var index = planIndex;
                for (int i = 0; i < plans.Count; i++)
                {
                    SetPlanIndex(i);
                    SavePlan(Collect());
                }
                SetPlanIndex(index);
            }

            public static void SavePlan(PanelCollection collect)
            {
                var selectType = plan.GetSelectType();
                foreach (var item in window._tabs.Values)
                {
                    if (item.GetType() == selectType)
                    {
                        (item as UIGenCode).GenPanelNames(collect, plan.ScriptGenPath, plan.ScriptName);
                    }
                }
                SaveConfig(collect);
            }
            private static void SaveConfig(PanelCollection collect)
            {
                File.WriteAllText(UICollectPath, JsonUtility.ToJson(collect, true));

                AssetDatabase.Refresh();
            }

            public static List<string> GetFitScriptPaths(string name) => name_ScriptMap.TryGetValue(name, out var list) ? list : null;

            private static Dictionary<string, List<string>> name_ScriptMap;


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

                public List<Seg> segs = new List<Seg>();
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
                public static void SaveScriptsData()
                {
                    EditorTools.SaveToPrefs(__context, nameof(ScriptPathCollection), false);
                }

                private static Seg GetSeg(string prefab) => context_scripts.Get(prefab);
                public static Seg GetSeg(PanelCollection.Data data) => GetSeg(data.path);
            }


            private static bool CollectScripPaths(PanelCollection collect)
            {
                name_ScriptMap = new Dictionary<string, List<string>>();
                var tab = window.GetTab(UICollectData.plan.GetSelectType());
                var paths = AssetDatabase.FindAssets(tab.GetScriptFitter())
                    .Select(x => AssetDatabase.GUIDToAssetPath(x))
                    .ToList();
                bool change = false;



                for (int i = 0; i < collect.datas.Count; i++)
                {
                    var data = collect.datas[i];
                    var s_name = tab.GetPanelScriptName(data.name);
                    var find = paths.FindAll(x => x.EndsWith("/" + s_name)) ?? new List<string>();
                    name_ScriptMap.Add(data.name, find);

                    var seg = ScriptPathCollection.GetSeg(data);
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
                ScriptPathCollection.SaveScriptsData();
                return change;
            }
            public static PanelCollection Collect()
            {
                string path = UICollectPath;
                PanelCollection collect = null;
                if (!File.Exists(path))
                    collect = new PanelCollection();
                else
                    collect = JsonUtility.FromJson<PanelCollection>(File.ReadAllText(path));

                var paths = AssetDatabase.FindAssets("t:prefab", new string[] { plan.PanelCollectPath })
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Where(path => AssetDatabase.LoadAssetAtPath<UIPanel>(path) != null).ToList();

                int remove = collect.datas.RemoveAll(x => !paths.Contains(x.path));
                var _new = paths.FindAll(path => collect.datas.Find(data => data.path == path) == null);

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
                      });
                  });
                var change = CollectScripPaths(collect);

                if (remove != 0 || _new.Count > 0 || change)
                    SaveConfig(collect);
                return collect;
            }





            internal static void SavePlan(string name, string GenPath, string CollectPath, string ScriptGenPath,
                string scriptName, string configName, int typeIndex)
            {
                if (plan.name != name || plan.ConfigGenPath != GenPath || plan.PanelCollectPath != CollectPath
                    || plan.ConfigName != configName
                    || plan.ScriptGenPath != ScriptGenPath || plan.ScriptName != scriptName || plan.typeIndex != typeIndex)
                {
                    plan.name = name;
                    plan.ConfigGenPath = GenPath;
                    plan.PanelCollectPath = CollectPath;
                    plan.ScriptGenPath = ScriptGenPath;
                    plan.ScriptName = scriptName;
                    plan.typeIndex = typeIndex;
                    plan.ConfigName = configName;
                    SavePlansData();
                }
            }
        }

        private Dictionary<string, UIModuleWindowTab> _tabs;
        private MenuTree menu = new MenuTree();
        private SplitView sp = new SplitView();
        private string _name;
        private static UIModuleWindow window;
        private void OnEnable()
        {
            window = this;

            _tabs = typeof(UIModuleWindowTab).GetSubTypesInAssemblies()
                .Where(x => !x.IsAbstract)
                                     .ToList()
                                     .ConvertAll((type) => { return Activator.CreateInstance(type) as UIModuleWindowTab; })
                                     .ToDictionary((tab) => { return tab.name; });


            var _names = _tabs.Keys.ToList();
            menu.ReadTree(_names);
            if (string.IsNullOrEmpty(_name))
            {
                _name = _names[0];
            }

            menu.Select(_name);
            foreach (var item in _tabs.Values)
            {
                item.OnEnable();
            }

            menu.onCurrentChange += (name) =>
            {
                _name = name;
            };





        }


        private void OnDisable()
        {
            foreach (var item in _tabs.Values)
            {
                item.OnDisable();
            }
        }
        private void OnGUI()
        {
            var rs = EditorTools.RectEx.HorizontalSplit(new Rect(Vector2.zero, position.size), 5);
            sp.OnGUI(rs[1]);
            menu.OnGUI(sp.rects[0]);
            if (!_tabs.ContainsKey(_name)) return;
            GUILayout.BeginArea(sp.rects[1]);
            {
                GUILayout.Space(10);
                _tabs[_name].OnGUI();
                GUILayout.Space(10);

            }
            GUILayout.EndArea();

        }

        private void OnHierarchyChange()
        {
            _tabs[_name].OnHierarchyChanged();
            Repaint();
        }
        private UIModuleWindowTab GetTab(Type type)
        {
            return _tabs.Values.First(x => x.GetType() == type);
        }

    }
}
