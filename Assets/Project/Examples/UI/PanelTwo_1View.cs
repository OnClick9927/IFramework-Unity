/*********************************************************************************
 *Author:         OnClick
 *Date:           2025-03-20
*********************************************************************************/
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
	public class PanelTwo_1View : IFramework.UI.UIView 
	{
		class View {
//FieldsStart
		public UnityEngine.UI.Button Close;
		public UnityEngine.UI.Button hide;

//FieldsEnd
		public View(PanelTwo_1View context){
//InitComponentsStart
			Close = context.GetComponent<UnityEngine.UI.Button>("Close@sm");
			hide = context.GetComponent<UnityEngine.UI.Button>("hide@sm");

//InitComponentsEnd
			}
		}
		private View view;
		protected override void InitComponents()
		{
			view = new View(this);
		}
		protected override void OnLoad(){}
		protected override void OnShow(){}
		protected override void OnHide(){}
		protected override void OnClose(){}
		protected override void OnBecameInvisible(){}
		protected override void OnBecameVisible(){}
	}
}