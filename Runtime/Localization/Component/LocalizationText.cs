/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Localization
{

    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Text))]
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/LocalizationText")]

    public class LocalizationText : LocalizationGraphic<UnityEngine.UI.Text>
    {


        public TextValueActor text = new TextValueActor();
        public TextFontActor font = new TextFontActor();
        public TextFontSizeActor fontSize = new TextFontSizeActor();

        protected override List<ILocalizationActor> GetActors()
        {
            var _base = base.GetActors();
            _base.Add(text);
            _base.Add(font);
            _base.Add(fontSize);
            return _base;
        }
    }
}
