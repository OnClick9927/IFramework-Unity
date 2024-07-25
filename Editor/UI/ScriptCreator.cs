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
        public bool executeSubContext
        {
            get
            {
                var context = this.context;
                if (context != null)

                    return context.executeSubContext;
                return false;
            }
            set
            {
                var context = this.context;

                if (context != null)
                    context.executeSubContext = value;
            }
        }
        internal void RemoveFromIgnore(List<GameObject> s)
        {

            var find = s.FindAll(x => !CouldMark(x));
            context.ignore.RemoveAll(x => find.Contains(x));
            SaveContext();
        }

        public string ToValidFiledName(string src) => context.ToValidFiledName(src);

        public void AddToIgnore(List<GameObject> s)
        {
            var context = this.context;
            var find = s.FindAll(x => !CouldMark(x));

            context.ignore.AddRange(find);
            context.ignore = context.ignore.Distinct().ToList();
            SaveContext();
        }
        public bool IsIgnore(GameObject go)
        {
            var ignorePaths = context?.ignore;
            if (ignorePaths == null) return false;
            for (int i = 0; i < ignorePaths.Count; i++)
            {
                var _path = ignorePaths[i];
                if (go == _path) return true;
                var tmp = go.transform;
                while (tmp != null)
                {
                    if (tmp.gameObject == _path) return true;
                    tmp = tmp.parent;
                }
            }
            return false;
        }

        public bool IsPrefabInstance(GameObject obj)
        {
            if (obj == null) return true;
            return UnityEditor.PrefabUtility.IsPartOfPrefabInstance(obj);
        }

        public void RemoveMarks(List<GameObject> marks)
        {
            for (int i = 0; i < marks.Count; i++)
            {
                var s = marks[i];
                if (!CouldMark(s)) continue;
                context.RemoveMark(s, !IsPrefabInstance(s));
            }
            SaveContext();
        }
        public ScriptCreatorContext.MarkContext AddMark(GameObject go, Type type) => AddMark(go, type.FullName);
        public ScriptCreatorContext.MarkContext AddMark(GameObject go, string type)
        {
            if (!CouldMark(go)) return null;
            var sm = context.AddMark(go, type, !IsPrefabInstance(go));
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


        public void RemoveUselessMarkFlag()
        {
            List<GameObject> result = new List<GameObject>();
            context.CollectFlagGameObjects(gameObject.transform, result);
            result.RemoveAll(x => IsPrefabInstance(x));

            result.RemoveAll(x => context.marks.Find(y => y.gameObject == x) != null);
            if (result.Count == 0) return;
            for (int i = 0; i < result.Count; i++)
                context.RemoveMark(result[i], true);
            SaveContext();

        }


        public void RemoveEmptyMarks()
        {
            context.RemoveEmpty();
            SaveContext();
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

                    SaveContext();
                }

            }
        }

        public bool HandleSameFieldName(out string same)
        {
            bool bo = context.HandleSameFieldName(out same, IsPrefabInstance);
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

        public bool CouldMark(GameObject go)
        {
            if (IsPrefabInstance(go))
            {
                var tmp = go.transform;
                while (true)
                {
                    var child = tmp.GetComponent<ScriptCreatorContext>();
                    if (child != null) return false;
                    tmp = tmp.parent;
                    if (!IsPrefabInstance(tmp.gameObject))
                        return true;
                }
            }
            return true;
        }



    }
}
