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
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Image))]
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/LocalizationImage")]
    public class LocalizationImage : LocalizationGraphic<UnityEngine.UI.Image>
    {
        public ImageSpriteActor sprite = new ImageSpriteActor(true);

        protected override List<ILocalizationActor> GetActors()
        {
            var _base = base.GetActors();
            _base.Add(sprite);
            return _base;

        }
    }
}
