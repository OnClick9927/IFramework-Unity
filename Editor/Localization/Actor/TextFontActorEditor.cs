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

    class TextFontActorEditor : LocalizationMapActorEditor<TextFontActor, Font>
    {
        protected override Font Draw(string lan, Font value) => EditorGUILayout.ObjectField(lan, value, typeof(Font), false) as Font;

        protected override Font GetDefault() => null;

        protected override SerializableDictionary<string, Font> GetMap(TextFontActor context) => context.fonts;

    }
}
