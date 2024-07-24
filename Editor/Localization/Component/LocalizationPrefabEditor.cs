/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using static IFramework.Localization.LocalizationPrefab;

namespace IFramework.Localization
{

    [CustomEditor(typeof(LocalizationPrefab))]
    class LocalizationPrefabEditor : LocalizationBehaviorEditor<LocalizationPrefab>
    {
        [LocalizationActorEditorAttribute]

        class PrefabActorEditor : LocalizationMapActorEditor<PrefabActor, GameObject, LocalizationBehavior>
        {
            protected override GameObject Draw(string lan, GameObject value) => EditorGUILayout.ObjectField(lan, value, typeof(GameObject), false) as GameObject;
        }
    }
}
