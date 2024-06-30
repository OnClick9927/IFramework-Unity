/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Image))]

    public class LocalizationImage : LocalizationGraphic<UnityEngine.UI.Image>
    {
        public ImageSpriteActor sprite = new ImageSpriteActor();

        protected override List<ILocalizationActor> GetActors()
        {
            var _base = base.GetActors();
            _base.Add(sprite);
            return _base;

        }
    }
}
