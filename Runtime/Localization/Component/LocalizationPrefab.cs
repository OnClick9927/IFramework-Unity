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
        public PrefabActor prefab = new PrefabActor();
        protected override List<ILocalizationActor> GetActors()
        {
            return new List<ILocalizationActor>() {
            prefab
            };
        }
    }
}
