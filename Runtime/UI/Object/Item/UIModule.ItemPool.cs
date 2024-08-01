/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace IFramework.UI
{
    public partial class UIModule
    {
        public class ItemPool : UIAsyncOperation
        {
            private Queue<UIItemOperation> pool = new Queue<UIItemOperation>();


            private UIModule module;

            public GameObject prefab
            {
                get
                {
                    return isDone ? op.value : null;
                }
            }
            public string path { get { return op.path; } }
            private LoadItemAsyncOperation op;

            public ItemPool(LoadItemAsyncOperation op, UIModule module)
            {
                this.op = op;
                this.module = module;
                Wait(op);
            }
            private async void Wait(LoadItemAsyncOperation op)
            {
                await op;
                base.SetComplete();

            }

            public UIItemOperation Get()
            {
                UIItemOperation val;
                if (pool.Count > 0)
                    val = pool.Dequeue();
                else
                    val = new UIItemOperation(this);
                return val;
            }

            public bool Set(UIItemOperation t)
            {
                if (!pool.Contains(t))
                {
                    var parent = module.GetLayerRectTransform(item_layer);
                    t.gameObject.transform.SetParent(parent, false);
                    pool.Enqueue(t);
                    return true;
                }

                return false;
            }
            public void Clear()
            {
                while (pool.Count > 0)
                {
                    var val = pool.Dequeue();
                    GameObject.Destroy(val.gameObject);
                    (val as IDisposable)?.Dispose();
                }
            }
        }

    }


}
