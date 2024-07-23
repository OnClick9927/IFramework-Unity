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
            argList.multiSelect = true;
            argList.onAddCallback = (value) =>
            {
                var array = argList.list as string[];
                ArrayUtility.Add(ref array, "newArg");
                argList.list = array;
                SaveArgs(component, context);
            };
            argList.onRemoveCallback = (value) =>
            {
                var indexes = argList.selectedIndices;
                var array = argList.list as string[];

                for (int i = indexes.Count - 1; i >= 0; i--)
                {
                    ArrayUtility.RemoveAt(ref array, indexes[i]);
                }
                argList.list = array;
                argList.ClearSelection();
                SaveArgs(component, context);
                GUIUtility.ExitGUI();
            };
            argList.onChangedCallback = (value) => { SaveArgs(component, context); };
            argList.onReorderCallback = (value) => { SaveArgs(component, context); };
            argList.drawHeaderCallback = (rect) =>
            {
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
            var __mode = (Mode)EditorGUILayout.EnumPopup(nameof(Mode), _mode);
            if (__mode != _mode)
            {
                _mode = __mode;
                switch (_mode)
                {
                    case Mode.Nomal:
                    case Mode.NewKey:
                        key = value = string.Empty;
                        break;
                    case Mode.ReplaceValue:
                        key = context.key;
                        value = component.GetLocalization(key);
                        break;
                }
            }
            switch (_mode)
            {
                case Mode.Nomal:
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
                    break;
                case Mode.NewKey:
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
                    break;
                case Mode.ReplaceValue:
                    {
                        value = EditorGUILayout.TextField(nameof(value), value);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("go", GUILayout.Width(30)))
                        {
                            var data = component.context;
                            //key = context.key;
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
                    break;
            }

            CreateList(component, context);
            argList.DoLayoutList();


            //GUI.enabled = false;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Preview");
            EditorGUILayout.LabelField("key", context.key);
            System.Exception err = null;
            var format = context.GetTargetText(component, out err);
            if (err != null)
                EditorGUILayout.HelpBox(err.Message, MessageType.Error, true);
            EditorGUILayout.LabelField("Localization", format);

            GUILayout.Space(5);
            GUILayout.EndVertical();

            //GUI.enabled = true;
        }


    }
}
