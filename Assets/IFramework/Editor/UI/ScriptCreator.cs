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
using UnityEditor;
using System;

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
        public string rootPath => gameObject.transform.GetPath();
        public GameObject gameObject { get; private set; }
        private IScriptCreatorContext context { get; set; }
        public List<GameObject> GetPrefabs() => context.GetPrefabs();

        private List<MarkContext> marks => context.GetMarks();
        private const string flag = "@sm";
        public List<MarkContext> GetMarks() => marks;


        public string ToValidFiledName(string src) => ScriptCreatorContext.ToValidFiledName(src);



        public bool IsPrefabInstance(GameObject obj) => obj == null ? true : UnityEditor.PrefabUtility.IsPartOfPrefabInstance(obj);

        private void ValidateMarkFieldName(MarkContext mark)
        {

            if (!ScriptCreatorContext.IsLegalFieldName(mark.fieldName)) mark.fieldName = mark.gameObject.name.Replace(flag, "");
            mark.fieldName = ToValidFiledName(mark.fieldName);
        }
        private void AddMarkFlag(GameObject go)
        {
            if (go == gameObject) return;
            string name = go.name;
            if (name.Contains(flag)) return;
            go.name += flag;
        }
        private MarkContext AddMark(GameObject go, string type, bool add_flag)
        {
            var find = marks.Find(x => x.gameObject == go);

            if (find == null)
            {
                find = new MarkContext() { gameObject = go, fieldType = type };
                if (add_flag)
                {
                    AddMarkFlag(go);
                }
                marks.Add(find);
            }
            else
            {
                find.fieldType = type;
            }
            ValidateMarkFieldName(find);
            return find;
        }
        public MarkContext AddMark(GameObject go, Type type) => AddMark(go, type.FullName);
        public MarkContext AddMark(GameObject go, string type)
        {
            if (!CouldMark(go)) return null;
            var sm = AddMark(go, type, !IsPrefabInstance(go));
            SaveContext();
            return sm;
        }





        private void CollectFlagGameObjects(Transform transform, List<GameObject> goes)
        {
            if (transform.name.Contains(flag))
            {
                goes.Add(transform.gameObject);
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                CollectFlagGameObjects(transform.GetChild(i), goes);
            }
        }
        private void RemoveMarkFlag(GameObject go)
        {
            string name = go.name;
            if (!name.Contains(flag)) return;
            go.name = name.Replace(flag, "");
        }
        private void RemoveMark(GameObject go, bool remove_flag)
        {
            if (remove_flag)
                RemoveMarkFlag(go);
            marks.RemoveAll(x => x.gameObject == go);
        }
        public void RemoveMarks(List<GameObject> marks)
        {
            for (int i = 0; i < marks.Count; i++)
            {
                var s = marks[i];
                if (!CouldMark(s)) continue;
                RemoveMark(s, !IsPrefabInstance(s));
            }
            SaveContext();
        }
        public void RemoveUselessMarkFlag()
        {
            List<GameObject> result = new List<GameObject>();
            CollectFlagGameObjects(gameObject.transform, result);
            result.RemoveAll(x => IsPrefabInstance(x));

            result.RemoveAll(x => context.GetMarks().Find(y => y.gameObject == x) != null);
            if (result.Count == 0) return;
            for (int i = 0; i < result.Count; i++)
                RemoveMark(result[i], true);
            SaveContext();

        }
        public void RemoveEmptyMarks()
        {
            marks.RemoveAll(x => x.gameObject == null);
            SaveContext();
        }
        public void DestroyAllMarks()
        {
            for (int i = 0; i < marks.Count; i++)
            {
                RemoveMarkFlag(marks[i].gameObject);
            }
            marks.Clear();
            SaveContext();
        }




        public MarkContext GetMark(GameObject go) => marks.Find(x => x.gameObject == go);
        public void SetGameObject(GameObject gameObject)
        {
            if (gameObject != this.gameObject)
            {
                this.gameObject = gameObject;
                this.context = null;
                if (gameObject != null)
                {
                    var cts = gameObject.GetComponents<IScriptCreatorContext>();

                    IScriptCreatorContext _context = null;

                    if (cts != null && cts.Length > 0)
                    {
                        IScriptCreatorContext other = null;
                        IScriptCreatorContext _base = null;
                        for (int i = 0; i < cts.Length; i++)
                        {
                            var ct = cts[i];
                            if (ct.GetType() == typeof(ScriptCreatorContext))
                                _base = ct;
                            else
                                other = ct;
                        }
                        if (other != null && _base != null)
                        {
                            other.Read(_base);
                            GameObject.DestroyImmediate(_base as UnityEngine.Object, true);
                            _context = other;
                        }

                        _context = other == null ? _base : other;
                    }

                    if (_context == null)
                        _context = gameObject.AddComponent<ScriptCreatorContext>();
                    this.context = _context;
                    RemoveEmptyMarks();

                    SaveContext();
                }

            }
        }
        public bool HandleSameFieldName(out string same)
        {
            bool bo = HandleSameFieldName(out same, IsPrefabInstance);
            SaveContext();
            return bo;
        }
        public bool CouldMark(GameObject go)
        {
            if (go == null) return false;
            if (IsPrefabInstance(go))
            {
                var tmp = go.transform;
                while (true)
                {
                    var child = tmp.GetComponent<IScriptCreatorContext>();
                    if (child != null) return false;
                    tmp = tmp.parent;
                    if (!IsPrefabInstance(tmp.gameObject))
                        return true;
                }
            }
            return true;
        }
        public bool HandleSameFieldName(out string same, Func<GameObject, bool> fixedName)
        {
            same = "";

            bool exist = false;
            var prefab_list = new List<string>();
            Dictionary<string, List<MarkContext>> map = new Dictionary<string, List<MarkContext>>();
            for (int i = 0; i < marks.Count; i++)
            {
                var mark = marks[i];
                var _fixed = fixedName(mark.gameObject);
                if (!_fixed)
                {
                    if (!map.ContainsKey(mark.fieldName))
                        map.Add(mark.fieldName, new List<MarkContext>());
                    map[mark.fieldName].Add(mark);
                }
                else
                {
                    prefab_list.Add(mark.fieldName);
                }
            }
            foreach (var item in map)
            {
                if (item.Value.Count > 1 || prefab_list.Contains(item.Key))
                {
                    exist = true;
                    var list = item.Value;
                    same += $"{list[0].fieldName}  ";

                    for (int i = 0; i < list.Count; i++)
                    {
                        var cur = list[i];
                        list[i].fieldName += $"_{i}";
                        var sameParrent = list.FindAll(x => x.gameObject.transform.parent == cur.gameObject.transform.parent);
                        if (sameParrent.Count != 1)
                            for (int i2 = 0; i2 < sameParrent.Count; i2++)
                            {
                                var __same = sameParrent[i2];
                                __same.gameObject.transform.name = __same.fieldName + flag;
                            }
                    }
                }
            }
            return exist;
        }
    }
}
