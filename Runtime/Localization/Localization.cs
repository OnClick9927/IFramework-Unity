/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using IFramework.Singleton;
using System.Collections.Generic;
namespace IFramework
{
    public class Localization : Singleton<Localization>
    {
        public const string LocalizationChange = "LocalizationChange";
        public string localizationType => pref.localizationType;
        public ILocalizationContext context;
        private LocalizationPref pref;
        private ILocalizationPrefRecorder recorder = new MixedRecorder();

        public void SetRecorder(ILocalizationPrefRecorder recorder)
        {
            (this.recorder as MixedRecorder).Add(recorder);
            pref = this.recorder.Read();
        }
        protected override void OnSingletonInit() { }

        public void SetDefaultLocalizationType(string type)
        {
            if (string.IsNullOrEmpty(localizationType))
                SetLocalizationType(type);
        }
        public void SetContext(ILocalizationContext context)
        {
            this.context = context;
        }
        public void SetLocalizationType(string type)
        {
            if (localizationType == type) return;
            pref.localizationType = type;
            recorder.Write(pref);
            Events.Publish(LocalizationChange, null);
        }
        public string GetLocalizationType()
        {
            return pref.localizationType;
        }
        public string GetLocalization(ILocalizationContext context, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (context == null) return string.Empty;
            var restult = context.GetLocalization(localizationType, key);
            if (string.IsNullOrEmpty(restult))
                return key;
            return restult;
        }
        public string GetLocalization(string key) => GetLocalization(this.context, key);

        public List<string> GetLocalizationTypes(ILocalizationContext context)
        {
            if (context == null)
                return null;
            return context.GetLocalizationTypes();
        }
        public List<string> GetLocalizationKeys(ILocalizationContext context)
        {
            if (context == null) return null;
            return context.GetLocalizationKeys();
        }

        public List<string> GetLocalizationTypes() => GetLocalizationTypes(this.context);
        public List<string> GetLocalizationKeys() => GetLocalizationKeys(this.context);
    }
}
