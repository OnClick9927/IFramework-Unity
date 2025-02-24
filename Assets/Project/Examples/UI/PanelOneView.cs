/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-10-24
*********************************************************************************/
using IFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
    public class PanelOneView : IFramework.UI.UIView
    {
        class View
        {
            //FieldsStart
		public UnityEngine.UI.Button Close;
		public UnityEngine.UI.Button add;
		public UnityEngine.UI.Button remove;
		public UnityEngine.Transform items;
		public UnityEngine.UI.Button OpenOne;

            //FieldsEnd
            public View(IFramework.UI.GameObjectView context)
            {
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
        private UIItemViewCollection collection;
        const string eve_key_remove = "eve_key_remove";
        protected override void InitComponents()
        {
            view = new View(this);
        }
        protected override void OnLoad()
        {
            BindButton(this.view.Close, (Launcher.Instance.game as UIGame).CloseView).AddTo(this);
            BindButton(this.view.add, Add).AddTo(this);
            BindButton(this.view.remove, () =>
            {
                Events.Publish(eve_key_remove, null);
            }).AddTo(this);
            BindButton(view.OpenOne, () =>
            {
                (Launcher.Instance.game as UIGame).ui.Show(PanelNames_UIGame.PanelTwo);
            });
            collection = new UIItemViewCollection((Launcher.Instance.game as UIGame).ui);
            SubscribeEvent(eve_key_remove, (e) =>
            {
                Remove();
            });
        }
        private Stack<PanelOneItemView> queue = new Stack<PanelOneItemView>();
        private void Remove()
        {
            if (queue.Count == 0) return;
            collection.Set(queue.Pop());
        }
        private async void Add()
        {
            var result = await collection.Get<PanelOneItemView>("Assets/Project/Examples/UI/PanelOneItem.prefab", this.view.items);
            result.SetColor(new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1));
            queue.Push(result);
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

        protected override void OnVisible()
        {
        }

        protected override void OnInVisible()
        {
        }
    }
}
