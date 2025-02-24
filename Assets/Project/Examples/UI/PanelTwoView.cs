/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2025-02-24
*********************************************************************************/
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
	public class PanelTwoView : IFramework.UI.UIView 
	{
		class View {
//FieldsStart
		public UnityEngine.UI.Button Close;
		public UnityEngine.UI.Button hide;

//FieldsEnd
		public View(IFramework.UI.GameObjectView context){
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
		protected override void OnLoad()
		{
			BindButton(view.Close, () => { (IFramework.Launcher.Instance.game as UIGame).ui.Close(PanelNames_UIGame.PanelTwo); });
			BindButton(view.hide, () => { (IFramework.Launcher.Instance.game as UIGame).ui.Hide(PanelNames_UIGame.PanelTwo); });

		}

        protected override void OnShow()
		{
		}

		protected override void OnHide()
		{
		}

		protected override void OnClose()
		{
		}

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }
    }
}
