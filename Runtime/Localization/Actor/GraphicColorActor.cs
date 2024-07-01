/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using UnityEngine;

namespace IFramework.Localization
{
    [System.Serializable]
    public class GraphicColorActor : LocalizationActor<LocalizationGraphic>
    {
        public SerializableDictionary<string, Color> colors = new SerializableDictionary<string, Color>();
        protected override void Execute(string localizationType, LocalizationGraphic component)
        {
            component.graphic.color = colors[localizationType];

        }
    }
}
