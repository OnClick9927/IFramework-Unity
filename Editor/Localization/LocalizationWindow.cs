/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework.Localization
{

    [EditorWindowCache("Localization")]
    class LocalizationWindow : EditorWindow
    {

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("CSV Regex");
            LocalizationSetting.quotesReg = EditorGUILayout.TextField(nameof(LocalizationSetting.quotesReg), LocalizationSetting.quotesReg);
            LocalizationSetting.lineReg = EditorGUILayout.TextField(nameof(LocalizationSetting.lineReg), LocalizationSetting.lineReg);
            LocalizationSetting.fieldReg = EditorGUILayout.TextField(nameof(LocalizationSetting.fieldReg), LocalizationSetting.fieldReg);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("YouDao");
            LocalizationSetting.youDaoAppId = EditorGUILayout.TextField(nameof(LocalizationSetting.youDaoAppId), LocalizationSetting.youDaoAppId);
            LocalizationSetting.youDaoAppSecret = EditorGUILayout.TextField(nameof(LocalizationSetting.youDaoAppSecret), LocalizationSetting.youDaoAppSecret);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Register"))
            {
                Application.OpenURL("https://ai.youdao.com/product-fanyi-text.s");
            }
            if (GUILayout.Button("Code && Language"))
            {
                Application.OpenURL("https://ai.youdao.com/DOCSIRMA/html/trans/api/wbfy/index.html");
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            LocalizationSetting.defaultData = EditorGUILayout.ObjectField(nameof(LocalizationSetting.defaultData), LocalizationSetting.defaultData, typeof(LocalizationData), false) as LocalizationData;

            if (LocalizationSetting.defaultData)
            {
                var types = LocalizationSetting.defaultData.GetLocalizationTypes();
                if (types.Count == 0)
                {
                    return;
                }
                var index = EditorGUILayout.Popup("LanguageType", types.IndexOf(LocalizationSetting.localizationType), types.ToArray());
                Localization.SetLocalizationType(types[Mathf.Clamp(index, 0, types.Count)]);
                GUILayout.Label("Type Reflect", EditorStyles.boldLabel);
                for (var i = 0; i < types.Count; i++)
                {
                    var type = types[i];
                    var src = LocalizationSetting.GetLocalizationTypeReflect(type);
                    var tmp = EditorGUILayout.TextField(type, src);
                    if (tmp != src)
                    {
                        LocalizationSetting.SetLocalizationTypeReflect(type, tmp);
                    }
                }

            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            if (GUILayout.Button("Import TMP Extend"))
            {
                string path = $"{EditorTools.pkgPath}/Package Resources/LocalizationTmp Extend.unitypackage";
                AssetDatabase.ImportPackage(path, true);
            }
        }




    }
}
