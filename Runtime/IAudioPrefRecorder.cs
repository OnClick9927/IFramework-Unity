﻿/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
namespace IFramework.AudioEx
{
    public interface IAudioPrefRecorder
    {
        AudioPref Read();
        void Write(AudioPref pref);
    }

}
