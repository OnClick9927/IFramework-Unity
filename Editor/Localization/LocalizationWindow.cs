/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-06-29
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace IFramework
{

    [EditorWindowCache("Localization")]
    public class LocalizationWindow : EditorWindow
    {
        LocalizationSetting setting => LocalizationSetting.context;

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            setting.defaultData = EditorGUILayout.ObjectField(nameof(setting.defaultData), setting.defaultData, typeof(LocalizationData), false) as LocalizationData;

            if (setting.defaultData)
            {
                var types = setting.defaultData.GetLocalizationTypes();
                var index = EditorGUILayout.Popup("LanguageType",types.IndexOf(setting.localizationType), types.ToArray());
                Localization.instance.SetLocalizationType(types[index]);
                //GUI.enabled = false;
                //setting.localizationType = EditorGUILayout.TextField(nameof(setting.localizationType), setting.localizationType);

            }

            if (EditorGUI.EndChangeCheck())
            {
                setting.Save();
            }

        }



    }
}
