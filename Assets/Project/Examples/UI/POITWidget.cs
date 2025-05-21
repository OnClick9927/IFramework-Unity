/*********************************************************************************
 *Author:         OnClick
 *Date:           2025-05-21
*********************************************************************************/
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
	public class POITWidget : IFramework.UI.GameObjectView 
	{
		class View {
//FieldsStart

//FieldsEnd
		public View(POITWidget context){
//InitComponentsStart

//InitComponentsEnd
			}
		}
		private View view;
		protected override void InitComponents()
		{
			view = new View(this);
			Debug.Log(parent);
		}
	}
}