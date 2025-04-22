using IFramework.AudioEx;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
public class Test : MonoBehaviour, IAudioPrefRecorder, IAudioAsset
{
    public AudioConfig AudioConfig;

    private class AudioAssetContext : IAudioAssetContext
    {
        public bool isDone => clip != null;

        public AudioClip GetClip()
        {
            return clip;
        }
        private AudioClip clip;
        public void Load(string path)
        {
            clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        public void Release(string path)
        {
            //if (clip)
            //    Resources.UnloadAsset(clip);
        }
    }
    public IAudioAssetContext CreateContext(string path)
    {
        return new AudioAssetContext();
    }

    public AudioPref Read()
    {
        return JsonUtility.FromJson<AudioPref>(PlayerPrefs.GetString(nameof(AudioPref)));
    }

    public void Write(AudioPref pref)
    {
        PlayerPrefs.SetString(nameof(AudioPref), JsonUtility.ToJson(pref));
    }

    // Start is called before the first frame update
    void Start()
    {
        Audio.Init(this, this);
        Audio.SetDefaultVolume(0, 0.7f);
        Audio.SetDefaultVolume(1, 0.1f);
        Audio.SetConfig(AudioConfig);
        AudioConfig.GenMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Audio.Play(0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Audio.Play(1);

        }
    }
}
