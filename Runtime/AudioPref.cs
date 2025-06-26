/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
namespace IFramework.AudioEx
{
    [System.Serializable]
    public class AudioPref
    {
        [UnityEngine.SerializeField]
        private SerializableDictionary<int, float> pairs = new SerializableDictionary<int, float>();
        [UnityEngine.SerializeField]

        private float MainVolume = 1;
        internal float GetVolume(int channel)
        {
            float vol = -1;
            if (!pairs.TryGetValue(channel, out vol))
            {
                vol = -1;
            }
            return vol;
        }
        internal float GetMainVolume() => MainVolume;
        internal void SetMainVolume(float volume)
        {
            MainVolume = volume;
        }
        internal void SetVolume(int channel, float volume)
        {
            pairs[channel] = volume;
        }
    }
}
