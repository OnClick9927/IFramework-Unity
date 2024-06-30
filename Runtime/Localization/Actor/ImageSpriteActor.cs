/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace IFramework
{
    [System.Serializable]
    public class ImageSpriteActor : LocalizationActor<LocalizationImage>
    {
        public SerializableDictionary<string, UnityEngine.Sprite> sprites = new SerializableDictionary<string, UnityEngine.Sprite>();
        protected override void Execute(string localizationType, LocalizationImage component)
        {
            component.image.sprite = sprites[localizationType];
        }
    }
}
