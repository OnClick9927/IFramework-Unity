﻿/*********************************************************************************
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
        public abstract class UIEventEntity : IDisposable
        {
            public abstract void Dispose();
        }
        private class UIEventEntity_Void : UIEventEntity
        {
            UnityEvent _event;
            UnityAction _action;

            public UIEventEntity_Void(UnityEvent @event, UnityAction action)
            {
                _event = @event;
                _action = action;
            }

            public override void Dispose()
            {
                _event.RemoveListener(_action);
            }
        }
        private class UIEventEntity_Delegate : UIEventEntity
        {
            private Delegate _delegate;

            public UIEventEntity_Delegate(Delegate @delegate, Delegate action)
            {
                _delegate = @delegate;
            }

            public override void Dispose()
            {
                _delegate = null;
            }
        }
        private class UIEventEntity<T> : UIEventEntity
        {
            UnityEvent<T> _event;
            UnityAction<T> _action;

            public UIEventEntity(UnityEvent<T> @event, UnityAction<T> action)
            {
                _event = @event;
                _action = action;
            }

            public override void Dispose()
            {
                _event.RemoveListener(_action);
            }
        }

        public interface IUIEventBox
        {
            void AddUIEvent(UIEventEntity entity);
            void DisposeUIEvent(UIEventEntity entity);
            void DisposeUIEvents();
        }

        public class UIEventBox : IUIEventBox
        {
            private List<UIEventEntity> uIEventEntities = new List<UIEventEntity>();
            public void AddUIEvent(UIEventEntity entity)
            {
                if (uIEventEntities.Contains(entity)) return;
                uIEventEntities.Add(entity);
            }
            public void DisposeUIEvent(UIEventEntity entity)
            {
                entity.Dispose();
                uIEventEntities.Remove(entity);
            }
            public void DisposeUIEvents()
            {
                for (int i = 0; i < uIEventEntities.Count; i++)
                {
                    uIEventEntities[i].Dispose();
                }
                uIEventEntities.Clear();
            }
        }
        public static UIEventEntity BindInputField(InputField input, UnityAction<string> callback)
        {
            input.onValueChanged.AddListener(callback);
            var entity = new UIEventEntity<string>(input.onValueChanged, callback);
            return entity;
        }
        public static UIEventEntity BindToggle(Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
            var entity = new UIEventEntity<bool>(toggle.onValueChanged, callback);
            return entity;
        }
        public static UIEventEntity BindSlider(Slider slider, UnityAction<float> callback)
        {
            slider.onValueChanged.AddListener(callback);
            var entity = new UIEventEntity<float>(slider.onValueChanged, callback);
            return entity;
        }
        public static UIEventEntity BindOnEndEdit(InputField input, UnityAction<string> callback)
        {
            input.onEndEdit.AddListener(callback);
            var entity = new UIEventEntity<string>(input.onEndEdit, callback);
            return entity;
        }
        public static UIEventEntity BindOnValidateInput(InputField input, InputField.OnValidateInput callback)
        {
            input.onValidateInput = callback;
            var entity = new UIEventEntity_Delegate(input.onValidateInput, callback);
            return entity;
        }
        public static UIEventEntity BindButton(Button button, UnityAction callback)
        {
            button.onClick.AddListener(callback);
            var entity = new UIEventEntity_Void(button.onClick, callback);
            return entity;
        }
        public static UIEventEntity AddTo(this UIEventEntity entity, IUIEventBox box)
        {
            box.AddUIEvent(entity);
            return entity;
        }
    }
}
