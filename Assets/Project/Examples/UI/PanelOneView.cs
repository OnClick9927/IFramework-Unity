/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-07-31
*********************************************************************************/
using IFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
    public class PanelOneView : IFramework.UI.MVC.UIView
    {
        //FieldsStart
        private UnityEngine.UI.Button Close;
        private UnityEngine.UI.Button add;
        private UnityEngine.UI.Button remove;
        private UnityEngine.Transform items;

        //FieldsEnd
        protected override void InitComponents()
        {
            //InitComponentsStart
            Close = GetComponent<UnityEngine.UI.Button>("Close@sm");
            add = GetComponent<UnityEngine.UI.Button>("add@sm");
            remove = GetComponent<UnityEngine.UI.Button>("remove@sm");
            items = GetTransform("items@sm");

            //InitComponentsEnd
        }
        private UIItemViewCollection collection;
        private UIEventBox eve_ui;
        private EventBox eve;
        const string eve_key_remove = "eve_key_remove";
        protected override void OnLoad()
        {
            eve_ui = new UIEventBox();
            eve = new EventBox();
            BindButton(this.Close, (Launcher.Instance.game as UIGame).CloseView).AddTo(eve_ui);
            BindButton(this.add, Add).AddTo(eve_ui);
            BindButton(this.remove, () =>
            {
                Events.Publish(eve_key_remove, null);
            }).AddTo(eve_ui);
            collection = new UIItemViewCollection((Launcher.Instance.game as UIGame).ui);
            eve.Subscribe(eve_key_remove, (e) =>
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
            var result = await collection.Get<PanelOneItemView>("Assets/Project/Examples/UI/PanelOneItem.prefab", this.items);
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
            eve_ui.Dispose();
            eve.Dispose();
        }
    }
}
