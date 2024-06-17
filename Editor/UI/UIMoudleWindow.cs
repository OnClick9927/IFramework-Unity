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

namespace IFramework.UI
{

    [EditorWindowCache("UIModule")]
    public partial class UIModuleWindow : EditorWindow
    {

        public class UICollectData
        {
            const string res = "Resources";
            [System.Serializable]
            public class Path
            {
                public string path;
            }
            private static Path _path;
            public static string UICollectDir
            {
                get
                {
                    if (_path == null)
                    {
                        _path = EditorTools.GetFromPrefs<Path>(nameof(UICollectData), false);
                        if (_path == null)
                            _path = new Path();
                    }
                    string path = _path.path;
                    if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    {
                        path = EditorTools.projectConfigPath;
                    }
                    return path;
                }

                set
                {
                    if (_path == null)
                        _path = new Path();
                    if (value != _path.path && Directory.Exists(value))
                    {
                        _path.path = value;
                        EditorTools.SaveToPrefs(_path, nameof(UICollectData), false);
                    }
                }
            }
            public static string UICollectPath { get { return UICollectDir.CombinePath("UICollect.json"); } }

            private static PanelPathCollect _collect;
            public static PanelPathCollect collect
            {
                get
                {
                    _collect = Collect();
                    return _collect;
                }
            }

            public static void Save(PanelPathCollect collect)
            {
                File.WriteAllText(UICollectPath, JsonUtility.ToJson(collect, true));
                AssetDatabase.Refresh();
            }
            public static PanelPathCollect Collect()
            {
                string path = UICollectPath;
                PanelPathCollect collect = null;
                if (!File.Exists(path))
                    collect = new PanelPathCollect();
                else
                    collect = JsonUtility.FromJson<PanelPathCollect>(File.ReadAllText(path));

                var paths = AssetDatabase.GetAllAssetPaths()
                    .Where(x => x.EndsWith("prefab") && AssetDatabase.LoadAssetAtPath<UIPanel>(x) != null)
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
                    collect.datas.Add(new PanelPathCollect.Data()
                    {
                        isResourcePath = x.isResourcePath,
                        path = x.path,
                    });
                });
                Save(collect);
                return collect;
            }

        }

        private Dictionary<string, UIMoudleWindowTab> _tabs;
        private MenuTree menu = new MenuTree();
        private SplitView sp = new SplitView();
        private string _name;
        private static UIModuleWindow window;
        private void OnEnable()
        {
            window = this;

            _tabs = typeof(UIMoudleWindowTab).GetSubTypesInAssemblys()
                .Where(x => !x.IsAbstract)
                                     .ToList()
                                     .ConvertAll((type) => { return Activator.CreateInstance(type) as UIMoudleWindowTab; })
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

        static string cs_path { get { return EditorTools.projectScriptPath.CombinePath("PanelNames.cs"); } }

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

    }
}
