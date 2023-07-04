using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.UI
{
    public class LoopExample : MonoBehaviour
    {
        public LoopScrollRect rect;
        public List<Button> btns;
        public GuideMask guide;
        private RedPointTree tree=new RedPointTree();
        private void Awake()
        {
            string[] keys = new string[]
            {
                "1","1/2,","1/3","1/3/4"
            };
            rect.SetFresh(FreshItem);
            tree.ReadTree(keys, '/');
            for (int i = 0; i < keys.Length; i++)
            {
                int index = i;
                var item = keys[index];
                btns[index].name = item;
                tree.Subscribe(item, (count) =>
                {
                    btns[index].GetComponentInChildren<Text>().text = count.ToString();
                });
            }
            for (int i = 0; i < keys.Length; i++)
            {
                int index = i;
                var item = keys[index];
                tree.SetCount(item, UnityEngine.Random.Range(10,20));
            }

            foreach (var item in btns)
            {
                item.onClick.AddListener(() => {
                    guide.Focus(item.GetComponent<Image>().rectTransform, Vector2.zero);
                    var count = tree.GetCount(item.name);
                    tree.SetCount(item.name, Mathf.Clamp(count - 1,0,1000));
                });
            }
        }

        private void FreshItem(GameObject arg1, int arg2)
        {
            AtlasExample atlas= arg1.GetComponent<AtlasExample>();
            atlas.Set(arg2);
        }

    }

}
