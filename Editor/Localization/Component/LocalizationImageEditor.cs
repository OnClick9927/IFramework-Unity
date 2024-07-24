/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using static IFramework.Localization.LocalizationImage;

namespace IFramework.Localization
{
    [CustomEditor(typeof(LocalizationImage))]
    class LocalizationImageEditor : LocalizationBehaviorEditor<LocalizationImage>
    {
        [LocalizationActorEditorAttribute]
        class ImageSpriteActorEditor : LocalizationMapActorEditor<ImageSpriteActor, Sprite, LocalizationImage>
        {
            protected override Sprite Draw(string lan, Sprite value)
            {
                return EditorGUILayout.ObjectField(lan, value, typeof(UnityEngine.Sprite), false) as Sprite;

            }

        }
    }
}
