/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework.Localization
{
    [LocalizationActorEditorAttribute]
    class TextValueActorEditor : LocalizationActorEditor<TextValueActor>
    {
        private enum Mode
        {
            Nomal,
            NewKey,
            ReplaceValue
        }
        private Mode _mode;
        private string key;
        private string value;

        protected override void OnGUI(LocalizationBehavior component, TextValueActor context)
        {
            var lan = Localization.GetLocalizationType();
            EditorGUILayout.LabelField(nameof(Localization), lan);
            _mode = (Mode)EditorGUILayout.EnumPopup(nameof(Mode), _mode);
            if (_mode == Mode.Nomal)
            {
                var keys = component.GetLocalizationKeys();

                if (keys == null || keys.Count == 0) return;
                var _index = keys.IndexOf(context.key);
                var index = EditorGUILayout.Popup("Key", _index, keys.ToArray());
                if (index >= keys.Count || index == -1)
                    index = 0;
                if (index != _index)
                {
                    context.SetKey(keys[index]);
                    SetDirty(component);
                }
            }
            else if (_mode== Mode.ReplaceValue)
            {
                value = EditorGUILayout.TextField(nameof(value), value);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("go", GUILayout.Width(30)))
                {
                    var data = component.context;
                    key = context.key;
                    bool bo = EditorUtility.DisplayDialog("same key exist", $"replace \n key {key} \n" +
                              $"value: {data.GetLocalization(lan, key)} => {value}", "yes", "no");
                    if (!bo)
                    {
                        GUIUtility.ExitGUI();
                        return;
                    }

                    data.Add(lan, key, value);
                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssetIfDirty(data);
                    context.SetKey(key);
                    SetDirty(component);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                key = EditorGUILayout.TextField(nameof(key), key);
                value = EditorGUILayout.TextField(nameof(value), value);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    var data = component.context;
                    if (string.IsNullOrEmpty(key))
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("value can not be null"));
                        GUIUtility.ExitGUI();
                        return;
                    }
                    var keys = data.GetLocalizationKeys();
                    if (keys.Contains(key))
                    {
                        bool bo = EditorUtility.DisplayDialog("same key exist", $"replace \n key {key} \n" +
                              $"value: {data.GetLocalization(lan, key)} => {value}", "yes", "no");
                        if (!bo)
                        {
                            GUIUtility.ExitGUI();
                            return;
                        }
                    }

                    data.Add(lan, key, value);
                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssetIfDirty(data);
                    context.SetKey(key);
                    SetDirty(component);

                }
                GUILayout.EndHorizontal();
            }

            GUI.enabled = false;
            GUILayout.Label("Preview");
            EditorGUILayout.TextField("key", context.key);
            EditorGUILayout.TextField("Localization", component.GetLocalization(context.key));
            GUI.enabled = true;
        }

     
    }
}
