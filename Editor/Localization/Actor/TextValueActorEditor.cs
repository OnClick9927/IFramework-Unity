/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Linq;
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

        UnityEditorInternal.ReorderableList argList;
        private void SaveArgs(LocalizationBehavior component, TextValueActor context)
        {
            context.formatArgs = argList.list as string[];
            SetDirty(component);
        }
        private void CreateList(LocalizationBehavior component, TextValueActor context)
        {
            if (argList == null)
                argList = new UnityEditorInternal.ReorderableList(null, typeof(string));

            argList.onAddCallback = (value) => { SaveArgs(component, context); };
            argList.onRemoveCallback = (value) => { SaveArgs(component, context); };
            argList.onChangedCallback = (value) => { SaveArgs(component, context); };
            argList.onReorderCallback = (value) => { SaveArgs(component, context); };
            argList.drawHeaderCallback = (rect) => {
                GUI.Label(rect, "FormatArgs", EditorStyles.boldLabel);
            };
            argList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var src = context.formatArgs[index];
                var tmp = EditorGUI.TextField(rect, src);
                if (tmp != src)
                {
                    context.formatArgs[index] = tmp;
                    SaveArgs(component, context);
                }
            };
            argList.list = context.formatArgs;
        }

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
            else if (_mode == Mode.ReplaceValue)
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

            CreateList(component, context);
            argList.DoLayoutList();


            GUI.enabled = false;
            GUILayout.Label("Preview");
            EditorGUILayout.TextField("key", context.key);

            var format = component.GetLocalization(context.key);
            EditorGUILayout.TextField("Localization", string.Format(format, context.formatArgs));
            GUI.enabled = true;
        }


    }
}
