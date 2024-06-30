/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using UnityEditor;

namespace IFramework
{
    [LocalizationActorAttribute]

    public class TextFontSizeActorEditor : LocalizationMapActorEditor<TextFontSizeActor, int>
    {
        protected override int Draw(string lan, int value) => EditorGUILayout.IntField(lan, value);

        protected override int GetDefault() => 14;

        protected override SerializableDictionary<string, int> GetMap(TextFontSizeActor context) => context.fonts;

    }
}
