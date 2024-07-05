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
    public abstract class LocalizationActorEditor<T> : ILocalizationActorEditor where T : class, ILocalizationActor
    {
        void ILocalizationActorEditor.OnGUI(string name, LocalizationBehavior component, object value)
        {
            T context = value as T;
            var on = EditorGUILayout.Toggle(context.enable, EditorStyles.toolbarPopup);
            if (on != context.enable)
            {
                context.enable = on;
                SetDirty(component);

            }

            var rect = GUILayoutUtility.GetLastRect();
            rect.x += 10;
            rect.width -= 10;
            GUI.Toggle(rect, on, name);
            if (context.enable)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                OnGUI(component, context);
                GUILayout.EndVertical();
            }


        }
        protected abstract void OnGUI(LocalizationBehavior component, T context);

        protected void SetDirty(LocalizationBehavior component)
        {
            EditorUtility.SetDirty(component);

        }

    
    }
}
