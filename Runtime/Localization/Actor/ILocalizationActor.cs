/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace IFramework.Localization
{
    public interface ILocalizationActor
    {
        bool enable { get; set; }
        void Execute(string localizationType, LocalizationBehavior component);

    }
}
