/*********************************************************************************
 *Author:         anonymous
 *Date:           2025-04-22
*********************************************************************************/
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
	public class PanelTwoView : IFramework.TestViewBase 
	{
		class View {
//FieldsStart
		public UnityEngine.UI.Button Close;
		public UnityEngine.UI.Button hide;

//FieldsEnd
		public View(PanelTwoView context){
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
		protected override void OnLoad(){

            BindButton(view.Close, () => { (Game.Current as UIGame).ui.Close(PanelNames_UIGame.PanelTwo); });
            BindButton(view.hide, () => { (Game.Current as UIGame).ui.Hide(PanelNames_UIGame.PanelTwo); });

        }
        protected override void OnShow(){}
		protected override void OnHide(){}
		protected override void OnClose(){}
		protected override void OnBecameInvisible(){}
		protected override void OnBecameVisible(){}
	}
}
