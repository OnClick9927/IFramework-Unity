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
            public IUIEventOwner owner;
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

        public interface IUIEventOwner
        {

        }

        public static void Bind(this IUIEventOwner obj, Action add, Action remove)
        {
            add?.Invoke();
            var entity = Allocate<CustomEntity>();
            entity.remove = remove;
            entity.AddTo(obj);
        }

        public static void Bind(this IUIEventOwner obj, UnityEvent eve, UnityAction callback)
        {
            eve.AddListener(callback);
            var entity = Allocate<UIEventEntity_Void>();
            entity._action = callback;
            entity._event = eve;
            entity.AddTo(obj);
        }
        public static void Bind<T>(this IUIEventOwner obj, UnityEvent<T> eve, UnityAction<T> callback)
        {
            eve.AddListener(callback);
            var entity = Allocate<UIEventEntity<T>>();
            entity._action = callback;
            entity._event = eve;
            entity.AddTo(obj);
        }
        public static void BindInputField(this IUIEventOwner obj, InputField input, UnityAction<string> callback)
            => Bind(obj, input.onValueChanged, callback);
        public static void BindToggle(this IUIEventOwner obj, Toggle toggle, UnityAction<bool> callback)
            => Bind(obj, toggle.onValueChanged, callback);

        public static void BindSlider(this IUIEventOwner obj, Slider slider, UnityAction<float> callback)
       => Bind(obj, slider.onValueChanged, callback);
        public static void BindOnEndEdit(this IUIEventOwner obj, InputField input, UnityAction<string> callback)
            => Bind(obj, input.onEndEdit, callback);


        public static void BindButton(this IUIEventOwner obj, Button button, UnityAction callback)
       => Bind(obj, button.onClick, callback);


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


        private static Dictionary<IUIEventOwner, List<UIEventEntity>> help = new Dictionary<IUIEventOwner, List<UIEventEntity>>();


        private static List<UIEventEntity> GetList(IUIEventOwner msg)
        {
            List<UIEventEntity> result = null;
            if (!help.TryGetValue(msg, out result))
            {
                result = listPool.Get();
                help.Add(msg, result);
            }
            return result;
        }
        private static List<UIEventEntity> FindList(IUIEventOwner msg)
        {
            List<UIEventEntity> result = null;
            help.TryGetValue(msg, out result);

            return result;
        }



        public static void DisposeUIEvents(this IUIEventOwner obj)
        {
            var list = FindList(obj);
            if (list == null) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var e = list[i];
                e.Dispose();

                var type = e.GetType();
                ISimpleObjectPool pool;
                if (pools.TryGetValue(type, out pool))
                {
                    pool.SetObject(e);
                }
            }
            help.Remove(obj);
            list.Clear();
            listPool.Set(list);
        }
        private static UIEventEntity AddTo(this UIEventEntity entity, IUIEventOwner obj)
        {
            entity.owner = obj;
            var list = GetList(obj);
            list.Add(entity);
            return entity;
        }
    }
}
