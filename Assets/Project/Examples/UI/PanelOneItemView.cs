/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-10-24
*********************************************************************************/
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
	public class PanelOneItemView : IFramework.UI.UIItemView 
	{
		class View {
//FieldsStart
		public UnityEngine.UI.Image PanelOneItem;

//FieldsEnd
		public View(IFramework.UI.GameObjectView context){
//InitComponentsStart
			PanelOneItem = context.GetComponent<UnityEngine.UI.Image>("");

//InitComponentsEnd
			}
		}
		private View view;
		protected override void InitComponents()
		{
			view = new View(this);
		}
        public override void OnSet()
        {
            base.OnSet();
            Log.L("OnSet");
        }

        protected override void OnGet()
        {
            Log.L("OnGet");
        }

        internal void SetColor(Color color)
        {
            this.view.PanelOneItem.color = color;
        }
    }
}