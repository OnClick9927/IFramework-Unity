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
using System.Text.RegularExpressions;
using System.Text;

namespace IFramework.UI
{
    [System.Serializable]
    public class ScriptCreator
    {

        public GameObject gameObject;
        [SerializeField] private List<ScriptMark> marks = new List<ScriptMark>();
        public bool IsPrefabInstance(GameObject obj)
        {
            if (obj == null) return true;
            return UnityEditor.PrefabUtility.IsPartOfPrefabInstance(obj);
        }

        public void RemoveMarks(List<ScriptMark> sms)
        {
            for (int i = 0; i < sms.Count; i++)
            {
                if (IsPrefabInstance(sms[i].gameObject)) continue;
                RemoveMarkFlag(sms[i]);
                GameObject.DestroyImmediate(sms[i], true);
            }
            CollectMarks();
        }
        public ScriptMark AddMark(GameObject go)
        {
            if (IsPrefabInstance(go)) return null;
            ScriptMark sm = go.AddComponent<ScriptMark>();
            if (go != gameObject)
            {
                AddMarkFlag(sm);
            }
            CollectMarks();

            return sm;
        }


        private const string flag = "@sm";
        private void RemoveMarkFlag(ScriptMark sm)
        {
            string name = sm.name;
            if (!name.Contains(flag)) return;
            sm.name = name.Replace(flag, "");
        }
        private void AddMarkFlag(ScriptMark sm)
        {
            string name = sm.name;
            if (name.Contains(flag)) return;
            sm.name += flag;
        }
        private static bool IsLegalFieldName(string self)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return Regex.IsMatch(self, @"^[_a-zA-Z][_a-zA-Z0-9]*$");
        }
        public void ValidateMarkFieldName(ScriptMark mark)
        {
            if (!IsLegalFieldName(mark.fieldName)) mark.fieldName = mark.name.Replace(flag, "");
            var m = Regex.Matches(mark.fieldName, "[_a-zA-Z0-9]");
            var list = m.Where(x => x.Success).Select(x => x.Value).ToList();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    int _val;
                    if (int.TryParse(list[i], out _val))
                    {
                        sb.Append("_");
                    }
                }
                sb.Append(list[i]);
            }
            mark.fieldName = sb.ToString();

        }

        public void SetGameObject(GameObject gameObject)
        {
            if (gameObject != this.gameObject)
            {
                this.gameObject = gameObject;
                marks.Clear();
                CollectMarks();
            }
        }
        private void CollectMarks()
        {
            if (!gameObject) return;

            var marks = gameObject.GetComponentsInChildren<ScriptMark>(true);
            if (marks == null) return;
            var list = marks.ToList();
            if (this.marks != null) list.AddRange(this.marks);
            list = list.Distinct().ToList();
            list.RemoveAll((o) => { return o == null || IsPrefabInstance(o.gameObject); });
            this.marks.Clear();
            this.marks.AddRange(list);
        }
        public bool HandleSameFieldName(out string same)
        {
            same = "";
            bool exist = false;
            CollectMarks();
            Dictionary<string, List<ScriptMark>> map = new Dictionary<string, List<ScriptMark>>();
            for (int i = 0; i < marks.Count; i++)
            {
                var cur = marks[i];
                if (!map.ContainsKey(cur.fieldName))
                    map.Add(cur.fieldName, new List<ScriptMark>());
                map[cur.fieldName].Add(cur);
            }
            foreach (var item in map)
            {
                if (item.Value.Count > 1)
                {
                    exist = true;
                    var list = item.Value;
                    same += $"{list[0].fieldName}  ";

                    for (int i = 0; i < list.Count; i++)
                    {
                        var cur = list[i];
                        list[i].fieldName += $"_{i}";
                        var sameParrent = list.FindAll(x => x.transform.parent == cur.transform.parent);
                        if (sameParrent.Count != 1)
                            for (int i2 = 0; i2 < sameParrent.Count; i2++)
                            {
                                var __same = sameParrent[i2];
                                __same.transform.name = __same.fieldName + flag;
                            }
                    }
                }
            }
            return exist;
        }
        public List<ScriptMark> GetMarks()
        {
            return this.marks;
        }
        public List<ScriptMark> GetAllMarks()
        {
            if (!gameObject) return null;
            var marks = gameObject.GetComponentsInChildren<ScriptMark>(true);
            return marks.ToList();
        }

        public void DestroyMarks()
        {
            gameObject.GetComponentsInChildren<ScriptMark>(true).ToList().ForEach((sm) =>
            {
                if (!IsPrefabInstance(sm.gameObject))
                {
                    GameObject.DestroyImmediate(sm, true);
                }
            });
            CollectMarks();
        }

    }
}
