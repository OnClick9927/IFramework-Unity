/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EditorWindowCacheAttribute : Attribute
    {
        public string searchName { get; private set; }
        public EditorWindowCacheAttribute() { }
        public EditorWindowCacheAttribute(string searchName)
        {
            this.searchName = searchName;
        }

    }
    partial class EditorTools
    {
        [InitializeOnLoad]
        public static class EditorWindowTool
        {
            public class Entity
            {
                public string searchName { get; private set; }
                public Type type { get; private set; }
                public Rect position
                {
                    get
                    {
                        EditorWindow window = Find();
                        if (window == null)
                            return new Rect(0, 0, 0, 0);
                        return window.position;
                    }
                    set
                    {
                        EditorWindow window = FindOrCreate();
                        if (window != null)
                            window.position = value;
                    }
                }
                public bool isOpen
                {
                    get
                    {
                        return FindAll().Length != 0;
                    }
                    set
                    {
                        if (value)
                            FindOrCreate();
                        else
                            foreach (EditorWindow window in FindAll())
                                window.Close();
                    }
                }

                public Entity(Type type, string searchName = "")
                {
                    this.type = type;
                    this.searchName = string.IsNullOrEmpty(searchName) ? type.FullName : searchName;
                }

                public EditorWindow Find()
                {
                    foreach (EditorWindow window in FindAll())
                    {
                        return window;
                    }
                    return null;
                }
                public EditorWindow Create()
                {
                    EditorWindow window = EditorWindow.GetWindow(type);
                    return window;
                }
                public EditorWindow[] FindAll()
                {
                    if (type == null)
                        return new EditorWindow[0];
                    return (EditorWindow[])(Resources.FindObjectsOfTypeAll(type));
                }
                public EditorWindow FindOrCreate()
                {
                    EditorWindow window = Find();
                    if (window != null) return window;
                    if (type == null) return null;
                    window = Create();
                    return window;
                }
            }

            private static List<Entity> _windows;
            public static List<Entity> windows { get { return _windows; } }
            public static List<Entity> user_windows { get; private set; }
            static EditorWindowTool()
            {
                Type type = typeof(EditorWindow);
                var _windows_1 = new List<Entity>();
                var _windows_2 = new List<Entity>();
                var _windows_3 = new List<Entity>();

                type.GetSubTypesInAssemblys().Where(x => !x.IsAbstract && x != typeof(RootWindow)).ToList().ForEach((_type) =>
                {
                    if (!_type.IsDefined(typeof(EditorWindowCacheAttribute), false))
                    {
                        if (_type.FullName.Contains("Unity"))
                            _windows_3.Add(new Entity(_type));
                        else
                            _windows_2.Add(new Entity(_type));
                    }
                    else
                    {
                        EditorWindowCacheAttribute attr = _type.GetCustomAttributes(typeof(EditorWindowCacheAttribute), false).First() as EditorWindowCacheAttribute;
                        _windows_1.Add(new Entity(_type, attr.searchName));
                    }
                });

                _windows_1.Sort((a, b) => { return a.searchName.CompareTo(b.searchName); });
                _windows_2.Sort((a, b) => { return a.searchName.CompareTo(b.searchName); });
                _windows_3.Sort((a, b) => { return a.searchName.CompareTo(b.searchName); });
                _windows = _windows_1.Concat(_windows_2).Concat(_windows_3).ToList();
                user_windows = _windows_1.Concat(_windows_2).ToList();
            }

            private static Entity FindEntity(string name)
            {
                return windows.Find((info) => { return info.searchName == name; });
            }
            private static Entity FindEntity(Type type)
            {
                return windows.Find((info) => { return info.type == type; });
            }
            public static EditorWindow Find(string name)
            {
                Entity item = FindEntity(name);
                return item == null ? null : item.Find();
            }
            public static EditorWindow Find(Type type)
            {
                Entity item = FindEntity(type);
                return item == null ? null : item.Find();
            }
            public static EditorWindow Create(string name)
            {
                Entity item = FindEntity(name);
                return item == null ? null : item.Create();
            }
            public static EditorWindow[] FindAll(string name)
            {
                Entity item = FindEntity(name);
                return item == null ? null : item.FindAll();
            }
            public static EditorWindow FindOrCreate(string name)
            {
                Entity item = FindEntity(name);
                return item == null ? null : item.FindOrCreate();
            }
        }
    }

}
