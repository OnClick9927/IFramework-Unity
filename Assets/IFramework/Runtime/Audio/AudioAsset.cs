/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using UnityEngine;

namespace IFramework.Audio
{
    class AudioAsset
    {
        private int refCount;
        private readonly float time;
        private float _time;
        public string path;
        private IAudioAssetContext context;

        public AudioAsset(string path, float life)
        {
            this.path = path;
            context = Audio.asset.CreateContext(path);
            context.Load(path);
            this.time = life;
        }

        public bool isDone => context.isDone;
        public AudioClip GetClip() => context.GetClip();
        public void Retain()
        {
            _time = time;
            refCount++;
        }
        public float MinusTime(float deltaTime)
        {
            if (refCount != 0)
                return time;
            _time -= deltaTime;
            return _time;
        }
        public void Release()
        {
            refCount--;
        }

        public void ReleaseAsset()
        {
            context.Release(path);
        }
    }

}
