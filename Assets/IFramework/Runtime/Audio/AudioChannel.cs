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
    class AudioChannel
    {
        private Queue<AudioPlayer> sleeps = new Queue<AudioPlayer>();
        private List<AudioPlayer> players = new List<AudioPlayer>();
        private int index;
        public int channel { get; private set; }
        private float volume = 0.5f;
        private AudioPlayer Get()
        {
            AudioPlayer player = null;
            if (sleeps.Count > 0)
                player = sleeps.Dequeue();
            else
            {
                var source = new GameObject($"{channel}_{index++}").AddComponent<AudioSource>();
                source.transform.SetParent(Audio.root.transform);
                player = new AudioPlayer(source);
            }
            player.SetVolume(volume);
            players.Add(player);

            return player;
        }

        public AudioChannel(int channel)
        {
            this.channel = channel;
        }
        public void Play(int sound_id)
        {
            if (Audio.config.GetSoundCover(sound_id))
                ShutDown();
            Get().Play(sound_id);
        }

        public void SetVolume(float volume)
        {
            this.volume = volume;
            for (int i = 0; i < players.Count; i++)
                players[i].SetVolume(volume);
        }
        private void ShutDown(AudioPlayer player)
        {
            player.Stop();
            players.Remove(player);
            sleeps.Enqueue(player);
        }
        public void Update()
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (!players[i].IsWork)
                    ShutDown(players[i]);
            }
        }
        public void ShutDown()
        {
            for (int i = players.Count - 1; i >= 0; i--)
                ShutDown(players[i]);
        }
        public void Rlease()
        {
            ShutDown();
        }

        public void Stop(string path)
        {
            var player = players.Find(x => x.asset.path == path);
            if (player != null)
                ShutDown(player);
        }
    }

}
