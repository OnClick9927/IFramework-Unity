/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Localization
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/LocalizationPrefab")]
    public class LocalizationPrefab : LocalizationBehavior
    {
        [System.Serializable]

        public class PrefabActor : LocalizationMapActor<LocalizationBehavior, GameObject>
        {
            private GameObject ins;

            public PrefabActor(bool enable) : base(enable)
            {
            }

            protected override void Execute(string localizationType, LocalizationBehavior component)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) return;

                var root = component.transform.root.GetChild(0).gameObject;

                if (UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(component)) return;
#endif
                var prefab = GetValue(localizationType);
                if (prefab != null)
                {
                    if (ins != null)
                    {
                        GameObject.Destroy(ins);
                        ins = null;
                    }

                    ins = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, component.transform);
                }

            }

            public void Create()
            {
                ((ILocalizationActor)this).enable = true;
                ((ILocalizationActor)this).Execute();
            }

            public override GameObject GetDefault()
            {
                return null;
            }
        }
        public PrefabActor prefab = new PrefabActor(true);
        protected override List<ILocalizationActor> GetActors()
        {
            return new List<ILocalizationActor>() {
            prefab
            };
        }
    }
}
