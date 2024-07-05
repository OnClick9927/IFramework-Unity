/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace IFramework.Localization
{
    [LocalizationActorEditorAttribute]

    class PrefabActorEditor : LocalizationMapActorEditor<PrefabActor, GameObject, LocalizationBehavior>
    {
        protected override GameObject Draw(string lan, GameObject value) => EditorGUILayout.ObjectField(lan, value, typeof(GameObject), false) as GameObject;

        protected override GameObject GetDefault() => null;

    }
}
