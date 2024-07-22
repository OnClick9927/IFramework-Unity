/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace IFramework.Localization
{
    [System.Serializable]
    public class TextFontSizeActor : LocalizationMapActor<LocalizationText, int>
    {
        public TextFontSizeActor(bool enable) : base(enable)
        {
        }

        protected override void Execute(string localizationType, LocalizationText component)
        {
            component.graphicT.fontSize = GetValue(localizationType);

        }
    }
}
