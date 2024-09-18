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
using System.Reflection;

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

                private static string[] _types, _shortTypes;
                public static string[] types
                {
                    get
                    {
                        if (_types == null)
                            Enable();
                        return _types;
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
                public int typeIndex;
                public static Type baseType = typeof(UIGenCode);
                public string ConfigName;

                private static void Enable()
                {
                    var list = EditorTools.GetSubTypesInAssemblies(baseType)
                   .Where(type => !type.IsAbstract);
                    _types = list.Select(type => type.FullName).ToArray();
                    _shortTypes = list.Select(type => type.Name).ToArray();
                }
                public Type GetSelectType()
                {
                    var type_str = types[typeIndex];
                    Type type = EditorTools.GetSubTypesInAssemblies(baseType)
                       .Where(type => !type.IsAbstract)
                       .ToList()
                       .Find(x => x.FullName == type_str);

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
                    PanelCollectPath = EditorTools.projectPath,
                    ConfigGenPath = EditorTools.projectConfigPath,
                    ScriptGenPath = EditorTools.projectScriptPath,
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
            public static PanelCollection Collect()
            {
                string path = UICollectPath;
                PanelCollection collect = null;
                if (!File.Exists(path))
                    collect = new PanelCollection();
                else
                    collect = JsonUtility.FromJson<PanelCollection>(File.ReadAllText(path));

                var paths = AssetDatabase.GetAllAssetPaths()
                    .Where(x => x.EndsWith("prefab") && AssetDatabase.LoadAssetAtPath<UIPanel>(x) != null)
                    .Where(x => x.Contains(plan.PanelCollectPath))
                    .ToList()
                    .ConvertAll(x =>
                    {
                        string path = x;
                        var isResourcePath = x.Contains(res);
                        if (isResourcePath)
                        {
                            var index = x.IndexOf(res);
                            path = x.Substring(index + res.Length + 1)
                                    .Replace(".prefab", "");
                        }
                        return new { isResourcePath, path };
                    });
                collect.datas.RemoveAll(x => paths.Find(y => y.path == x.path) == null);
                paths.FindAll(x => collect.datas.Find(y => y.path == x.path) == null)
                .ForEach(x =>
                {
                    collect.datas.Add(new PanelCollection.Data()
                    {
                        isResourcePath = x.isResourcePath,
                        path = x.path,
                    });
                });
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
        private List<string> GetPanelScriptNames(string name)
        {
            return _tabs.Values.Select(x => x.GetPanelScriptName(name)).ToList();
        }
    }
}
