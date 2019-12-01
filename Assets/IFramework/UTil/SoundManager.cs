/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-31
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework
{
    public class SoundManager:Singleton<SoundManager>
    {
        private SoundManager() { }
        public override void Dispose()
        {
            base.Dispose();
            BgmSource = SoundSource = null;
        }

        public AudioSource BgmSource { get; set; }
        public AudioSource SoundSource { get; set; }
        public bool Mute { get { return BgmSource.mute && SoundSource.mute; }set { BgmSource.mute = SoundSource.mute = value; } }
        public bool BgmMute { get { return BgmSource.mute; } set { BgmSource.mute = value; } }
        public bool SoundMute { get { return SoundSource.mute; } set { SoundSource.mute = value; } }
        public bool IsPlaying { get { return BgmSource.isPlaying && SoundSource.isPlaying; } }
        public bool BgmIsPlaying { get { return BgmSource.isPlaying ; } }
        public bool SoundIsPlaying { get { return  SoundSource.isPlaying; } }


        public float Volume { set { BgmSource.volume = SoundSource.volume = value; } }
        public float BgmVolume { get { return BgmSource.volume; } set { BgmSource.volume = value; } }
        public float SoundVolume { get { return SoundSource.volume; } set { SoundSource.volume = value; } }

        public void PlayBgm(AudioClip clip)
        {
            BgmSource.Stop();
            BgmSource.clip = clip;
            BgmSource.Play();
        }
        public void PlayBgm()
        {
            BgmSource.Stop();
            BgmSource.Play();
        }
        public void PlaySound(AudioClip clip, AudioSource source)
        {
            if (source == null)
            {
                source.Stop();
                source.clip = clip;
                source.Play();
            }
            else
            {
                SoundSource.Stop();
                SoundSource.clip = clip;
                SoundSource.Play();
            }
        }

        public void PauseBgm()
        {
            BgmSource.Pause();
        }
        public void UnPauseBgm()
        {
            BgmSource.UnPause();
        }
        public void StopBgm()
        {
            BgmSource.Stop();
        }
    }
}
