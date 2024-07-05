/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using IFramework.Singleton;
using System.Collections.Generic;
namespace IFramework.Localization
{
    public static class Localization
    {
        public const string LocalizationChange = "LocalizationChange";
        public static string localizationType => pref.localizationType;
        public static ILocalizationContext context;
        private static LocalizationPref pref;
        private static ILocalizationPrefRecorder recorder = new MixedRecorder();

        public static void SetRecorder(ILocalizationPrefRecorder recorder)
        {
            (Localization.recorder as MixedRecorder).Add(recorder);
            pref = Localization.recorder.Read();
        }

        public static void SetDefaultLocalizationType(string type)
        {
            if (string.IsNullOrEmpty(localizationType))
                SetLocalizationType(type);
        }
        public static void SetContext(ILocalizationContext context)
        {
            Localization.context = context;
        }
        public static void SetLocalizationType(string type)
        {
            if (localizationType == type) return;
            pref.localizationType = type;
            recorder.Write(pref);
            Events.Publish(LocalizationChange, null);
        }
        public static string GetLocalizationType()
        {
            return pref.localizationType;
        }
        public static string GetLocalization(ILocalizationContext context, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (context == null) return string.Empty;
            var restult = context.GetLocalization(localizationType, key);
            if (string.IsNullOrEmpty(restult))
                return key;
            return restult;
        }
        public static string GetLocalization(string key) => GetLocalization(context, key);

        public static List<string> GetLocalizationTypes(ILocalizationContext context)
        {
            if (context == null)
                return null;
            return context.GetLocalizationTypes();
        }
        public static List<string> GetLocalizationKeys(ILocalizationContext context)
        {
            if (context == null) return null;
            return context.GetLocalizationKeys();
        }

        public static List<string> GetLocalizationTypes() => GetLocalizationTypes(context);
        public static List<string> GetLocalizationKeys() => GetLocalizationKeys(context);
    }
}
