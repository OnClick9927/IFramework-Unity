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
using IFramework;

using System.IO;
using System.Text;

namespace IFramework.UI
{

    [EditorWindowCache("UIModule")]
    public partial class UIMoudleWindow : EditorWindow
    {
        private Dictionary<string, UIMoudleWindowTab> _tabs;
        private MenuTree menu=new MenuTree();
        private SplitView sp = new SplitView();
        private string _name;
        private static UIMoudleWindow window;
        private void OnEnable()
        {
            window = this;
            if (!File.Exists(UICollectPath))
            {
                collect = new PanelPathCollect();
                Save();
            }
            collect = JsonUtility.FromJson<PanelPathCollect>(File.ReadAllText(UICollectPath));
            //menu = new MenuTree();
            _tabs = typeof(UIMoudleWindowTab).GetSubTypesInAssemblys()
                                     .ToList()
                                     .ConvertAll((type) => { return Activator.CreateInstance(type) as UIMoudleWindowTab; })
                                     .ToDictionary((tab) => { return tab.name; });

            var _names = _tabs.Keys.ToList();

            foreach (var item in _tabs.Values)
            {
                item.OnEnable();
            }
            menu.ReadTree(_names);
            menu.onCurrentChange += (name) =>
            {
                _name = name;
            };
            if (string.IsNullOrEmpty(_name))
            {
                menu.Select(_names[0]);
            }
            else
            {
                menu.Select(_name);
            }
            sp.fistPan += Sp_fistPan;
            sp.secondPan += Sp_secondPan;

       

        }

        static string UICollectPath { get { return EditorTools.projectConfigPath.CombinePath("UICollect.json"); } }
        static string cs_path { get { return EditorTools.projectScriptPath.CombinePath("PanelNames.cs"); } }

        const string res = "Resources";

        private static PanelPathCollect collect;
        private static PanelPathCollect Collect()
        {
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

            Save();
            return collect;
        }
        private static void Save()
        {
            File.WriteAllText(UICollectPath, JsonUtility.ToJson(collect, true));
            AssetDatabase.Refresh();
        }

        private void Sp_secondPan(Rect obj)
        {
            if (!_tabs.ContainsKey(_name)) return;
            GUILayout.BeginArea(obj);
            {
                GUILayout.Space(10);
                _tabs[_name].OnGUI();
                GUILayout.Space(10);

            }
            GUILayout.EndArea();
        }

        private void Sp_fistPan(Rect obj)
        {
            menu.OnGUI(obj);
        }
        public static void CS_BuildPanelNames()
        {
            Collect();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public class PanelNames");
            sb.AppendLine("{");
            foreach (var data in collect.datas)
            {
                sb.AppendLine($"\t public static string {data.name} = \"{data.path}\";");
            }
            sb.AppendLine("}");
            File.WriteAllText(cs_path, sb.ToString());
            AssetDatabase.Refresh();
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
            var rs = EditorTools.RectEx.HorizontalSplit(new Rect(Vector2.zero,position.size),20);
            Tool(rs[0]);
            sp.OnGUI(rs[1]);

        }
        private void Tool(Rect position)
        {
            GUILayout.BeginArea(position);
            GUILayout.BeginHorizontal("ToolBar");
            GUILayout.Label("", GUILayout.Width(100));

            Rect r = GUILayoutUtility.GetLastRect();
            if (GUI.Button(r, "Build", GUIStyles.ToolbarDropDown))
            {
                var methods = GetType().GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                GenericMenu menu = new GenericMenu();
                foreach (var method in methods)
                {
                    menu.AddItem(new GUIContent($"{method.Name}"), false, () =>
                    {
                        method.Invoke(null, null);
                    });
                }
                menu.DropDown(r);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void OnHierarchyChange()
        {
            Collect();
            _tabs[_name].OnHierarchyChanged();
            Repaint();
        }

    }
}
