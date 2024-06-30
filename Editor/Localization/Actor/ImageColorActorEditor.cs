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
    public class ImageColorActorEditor : LocalizationMapActorEditor<ImageColorActor, Color>
    {
        protected override Color Draw(string lan, Color value)
        {
            return EditorGUILayout.ColorField(lan, value);

        }

        protected override Color GetDefault()
        {
            return Color.white;
        }

        protected override SerializableDictionary<string, Color> GetMap(ImageColorActor context)
        {
            return context.colors;
        }

    }
}
