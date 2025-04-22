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
		class View {
//FieldsStart
		public UnityEngine.UI.Image PanelOneItem;

//FieldsEnd
		public View(PanelOneItemWidget context){
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
        internal void SetColor(Color color)
        {
            this.view.PanelOneItem.color = color;
        }
    }
}