/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.Localization
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Graphic))]
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/LocalizationGraphic")]
    public class LocalizationGraphic : LocalizationBehavior
    {
        [System.Serializable]
        public class GraphicColorActor : LocalizationMapActor<LocalizationGraphic, Color>
        {
            public GraphicColorActor(bool enable) : base(enable)
            {
            }

            public override Color GetDefault()
            {
                return Color.white;
            }

            protected override void Execute(string localizationType, LocalizationGraphic component)
            {
                component.graphic.color = GetValue(localizationType);

            }
        }
        [System.Serializable]
        public class GraphicMaterialActor : LocalizationMapActor<LocalizationGraphic, Material>
        {
            public SerializableDictionary<string, Material> materials = new SerializableDictionary<string, Material>();

            public GraphicMaterialActor(bool enable) : base(enable)
            {
            }

            protected override void Execute(string localizationType, LocalizationGraphic component)
            {
                component.graphic.material = GetValue(localizationType);

            }

            public override Material GetDefault()
            {
                return UnityEngine.UI.Graphic.defaultGraphicMaterial;
            }
        }
        public Graphic graphic { get; private set; }
        public GraphicColorActor color = new GraphicColorActor(false);
        public GraphicMaterialActor material = new GraphicMaterialActor(false);
        protected override void Awake()
        {
            graphic = GetComponent<Graphic>();
            base.Awake();
        }
        protected override List<ILocalizationActor> GetActors()
        {
            return new List<ILocalizationActor>() {
                color,material,
           };
        }
    }

    public class LocalizationGraphic<T> : LocalizationGraphic where T : Graphic
    {
        public T graphicT { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            graphicT = graphic as T;
        }
    }
}
