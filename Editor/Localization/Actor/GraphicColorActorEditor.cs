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
    public class GraphicColorActorEditor : LocalizationMapActorEditor<GraphicColorActor, Color>
    {
        protected override Color Draw(string lan, Color value)
        {
            return EditorGUILayout.ColorField(lan, value);
        }

        protected override Color GetDefault()
        {
            return Color.white;
        }

        protected override SerializableDictionary<string, Color> GetMap(GraphicColorActor context)
        {
            return context.colors;
        }

    }
}
