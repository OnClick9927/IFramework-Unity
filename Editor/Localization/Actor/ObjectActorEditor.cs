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

    class ObjectActorEditor<T> : LocalizationMapActorEditor<ObjectActor<T>, T, LocalizationBehavior> where T : UnityEngine.Object
    {
        protected override T Draw(string lan, T value) => EditorGUILayout.ObjectField(lan, value, typeof(T), false) as T;

        protected override T GetDefault() => null;


    }
}
