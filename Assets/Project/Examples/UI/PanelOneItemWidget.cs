/*********************************************************************************
 *Author:         anonymous
 *Date:           2025-04-22
*********************************************************************************/
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
    public class PanelOneItemWidget : IFramework.TestWidgetBase
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
        protected override void InitComponents()
        {
            view = new View(this);
            var pool = root.CreateWidgetPool<POITWidget>(this, view.Prefab_POIT, view.Prefab_POIT.transform.parent);
            pool.Get();
        }
        internal void SetColor(Color color)
        {
            this.view.PanelOneItem.color = color;
        }
    }
}
