/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.AudioEx
{
    [MonoSingletonPath(nameof(Audio))]
    [AddComponentMenu("")]
    public class Audio : MonoSingleton<Audio>
    {

        internal IAudioConfig config;
        internal IAudioPrefRecorder recorder;
        internal IAudioAsset asset;
        AudioPref pref;

        private Dictionary<int, AudioChannel> channels;
        private static Audio _ins;
        private Dictionary<string, AudioAsset> assets;

        //internal static Audio ins
        //{
        //    get
        //    {
        //        if (_ins == null)
        //        {
        //            _ins = new GameObject("Audio").AddComponent<Audio>();
        //            DontDestroyOnLoad(ins.gameObject);
        //        }
        //        return _ins;
        //    }
        //}

        public static void Init(IAudioPrefRecorder recorder, IAudioAsset asset)
        {
            Instance.asset = asset;
            Instance.recorder = recorder;
            Instance.pref = recorder.Read();
            if (Instance.pref == null)
                Instance.pref = new AudioPref();
            if (Instance.assets == null)
                Instance.assets = new Dictionary<string, AudioAsset>();
            if (Instance.channels == null)
                Instance.channels = new Dictionary<int, AudioChannel>();
        }
        public static void SetDefaultVolume(int channel, float vol)
        {
            if (GetVolume(channel) != -1) return;
            SetVolume(channel, vol);
        }
        public static void SetConfig(IAudioConfig config)
        {
            Instance.config = config;
        }


        private void Update()
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
        private void OnDisable()
        {

            foreach (var channel in channels.Values)
                channel.StopChannel();
            foreach (var item in assets.Values)
                AddToRelease(item);
            RleaseAssets();
        }
        public static void SetVolume(Enum channel, float volume)
        {
            SetVolume(Convert.ToInt32(channel), volume);
        }
        public static float GetVolume(Enum channel) => GetVolume(Convert.ToInt32(channel));

        public static void SetVolume(int channel, float volume)
        {
            Instance.pref.SetVolume(channel, volume);
            Instance.recorder.Write(Instance.pref);

            AudioChannel chan = GetChannel(channel);
            chan.SetVolume(volume);
        }
        public static float GetVolume(int channel) => Instance.pref.GetVolume(channel);
        public static void Play(int sound_id)
        {
            AudioChannel chan = GetChannel(Instance.config.GetSoundChannel(sound_id));
            chan.Play(sound_id);
        }
        public static void Stop(int sound_id, bool all = false)
        {
            AudioChannel chan = GetChannel(Instance.config.GetSoundChannel(sound_id));
            chan.Stop(sound_id, all);
        }
        public static void StopAllChannel()
        {
            foreach (var item in Instance.channels.Values)
            {
                item.StopChannel();
            }
        }

        public static void StopChannel(int channel)
        {
            AudioChannel chan = GetChannel(channel);
            chan.StopChannel();
        }


        private static AudioChannel GetChannel(int channel)
        {
            AudioChannel chan;
            if (!Instance.channels.TryGetValue(channel, out chan))
            {
                chan = new AudioChannel(channel);
                chan.SetVolume(GetVolume(channel));
                Instance.channels.Add(channel, chan);
            }
            return chan;
        }
        private static Queue<AudioAsset> release = new Queue<AudioAsset>();
        private static void RleaseAssets()
        {
            while (release.Count > 0)
            {
                var item = release.Dequeue();
                Instance.assets.Remove(item.path);
                item.ReleaseAsset();
            }
        }
        private static void AddToRelease(AudioAsset asset) => release.Enqueue(asset);
        internal static AudioAsset Prepare(int sound_id)
        {
            string path = Instance.config.GetSoundPath(sound_id);
            AudioAsset asset;
            if (!Instance.assets.TryGetValue(path, out asset))
            {
                asset = new AudioAsset(path, Instance.config.GetSoundExistTime(sound_id));
                Instance.assets.Add(path, asset);
            }
            asset.Retain();
            return asset;
        }
        internal static void ReleaseAsset(AudioAsset asset) => asset.Release();
    }
}
