/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace IFramework
{
    public interface ILocalizationActor
    {
        bool enable { get; set; }
        void Execute(string localizationType, LocalizationComponent component);

    }
}
