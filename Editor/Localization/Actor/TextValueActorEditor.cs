/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [LocalizationActorAttribute]
    public class TextValueActorEditor : LocalizationActorEditor<TextValueActor>
    {

        protected override void OnGUI(LocalizationBehavior component, TextValueActor context)
        {
            var keys = component.GetLocalizationKeys();

            if (keys == null || keys.Count == 0) return;
            var _index = keys.IndexOf(context.key);
            var index = EditorGUILayout.Popup("Key", _index, keys.ToArray());
            if (index >= keys.Count || index == -1)
                index = 0;
            if (index != _index)
            {
                context.key = keys[index];
                SetDirty(component);
            }

            GUI.enabled = false;
            GUILayout.Label("Preview");
            EditorGUILayout.TextField("key", context.key);
            EditorGUILayout.TextField("Localization", component.GetLocalization(context.key));
            GUI.enabled = true;
        }
    }
}
