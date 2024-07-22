/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
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

        protected override void Execute(string localizationType, LocalizationText component)
        {
            _lastKey = key;
            var format = component.GetLocalization(key);
            try
            {
                component.graphicT.text = string.Format(format, formatArgs);

            }
            catch (System.Exception)
            {

                throw;
            }
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
