/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using UnityEditor;

namespace IFramework.Localization
{
    [LocalizationActorEditorAttribute]
    class TextFontSizeActorEditor : LocalizationMapActorEditor<TextFontSizeActor, int,LocalizationText>
    {
        protected override int Draw(string lan, int value) => EditorGUILayout.IntField(lan, value);

        protected override int GetDefault() => 14;


    }
}
