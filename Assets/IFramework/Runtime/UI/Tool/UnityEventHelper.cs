/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *History:        2022-08-03--
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
namespace IFramework.UI
{
    public static class UnityEventHelper
    {
        private abstract class UIEventEntity : IDisposable
        {
            public abstract void Dispose();
        }
        private class CustomEntity : UIEventEntity
        {
            public Action remove;
            public override void Dispose()
            {
                remove?.Invoke();
            }
        }
        private class UIEventEntity_Void : UIEventEntity
        {
            public UnityEvent _event;
            public UnityAction _action;


            public override void Dispose()
            {
                _event.RemoveListener(_action);
            }
        }
        private class UIEventEntity<T> : UIEventEntity
        {
            public UnityEvent<T> _event;
            public UnityAction<T> _action;



            public override void Dispose()
            {
                _event.RemoveListener(_action);
            }
        }

        public static T Bind<T>(this T obj, Action add, Action remove)
        {
            add?.Invoke();
            var entity = Allocate<CustomEntity>();
            entity.remove = remove;
            entity.AddTo(obj);
            return obj;
        }

        public static T Bind<T>(this T obj, UnityEvent eve, UnityAction callback)
        {
            eve.AddListener(callback);
            var entity = Allocate<UIEventEntity_Void>();
            entity._action = callback;
            entity._event = eve;
            entity.AddTo(obj);
            return obj;
        }
        public static object Bind<T>(this object obj, UnityEvent<T> eve, UnityAction<T> callback)
        {
            eve.AddListener(callback);
            var entity = Allocate<UIEventEntity<T>>();
            entity._action = callback;
            entity._event = eve;
            entity.AddTo(obj);
            return obj;
        }
        public static T BindInputField<T>(this T obj, InputField input, UnityAction<string> callback)
            => (T)Bind(obj, input.onValueChanged, callback);
        public static T BindToggle<T>(this T obj, Toggle toggle, UnityAction<bool> callback)
            => (T)Bind(obj, toggle.onValueChanged, callback);

        public static T BindSlider<T>(this T obj, Slider slider, UnityAction<float> callback)
       => (T)Bind(obj, slider.onValueChanged, callback);
        public static T BindOnEndEdit<T>(this T obj, InputField input, UnityAction<string> callback)
            => (T)Bind(obj, input.onEndEdit, callback);


        public static T BindButton<T>(this T obj, Button button, UnityAction callback)
       => (T)Bind(obj, button.onClick, callback);


        private static Dictionary<Type, ISimpleObjectPool> pools = new Dictionary<Type, ISimpleObjectPool>();
        static SimpleObjectPool<List<UIEventEntity>> listPool = new SimpleObjectPool<List<UIEventEntity>>();
        static T Allocate<T>() where T : UIEventEntity, new()
        {
            var type = typeof(T);
            ISimpleObjectPool pool;
            if (!pools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<T>();
                pools.Add(type, pool);
            }
            return (pool as SimpleObjectPool<T>).Get();
        }
    }
}
