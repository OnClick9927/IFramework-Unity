/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/
using UnityEditor;

namespace IFramework.Localization
{

    [EditorWindowCache("Localization")]
    class LocalizationWindow : EditorWindow
    {

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Regex");
            LocalizationSetting.quotesReg = EditorGUILayout.TextField(nameof(LocalizationSetting.quotesReg), LocalizationSetting.quotesReg);
            LocalizationSetting.lineReg = EditorGUILayout.TextField(nameof(LocalizationSetting.lineReg), LocalizationSetting.lineReg);
            LocalizationSetting.fieldReg = EditorGUILayout.TextField(nameof(LocalizationSetting.fieldReg), LocalizationSetting.fieldReg);
            EditorGUILayout.EndVertical();

            LocalizationSetting.defaultData = EditorGUILayout.ObjectField(nameof(LocalizationSetting.defaultData), LocalizationSetting.defaultData, typeof(LocalizationData), false) as LocalizationData;

            if (LocalizationSetting.defaultData)
            {
                var types = LocalizationSetting.defaultData.GetLocalizationTypes();
                if (types.Count==0)
                {
                    return;
                }
                var index = EditorGUILayout.Popup("LanguageType", types.IndexOf(LocalizationSetting.localizationType), types.ToArray());
                Localization.SetLocalizationType(types[index]);
                //GUI.enabled = false;
                //setting.localizationType = EditorGUILayout.TextField(nameof(setting.localizationType), setting.localizationType);

            }


        }



    }
}
