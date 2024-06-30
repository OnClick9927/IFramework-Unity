/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace IFramework
{
    [System.Serializable]
    public class TextFontSizeActor : LocalizationActor<LocalizationText>
    {
        public SerializableDictionary<string, int> fonts = new SerializableDictionary<string, int>();
        protected override void Execute(string localizationType, LocalizationText component)
        {
            component.graphicT.fontSize = fonts[localizationType];

        }
    }
}
