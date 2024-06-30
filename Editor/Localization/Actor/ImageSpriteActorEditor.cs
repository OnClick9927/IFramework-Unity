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
    public class ImageSpriteActorEditor : LocalizationMapActorEditor<ImageSpriteActor,Sprite>
    {
        protected override Sprite Draw(string lan, Sprite value)
        {
            return EditorGUILayout.ObjectField(lan, value, typeof(UnityEngine.Sprite), false) as Sprite;

        }

        protected override Sprite GetDefault()
        {
            return null;
        }

        protected override SerializableDictionary<string, Sprite> GetMap(ImageSpriteActor context)
        {
            return context.sprites;
        }

    }
}
