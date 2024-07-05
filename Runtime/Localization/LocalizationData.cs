/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IFramework.Localization
{
    [CreateAssetMenu]
    public class LocalizationData : ScriptableObject, ILocalizationContext, ISerializationCallbackReceiver
    {
        private Dictionary<string, Dictionary<string, string>> map
            = new Dictionary<string, Dictionary<string, string>>();

        [UnityEngine.SerializeField]
        private SerializableDictionary<string, SerializableDictionary<int, string>> map_reflect
= new SerializableDictionary<string, SerializableDictionary<int, string>>();
        [UnityEngine.SerializeField]
        private List<string> keys = new List<string>();

        public string GetLocalization(string localizationType, string key)
        {
            if (!map.ContainsKey(localizationType)) return string.Empty;
            if (!map[localizationType].ContainsKey(key)) return string.Empty;
            return map[localizationType][key];

        }

        public List<string> GetLocalizationTypes()
        {
            return map.Keys.ToList();
        }

        public List<string> GetLocalizationKeys()
        {
            return keys;
        }

        public void Clear()
        {
            map.Clear();
        }

        public void Add(string lan, string key, string value)
        {
            if (!map.ContainsKey(lan))
                map.Add(lan, new Dictionary<string, string>());
            map[lan][key] = value;
            if (!keys.Contains(key)) { keys.Add(key); }
        }

        public void Add(string lan)
        {
            if (!map.ContainsKey(lan))
                map.Add(lan, new Dictionary<string, string>());
        }

        public void OnBeforeSerialize()
        {
            foreach (var item in map)
            {
                var lan = item.Key;
                SerializableDictionary<int, string> _ref;
                if (!map_reflect.TryGetValue(lan, out _ref))
                {
                    _ref = new SerializableDictionary<int, string>();
                    map_reflect.Add(lan, _ref);
                }
                foreach (var pair in item.Value)
                {
                    var key = pair.Key;
                    var value = pair.Value;
                    var key_index = keys.IndexOf(key);
                    if (key_index != -1)
                    {
                        _ref[key_index] = value;
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {
            foreach (var item in map_reflect)
            {
                var lan = item.Key;
                Dictionary<string, string> _ref;
                if (!map.TryGetValue(lan, out _ref))
                {
                    _ref = new Dictionary<string, string>();
                    map.Add(lan, _ref);
                }
                foreach (var pair in item.Value)
                {
                    var key = pair.Key;
                    var value = pair.Value;
                    var key_value = keys[key];
                    _ref[key_value] = value;

                }

            }
        }
    }
}

