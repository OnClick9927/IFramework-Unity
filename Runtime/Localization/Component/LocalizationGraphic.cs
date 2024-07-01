/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine.UI;

namespace IFramework
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Graphic))]

    public class LocalizationGraphic : LocalizationBehavior
    {
        public Graphic graphic { get; private set; }
        public GraphicColorActor color = new GraphicColorActor();
        public GraphicMaterialActor material = new GraphicMaterialActor();
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
