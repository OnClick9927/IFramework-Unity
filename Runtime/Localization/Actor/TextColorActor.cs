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
    public class TextColorActor : LocalizationActor<LocalizationText>
    {
        public SerializableDictionary<string, Color> colors = new SerializableDictionary<string, Color>();
        protected override void Execute(string localizationType, LocalizationText component)
        {
            component.text.color = colors[localizationType];

        }
    }
}
