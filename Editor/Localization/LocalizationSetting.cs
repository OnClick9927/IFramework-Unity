/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/

namespace IFramework
{
    [System.Serializable]
    [UnityEditor.InitializeOnLoad]
    class LocalizationSetting : ILocalizationPrefRecorder
    {
        static LocalizationSetting()
        {
            Localization.instance.SetRecorder(context);
        }
        LocalizationPref ILocalizationPrefRecorder.Read()
        {
            return new LocalizationPref()
            {
                localizationType = this.localizationType,
            };
        }

        void ILocalizationPrefRecorder.Write(LocalizationPref pref)
        {
            this.localizationType = pref.localizationType;
            Save();
        }
        public string localizationType = "CN";
        public LocalizationData defaultData;

        private static LocalizationSetting _context;
        public static LocalizationSetting context
        {
            get
            {
                if (_context == null)
                {
                    _context = EditorTools.GetFromPrefs<LocalizationSetting>(nameof(LocalizationSetting), false);
                    if (_context == null)
                        _context = new LocalizationSetting();
                }
                return _context;
            }
        }

        public void Save()
        {
            EditorTools.SaveToPrefs(_context, nameof(LocalizationSetting), false);
        }
    }
}
