/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Audio
{
    [System.Serializable]
    public class AudioPref
    {
        public Dictionary<int, float> pairs = new Dictionary<int, float>();

        public float GetVolume(int channel)
        {
            float vol = -1;
            pairs.TryGetValue(channel, out vol);
            return vol;
        }

        internal void SetVolume(int channel, float volume)
        {
            pairs[channel] = volume;
        }
    }
    public static class Audio
    {

        internal static IAudioConfig config;
        internal static IAudioPrefRecorder recorder;
        internal static IAudioAsset asset;
        static AudioPref pref;

        private static Dictionary<int, AudioChannel> channels;
        public static GameObject root;
        private static Dictionary<string, AudioAsset> assets;

        public static void Init(IAudioPrefRecorder recorder, IAudioAsset asset)
        {
            Audio.asset = asset;
            Audio.recorder = recorder;
            pref = recorder.Read();
            assets = new Dictionary<string, AudioAsset>();
            channels = new Dictionary<int, AudioChannel>();
            root = new GameObject("Sound");
            root.transform.SetParent(Launcher.Instance.transform);
            Launcher.BindUpdate(OnUpdate);
            Launcher.BindDisable(Dispose);
        }

        public static void SetConfig(IAudioConfig config)
        {
            Audio.config = config;
            if (config != null)
            {
                var values = config.GetChannels();
                foreach (var channel in values)
                {
                    var vol = GetVolume(channel);
                    if (vol != -1)
                        vol = config.GetDefaultVolume(channel);
                    SetVolume(channel, vol);
                }
            }

        }
        public static void SetVolume(int channel, float volume)
        {
            pref.SetVolume(channel, volume);
            recorder.Write(pref);

            AudioChannel chan = GetChannel(channel);
            chan.SetVolume(volume);
        }
        public static float GetVolume(int channel) => pref.GetVolume(channel);
        public static void Play(int sound_id)
        {
            AudioChannel chan = GetChannel(config.GetSoundChannel(sound_id));
            chan.Play(sound_id);
        }
        public static void Stop(int sound_id)
        {
            AudioChannel chan = GetChannel(config.GetSoundChannel(sound_id));
            chan.Stop(config.GetSoundPath(sound_id));
        }

        public static void ShutDown(int channel)
        {
            AudioChannel chan = GetChannel(channel);
            chan.ShutDown();
        }

        static void OnUpdate()
        {
            foreach (var channel in channels.Values)
            {
                channel.Update();
            }
            float delta = Time.deltaTime;
            foreach (var item in assets.Values)
            {
                if (item.MinusTime(delta) <= 0)
                    AddToRelease(item);
            }
            RleaseAssets();
        }

        static void Dispose()
        {
            Launcher.UnBindDisable(Dispose);
            Launcher.UnBindUpdate(OnUpdate);
            foreach (var channel in channels.Values)
                channel.Rlease();
            foreach (var item in assets.Values)
                AddToRelease(item);
            RleaseAssets();
        }
        private static AudioChannel GetChannel(int channel)
        {
            AudioChannel chan;
            if (!channels.TryGetValue(channel, out chan))
            {
                chan = new AudioChannel(channel);
                channels.Add(channel, chan);
            }
            return chan;
        }
        private static Queue<AudioAsset> release = new Queue<AudioAsset>();
        private static void RleaseAssets()
        {
            while (release.Count > 0)
            {
                var item = release.Dequeue();
                assets.Remove(item.path);
                item.ReleaseAsset();
            }
        }
        private static void AddToRelease(AudioAsset asset) => release.Enqueue(asset);
        internal static AudioAsset Prepare(int sound_id)
        {
            string path = config.GetSoundPath(sound_id);
            AudioAsset asset;
            if (!assets.TryGetValue(path, out asset))
            {
                asset = new AudioAsset(path, config.GetSoundExistTime(sound_id));
                assets.Add(path, asset);
            }
            asset.Retain();
            return asset;
        }
        internal static void ReleaseAsset(AudioAsset asset) => asset.Release();
    }
}
