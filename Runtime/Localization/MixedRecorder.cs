/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework.Localization
{
    class MixedRecorder : ILocalizationPrefRecorder
    {
        private List<ILocalizationPrefRecorder> recorders = new List<ILocalizationPrefRecorder>();
        public void Add(ILocalizationPrefRecorder recorder)
        {
            if (recorders.Contains(recorder)) return;
            recorders.Add(recorder);
        }
        LocalizationPref ILocalizationPrefRecorder.Read()
        {
            return recorders[0].Read();
        }

        void ILocalizationPrefRecorder.Write(LocalizationPref pref)
        {
            for (int i = 0; i < recorders.Count; i++)
            {
                recorders[i].Write(pref);
            }
        }
    }
}
