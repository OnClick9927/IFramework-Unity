/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Text.RegularExpressions;
using System;
using UnityEngine;

namespace IFramework.Localization
{
    [System.Serializable]
    public class TextValueActor : LocalizationActor<LocalizationText>
    {
        public string key;
        private string _lastKey;
        public string[] formatArgs = new string[0];

        public TextValueActor(bool enable) : base(enable)
        {
        }

        public string GetTargetText(LocalizationBehavior component, out Exception err)
        {
            err = null;
            var format = component.GetLocalization(key);
            if (Regex.Match(format, "^{[0-9]*}$") == null) return format;
            try
            {
                return string.Format(format, formatArgs);
            }
            catch (System.Exception ex)
            {
                err = ex;
                return format;
            }
        }
        protected override void Execute(string localizationType, LocalizationText component)
        {
            _lastKey = key;
            Exception err;
            component.graphicT.text = GetTargetText(component, out err);
            if (err != null)
                throw err;

        }
        public void SetKey(string key)
        {
            this.key = key;
            ((ILocalizationActor)this).enable = true;
            ((ILocalizationActor)this).Execute();
        }
        protected override bool NeedExecute(string localizationType)
        {
            var _base = base.NeedExecute(localizationType);
            bool self = _lastKey != this.key;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                self = true;
#endif
            return self || _base;
        }
    }
}
