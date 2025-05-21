/*********************************************************************************
 *Author:         anonymous
 *Date:           2025-04-22
*********************************************************************************/
using IFramework.UI;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
    public class PanelOneItemWidget : IFramework.TestWidgetBase, IPoolAbleWidget
    {
        class View
        {
            //FieldsStart
            public UnityEngine.UI.Image PanelOneItem;
            public UnityEngine.GameObject Prefab_POIT;

            //FieldsEnd
            public View(PanelOneItemWidget context)
            {
                //InitComponentsStart
                PanelOneItem = context.GetComponent<UnityEngine.UI.Image>("");
                Prefab_POIT = context.FindPrefab("POIT");

                //InitComponentsEnd
            }
        }
        private View view;
        Queue<POITWidget> queue = new Queue<POITWidget>();
        protected override void InitComponents()
        {
            view = new View(this);
            var pool = root.CreateWidgetPool<POITWidget>(this, view.Prefab_POIT, view.Prefab_POIT.transform.parent);

            queue.Enqueue(pool.Get());
            queue.Enqueue(pool.Get());

        }
        internal void SetColor(Color color)
        {
            this.view.PanelOneItem.color = color;
        }

        void IPoolAbleWidget.OnSet()
        {
            while (queue.Count > 0)
            {
                var pool = root.FindWidgetPool<POITWidget>(view.Prefab_POIT);
                pool.Set(queue.Dequeue());
            }
        }
    }
}
