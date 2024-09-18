/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using UnityEngine;

namespace IFramework.Audio
{
    public interface IAudioAssetContext
    {
        bool isDone { get; }

        public void Load(string path);
        AudioClip GetClip();
        void Release(string path);
    }

}
