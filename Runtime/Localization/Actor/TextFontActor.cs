/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    [System.Serializable]
    public class TextFontActor : LocalizationActor<LocalizationText>
    {
        public SerializableDictionary<string, Font> fonts = new SerializableDictionary<string, Font>();
        protected override void Execute(string localizationType, LocalizationText component)
        {
            component.text.font = fonts[localizationType];

        }
    }
}
