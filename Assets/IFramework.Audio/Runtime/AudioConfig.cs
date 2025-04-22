/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IFramework.AudioEx
{
    [CreateAssetMenu]
    public class AudioConfig : UnityEngine.ScriptableObject, IAudioConfig
    {
        [Serializable]
        public class SoundData
        {
            public int id;
            public int channel;
            public bool loop;
            public bool cover;
            public int existTime;
            public string clip;
            public float volume;
        }
        public List<string> channels;

        public List<SoundData> _sounds = new List<SoundData>();
        private Dictionary<int, SoundData> _map;
        public void GenMap()
        {
            _map = _sounds.ToDictionary(x => x.id);
        }
        public SoundData Get(int id)
        {
            if (_map==null)
            {
                _map = _sounds.ToDictionary(x => x.id);
            }
            SoundData data = null;
            _map.TryGetValue(id, out data);
            return data;
        }
        public int GetSoundChannel(int sound_id)=> Get(sound_id).channel;
        public bool GetSoundCover(int sound_id) => Get(sound_id).cover;

        public int GetSoundExistTime(int sound_id) => Get(sound_id).existTime;

        public bool GetSoundLoop(int sound_id) => Get(sound_id).loop;

        public string GetSoundPath(int sound_id) => Get(sound_id).clip;

        public float GetSoundVolume(int sound_id) => Get(sound_id).volume;

  
    }

}
