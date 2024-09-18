/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using System.Collections;
using UnityEngine;

namespace IFramework.Audio
{
    class AudioPlayer
    {
        private AudioSource _source;
        private float volume = 0f;
        private bool _stop;
        private bool _loading;
        private int sound_id;
        public bool IsWork => _source.isPlaying || _loading;
        public AudioAsset asset { get; private set; }
        public AudioPlayer(AudioSource source)
        {
            _source = source;
            _source.playOnAwake = false;
        }
        private float GetTargetVolume(float percent) => volume * (1 + percent);
        public void SetVolume(float volume)
        {
            this.volume = volume;
            if (_source.isPlaying && sound_id != 0)
                _source.volume = GetTargetVolume(Audio.config.GetSoundVolume(sound_id));
        }

        IEnumerator PlayIE()
        {
            _loading = true;
            while (!asset.isDone || _stop)
            {
                yield return null;
            }
            _loading = false;
            _Play();
        }
        private Coroutine _cor;
        private void StopCoroutine()
        {
            if (_cor != null)
            {
                Launcher.Instance.StopCoroutine(_cor);
                _cor = null;
            }
        }
        private void _Play()
        {
            StopCoroutine();
            if (!_stop)
            {
                AudioClip clip = asset.GetClip();
                _source.clip = clip;
                _source.volume = GetTargetVolume(Audio.config.GetSoundVolume(sound_id));
                _source.loop = Audio.config.GetSoundLoop(sound_id);
                _source.Play();
            }
        }
        public void Play(int sound_id)
        {
            _stop = false;
            this.sound_id = sound_id;
            asset = Audio.Prepare(sound_id);
            if (asset.isDone)
                _Play();
            else
                _cor = Launcher.Instance.StartCoroutine(PlayIE());
        }
        public void Stop()
        {
            if (_stop) return;
            if (_loading)
            {
                StopCoroutine();
                _loading = false;
            }
            _stop = true;
            _source.Stop();
            _source.clip = null;
            Audio.ReleaseAsset(asset);
            asset = null;
        }
    }

}
