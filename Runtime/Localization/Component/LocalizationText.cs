/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework
{

    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Text))]
    public class LocalizationText : LocalizationComponent
    {


        public TextValueActor textValueActor = new TextValueActor();
        public TextColorActor textColorActor = new TextColorActor();
        public TextFontActor textFontActor = new TextFontActor();  
        public TextFontSizeActor textFontSizeActor = new TextFontSizeActor();

        public UnityEngine.UI.Text text;



        protected override void Awake()
        {
            base.Awake();
            text = GetComponent<UnityEngine.UI.Text>();
        }

        protected override List<ILocalizationActor> GetActors()
        {
            return new List<ILocalizationActor>() {
            textValueActor,textColorActor,textFontActor,textFontSizeActor

            };
        }
    }
}
