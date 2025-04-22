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
using static IFramework.EditorTools;

namespace IFramework.UI
{

    [EditorWindowCache("UIModule")]
    partial class UIModuleWindow : EditorWindow
    {

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
        private void SwitchToGenCode(GameObject go, string scriptPath)
        {
            var type = EditorPanelCollectionPlans.plan_current.GetSelectType();
            foreach (var item in _tabs.Values)
            {
                if (item.GetType() == type)
                {
                    menu.Select(item.name);
                    (item as UIGenCode).SetGameObject(go, scriptPath);
                    break;
                }
            }
        }
    }
}
