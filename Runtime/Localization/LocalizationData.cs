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
    public class LocalizationData : ScriptableObject, ILocalizationContext
    {
        [UnityEngine.SerializeField]
        private SerializableDictionary<string, SerializableDictionary<string, string>> map
            = new SerializableDictionary<string, SerializableDictionary<string, string>>();
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
            foreach (var key in map.Keys)
            {
                return map[key].Keys.ToList();
            }
            return null;
        }

        public void Clear()
        {
            map.Clear();
        }

        public void Add(string lan, string key, string value)
        {
            if (!map.ContainsKey(lan))
                map.Add(lan, new SerializableDictionary<string, string>());
            map[lan][key] = value;
        }

        public void Add(string lan)
        {
            if (!map.ContainsKey(lan))
                map.Add(lan, new SerializableDictionary<string, string>());
        }
    }
}

