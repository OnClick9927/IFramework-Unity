/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/

using System.Linq;
using System.Threading.Tasks;
using UnityEditor;

namespace IFramework.Localization
{
    [System.Serializable]
    [UnityEditor.InitializeOnLoad]
    class LocalizationSetting : ILocalizationPrefRecorder
    {
        static LocalizationSetting()
        {
            Localization.SetRecorder(context);
        }


        LocalizationPref ILocalizationPrefRecorder.Read()
        {
            return new LocalizationPref()
            {
                localizationType = localizationType,
            };
        }

        void ILocalizationPrefRecorder.Write(LocalizationPref pref)
        {
            this._localizationType = pref.localizationType;
            Save();
        }
        [UnityEngine.SerializeField] private string _localizationType = "CN";
        [UnityEngine.SerializeField] private string _defaultData;
        [UnityEngine.SerializeField] private string _lineReg = "\"";
        [UnityEngine.SerializeField] private string _fieldReg = "\\G(?:^|,)(?:\"((?>[^\"]*)(?>\"\"[^\"]*)*)\"|([^\",]*))";
        [UnityEngine.SerializeField] private string _quotesReg = "\"\"";

        private static LocalizationSetting _context;
        private static LocalizationSetting context
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

        private void Save()
        {
            EditorTools.SaveToPrefs(_context, nameof(LocalizationSetting), false);
        }

        public static string localizationType
        {
            get { return context._localizationType; }
            set
            {
                if (context._localizationType == value) return;
                context._localizationType = value;
                context.Save();

            }
        }
        public static string lineReg
        {
            get { return context._lineReg; }
            set
            {
                if (context._lineReg == value) return; context._lineReg = value;
                context.Save();
            }
        }
        public static string fieldReg
        {
            get { return context._fieldReg; }
            set
            {
                if (context._fieldReg == value) return;
                context._fieldReg = value;
                context.Save();
            }
        }
        public static string quotesReg
        {
            get { return context._quotesReg; }
            set
            {
                if (context._quotesReg == value) return;
                context._quotesReg = value;
                context.Save();
            }
        }
        private static LocalizationData __defaultData;
        public static LocalizationData defaultData
        {
            get
            {
                if (__defaultData == null)
                {
                    __defaultData = AssetDatabase.LoadAssetAtPath<LocalizationData>(context._defaultData);
                }

                return __defaultData;
            }
            set
            {
                var path = AssetDatabase.GetAssetPath(value);
                if (context._defaultData == path) return;
                __defaultData = null;
                context._defaultData = path;
                context.Save();

            }
        }
    }
}
