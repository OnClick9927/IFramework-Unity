/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;
using static IFramework.UI.ScriptCreatorContext;
using UnityEditor.SceneManagement;

namespace IFramework.UI
{
    [System.Serializable]
    public class ScriptCreator
    {
        public void SaveContext()
        {
            var gameObject = this.gameObject;
            if (!gameObject) return;
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public GameObject gameObject { get; private set; }
        private ScriptCreatorContext context;
        public bool containsChildren
        {
            get
            {
                var context = this.context;
                if (context != null)

                    return context.containsChildren;
                return false;
            }
            set
            {
                var context = this.context;

                if (context != null)
                    context.containsChildren = value;
            }
        }
        internal void RemoveFromIgnorePath(List<GameObject> s)
        {

            var find = s.FindAll(x => IsPrefabInstance(x)).Select(x => x.transform.GetPath());
            context.ignorePaths.RemoveAll(x => find.Contains(x));
            SaveContext();
        }
        public void AddToIgnorePath(List<GameObject> s)
        {
            var context = this.context;
            var find = s.FindAll(x => IsPrefabInstance(x)).Select(x => x.transform.GetPath());

            context.ignorePaths.AddRange(find);
            context.ignorePaths = context.ignorePaths.Distinct().ToList();
            SaveContext();
        }
        public bool IsIgnorePath(string path)
        {

            var ignorePaths = context?.ignorePaths;
            if (ignorePaths == null) return false;
            for (int i = 0; i < ignorePaths.Count; i++)
            {
                var _path = ignorePaths[i];
                if (path == _path) return true;
                if (path.Contains(_path))
                {
                    var end = path.Substring(_path.Length);
                    if (end.StartsWith("/")) return true;
                }
            }
            return false;
        }

        public bool IsPrefabInstance(GameObject obj)
        {
            if (obj == null) return true;
            return UnityEditor.PrefabUtility.IsPartOfPrefabInstance(obj);
        }

        public void RemoveMarks(List<GameObject> sms)
        {
            for (int i = 0; i < sms.Count; i++)
            {
                if (IsPrefabInstance(sms[i].gameObject)) continue;
                context.RemoveMark(sms[i].gameObject);
            }
            SaveContext();
        }
        public ScriptCreatorContext.MarkContext AddMark(GameObject go, Type type)
        {
            if (IsPrefabInstance(go)) return null;
            var sm = context.AddMark(go, type.FullName);
            SaveContext();
            return sm;
        }
        public ScriptCreatorContext.MarkContext AddMark(GameObject go, string type)
        {
            if (IsPrefabInstance(go)) return null;
            var sm = context.AddMark(go, type);
            SaveContext();
            return sm;
        }

        public MarkContext GetMark(GameObject go)
        {
            return context.GetMark(go);
        }
        public MarkContext GetPrefabMark(GameObject go)
        {
            var all = context.GetAllMarks();
            return all.Find(m => m.gameObject == go);
        }

        public void SetGameObject(GameObject gameObject)
        {
            if (gameObject != this.gameObject)
            {
                this.gameObject = gameObject;
                this.context = null;
                if (gameObject != null)
                {
                    var context = this.gameObject.GetComponent<ScriptCreatorContext>();
                    if (context == null)
                    {
                        context = this.gameObject.AddComponent<ScriptCreatorContext>();
                    }
                    this.context = context;
                    context.RemoveEmpty();
                    var list = this.gameObject.GetComponentsInChildren<ScriptMark>()
                              .Where(m => !IsPrefabInstance(m.gameObject)).ToList();
                    if (list.Count > 0)
                    {
                        var paths = list.ConvertAll(x => x.transform.GetPath());
                        foreach (var m in list)
                        {
                            var mark = context.AddMark(m.gameObject, m.fieldType);
                            mark.fieldName = m.fieldName;
                        }
                        var ins = PrefabUtility.InstantiatePrefab(gameObject) as GameObject;
                        var find = ins.transform.GetComponentsInChildren<ScriptMark>().Where(x => paths.Contains(x.transform.GetPath()));
                        foreach (var m in find)
                            GameObject.DestroyImmediate(m);
                        PrefabUtility.SaveAsPrefabAsset(ins, AssetDatabase.GetAssetPath(gameObject));
                        GameObject.DestroyImmediate(ins);
                    }

                    SaveContext();
                }

            }
        }

        public bool HandleSameFieldName(out string same)
        {
            bool bo = context.HandleSameFieldName(out same);
            SaveContext();
            return bo;
        }
        public List<ScriptCreatorContext.MarkContext> GetMarks()
        {
            return this.context.marks;
        }
        public List<ScriptCreatorContext.MarkContext> GetAllMarks()
        {
            if (!gameObject) return null;
            return this.context.GetAllMarks();
        }

        public void DestroyMarks()
        {
            context.DestroyMarks();
            SaveContext();
        }


    }
}
