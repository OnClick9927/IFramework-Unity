/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using static IFramework.Localization.LocalizationGraphic;

namespace IFramework.Localization
{
    [CustomEditor(typeof(LocalizationGraphic))]
    class LocalizationGraphicEditor : LocalizationBehaviorEditor<LocalizationGraphic>
    {
        [LocalizationActorEditorAttribute]
        class GraphicColorActorEditor : LocalizationMapActorEditor<GraphicColorActor, Color, LocalizationGraphic>
        {
            protected override Color Draw(string lan, Color value)
            {
                return EditorGUILayout.ColorField(lan, value);
            }
        }
        [LocalizationActorEditorAttribute]
        class GraphicMaterialActorEditor : LocalizationMapActorEditor<GraphicMaterialActor, Material, LocalizationGraphic>
        {
            protected override Material Draw(string lan, Material value)
            {
                return EditorGUILayout.ObjectField(lan, value, typeof(Material), false) as Material;
            }
        }
    }
}
