/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IFramework.UI
{
    public abstract class UIObjectView : GameObjectView
    {
        protected void BindInputField(InputField input, UnityAction<string> callback)
        {
            input.onValueChanged.AddListener(callback);
        }
        protected void BindToggle(Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
        }
        protected void BindSlider(Slider slider, UnityAction<float> callback)
        {
            slider.onValueChanged.AddListener(callback);
        }
        protected void BindOnEndEdit(InputField input, UnityAction<string> callback)
        {
            input.onEndEdit.AddListener(callback);
        }
        protected void BindOnValidateInput(InputField input, InputField.OnValidateInput callback)
        {
            input.onValidateInput = callback;
        }
        protected void BindButton(Button button, UnityAction callback)
        {
            button.onClick.AddListener(callback);
        }
    }
}
