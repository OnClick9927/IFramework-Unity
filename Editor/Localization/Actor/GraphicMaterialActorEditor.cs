/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IFramework
{
    [LocalizationActorAttribute]
    public class GraphicMaterialActorEditor : LocalizationMapActorEditor<GraphicMaterialActor, Material>
    {
        protected override Material Draw(string lan, Material value)
        {
            return EditorGUILayout.ObjectField(lan, value, typeof(Material), false) as Material;
        }

        protected override Material GetDefault()
        {
            return Graphic.defaultGraphicMaterial;
        }

        protected override SerializableDictionary<string, Material> GetMap(GraphicMaterialActor context)
        {
            return context.materials;
        }

    }
}
