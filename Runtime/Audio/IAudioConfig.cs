/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework.Audio
{
    public interface IAudioConfig
    {
        float GetSoundVolume(int sound_id);
        bool GetSoundLoop(int sound_id);
        bool GetSoundCover(int sound_id);
        int GetSoundExistTime(int sound_id);
        string GetSoundPath(int sound_id);
        int GetSoundChannel(int sound_id);
        List<int> GetChannels();
        float GetDefaultVolume(int channel);
    }

}
