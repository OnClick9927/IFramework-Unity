/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using UnityEngine;

namespace IFramework.AudioEx
{
    class AudioPlayer
    {
        private AudioSource _source;
        private float volume = 0f;
        private bool _loading;
        public int sound_id { get; private set; }

        public bool lifeEnd { get; private set; }

        public AudioAsset asset { get; private set; }
        public AudioPlayer(AudioSource source) => _source = source;
        private float GetTargetVolume(float percent) => volume * (1 + percent);
        public void SetVolume(float volume)
        {
            this.volume = volume;
            if (!lifeEnd && sound_id != 0)
                _source.volume = GetTargetVolume(Audio.Instance.config.GetSoundVolume(sound_id));
        }


        private void PlayAudio()
        {
            if (lifeEnd) return;
            AudioClip clip = asset.GetClip();
            _source.clip = clip;
            _source.volume = GetTargetVolume(Audio.Instance.config.GetSoundVolume(sound_id));
            _source.loop = Audio.Instance.config.GetSoundLoop(sound_id);
            _source.Play(0);
        }
        public void Play(int sound_id)
        {
            this.sound_id = sound_id;
            asset = Audio.Prepare(sound_id);
            if (asset.isDone)
                PlayAudio();
            else
                _loading = true;
        }
        public void Update()
        {
            if (lifeEnd) return;
            if (_loading)
            {
                if (!asset.isDone) return;
                if (asset.isDone)
                {
                    _loading = false;
                    PlayAudio();
                }
            }
            else
            {
                if (Application.runInBackground)
                {
                    if (!_source.isPlaying)
                        EndLife();
                }
                else
                {
                    if (!_source.isPlaying && Application.isFocused)
                        EndLife();
                }
            }
        }

        public void EndLife()
        {
            if (lifeEnd)
                return;

            lifeEnd = true;
            _loading = false;
            _source.Stop();
            _source.clip = null;
            Audio.ReleaseAsset(asset);
            asset = null;
            this.sound_id = 0;
        }

        public void BeginLife()
        {
            _loading = false;
            lifeEnd = false;
        }
    }

}
