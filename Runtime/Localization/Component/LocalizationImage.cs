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

    public class LocalizationImage : LocalizationComponent
    {
        public UnityEngine.UI.Image image;
        public ImageSpriteActor imageSpriteActor = new ImageSpriteActor();
        public ImageColorActor imageColorActor = new ImageColorActor();
        protected override void Awake()
        {
            image = GetComponent<UnityEngine.UI.Image>();
            base.Awake();
        }
        protected override List<ILocalizationActor> GetActors()
        {
            return new List<ILocalizationActor>() {
            imageSpriteActor,imageColorActor
            };
        }
    }
}
