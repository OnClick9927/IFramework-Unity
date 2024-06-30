/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using System.Runtime.Remoting.Contexts;
using UnityEditor;
using UnityEngine;

namespace IFramework
{

    [LocalizationActorAttribute]
    public class TextColorActorEditor : LocalizationMapActorEditor<TextColorActor,Color>
    {
        protected override Color Draw(string lan, Color value)
        {
            return EditorGUILayout.ColorField(lan, value);
        }

        protected override Color GetDefault()
        {
            return Color.white;
        }

        protected override SerializableDictionary<string, Color> GetMap(TextColorActor context)
        {
            return context.colors;
        }

    }
}
