/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace IFramework
{
    public interface ILocalizationPrefRecorder
    {
        LocalizationPref Read();
        void Write(LocalizationPref pref);
    }
}
