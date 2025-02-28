/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2025-02-28
*********************************************************************************/
using static IFramework.UI.UnityEventHelper;
namespace IFramework.ui
{
	public class PanelOneView : IFramework.UI.UIView 
	{
		class View {
//FieldsStart
		public UnityEngine.UI.Button Close;
		public UnityEngine.UI.Button add;
		public UnityEngine.UI.Button remove;
		public UnityEngine.Transform items;
		public UnityEngine.UI.Button OpenOne;

//FieldsEnd
		public View(IFramework.UI.GameObjectView context){
//InitComponentsStart
			Close = context.GetComponent<UnityEngine.UI.Button>("Close@sm");
			add = context.GetComponent<UnityEngine.UI.Button>("add@sm");
			remove = context.GetComponent<UnityEngine.UI.Button>("remove@sm");
			items = context.GetTransform("items@sm");
			OpenOne = context.GetComponent<UnityEngine.UI.Button>("OpenOne@sm");

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
		protected override void OnBecameInvisible()
		{
		}
		protected override void OnBecameVisible()
		{
		}
	}
}