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
    public class GraphicMaterialActor : LocalizationActor<LocalizationGraphic>
    {
        public SerializableDictionary<string, Material> materials = new SerializableDictionary<string, Material>();
        protected override void Execute(string localizationType, LocalizationGraphic component)
        {
            component.graphic.material = materials[localizationType];

        }
    }
}
