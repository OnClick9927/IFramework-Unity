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
    class AddArg : IEventArgs
    {
        public float time;
    }
    public class PanelOneView : TestViewBase, IEventHandler<AddArg>
    {
        class View
        {
            //FieldsStart
            public UnityEngine.UI.Button Close;
            public UnityEngine.UI.Button add;
            public UnityEngine.UI.Button remove;
            public UnityEngine.Transform items;
            public UnityEngine.UI.Button OpenOne;
            public UnityEngine.GameObject Prefab_PanelOneItem;

            //FieldsEnd
            public View(IFramework.UI.GameObjectView context)
            {
                //InitComponentsStart
                Close = context.GetComponent<UnityEngine.UI.Button>("Close@sm");
                add = context.GetComponent<UnityEngine.UI.Button>("add@sm");
                remove = context.GetComponent<UnityEngine.UI.Button>("remove@sm");
                items = context.GetTransform("items@sm");
                OpenOne = context.GetComponent<UnityEngine.UI.Button>("OpenOne@sm");
                Prefab_PanelOneItem = context.FindPrefab("PanelOneItem");

                //InitComponentsEnd
            }
        }
        private View view;

        const string eve_key_remove = "eve_key_remove";
        protected override void InitComponents()
        {
            view = new View(this);
        }
        async void TweenTest()
        {

            var tween = await this.view.OpenOne.transform.DoLocalScale(Vector3.one * 2, 0.2f, false).SetLoop(LoopType.PingPong, 6).AddTo(this);
            Debug.LogError("xxl");


        }
        async void TweenTest2()
        {
            var pos = transform.position;
            var _tween = await Tween.Parallel()
                       .NewContext(() => this.view.OpenOne.transform.DoLocalScale(Vector3.one, Vector3.one * 2, 0.2f, false).SetLoop(LoopType.PingPong, 6))
                      .NewContext(() => this.view.OpenOne.transform.DoPosition(pos, Vector3.one, 0.2f, true).SetLoop(LoopType.PingPong, 6))
                               .Run().AddTo(this).SetAutoCycle(false);

            Debug.LogError("xxl");

            await TimeEx.Scheduler.Delay(0.5f).AddTo(this);
            await _tween.ReStart();

            Debug.LogError("xxl");

        }
        protected override void OnLoad()
        {

            this.BindButton(this.view.remove, () =>
             {
                 Events.Publish(eve_key_remove, null);
             });
            this.BindButton(view.OpenOne, () =>
             {
                 (Game.Current as UIGame).ui.Show(PanelNames_UIGame.PanelTwo);
             });
            CreateWidgetPool<PanelOneItemWidget>(view.Prefab_PanelOneItem, view.items, () => new PanelOneItemWidget());
            //collection = new UIItemViewCollection((Launcher.Instance.game as UIGame).ui);
            this.BindButton(this.view.Close, (Game.Current as UIGame).CloseView);
            this.BindButton(this.view.add, () =>
                 {
                     //Events.Publish(new AddArg() { time = Time.deltaTime });
                     Events.Publish(nameof(AddArg), new AddArg() { time = Time.deltaTime });

                 });
            this.SubscribeEvent<AddArg>(this);
            this.SubscribeEvent(eve_key_remove, (e) =>
            {
                Remove();
            });
            this.SubscribeEvent(nameof(AddArg), (e) =>
            {
                Debug.Log("add");
            });
            //TweenTest();
            //TweenTest2();
            Test2();
            //Test();
        }
        private async void Test()
        {
            Debug.LogError("HH0");
            Debug.LogError(Time.time);

            await TimeEx.Scheduler.While((time, delta) => Time.time <= 5f).AddTo(this);
            Debug.LogError(Time.time);

            await TimeEx.Scheduler.Delay(1f).AddTo(this);
            await TimeEx.Scheduler.Delay(1f, (time, delta) =>
            {
                Debug.LogError("HH1");
            }).AddTo(this);
            if (this.gameObject)
                Debug.LogError("HH4");
        }

        private async void Test2()
        {
            Debug.LogError("HH0");
            Debug.LogError(Time.time);

            var seq = await TimeEx.Scheduler.Parallel()
                     .NewContext((scheduler) =>
                     scheduler.While((time, delta) => Time.time <= 5f).OnComplete((context) =>
                     {
                         Debug.LogError("HH1");
                         Debug.LogError(Time.time);

                     }))
                     .NewContext((scheduler) => scheduler.Delay(1f, (time, delta) =>
                     {
                         Debug.LogError("HH2");
                     }))
                     .Run().AddTo(this);


            Debug.LogError(Time.time);

            if (this.gameObject)
                Debug.LogError("HH4");
        }

        private Stack<PanelOneItemWidget> queue = new Stack<PanelOneItemWidget>();
        private void Remove()
        {
            if (queue.Count == 0) return;
            var pool = this.FindWidgetPool<PanelOneItemWidget>(view.Prefab_PanelOneItem);
            pool.Set(queue.Pop());
        }
        private void Add()
        {
            var pool = this.FindWidgetPool<PanelOneItemWidget>(view.Prefab_PanelOneItem);
            var result = pool.Get();
            result.SetColor(new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1));
            queue.Push(result);
        }
        void IEventHandler<AddArg>.OnEvent(AddArg msg)
        {
            Add();
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

        protected override void OnBecameVisible()
        {
        }

        protected override void OnBecameInvisible()
        {
        }


    }
}
