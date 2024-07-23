/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
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
            keys.Clear();
        }
        public void ClearLan(string lan)
        {
            if (map.ContainsKey(lan))
                map.Remove(lan);

            if (map.Count == 0)
                Clear();
        }
        public void Add(string lan, string key, string value)
        {
            if (!map.ContainsKey(lan))
                map.Add(lan, new SerializableDictionary<string, string>());
            map[lan][key] = value;
            if (!keys.Contains(key)) { keys.Add(key); }
        }

        public void Add(string lan)
        {
            if (!map.ContainsKey(lan))
                map.Add(lan, new SerializableDictionary<string, string>());
        }



        public void ClearKeys(IList<string> list)
        {
            var lanTypes = this.GetLocalizationTypes();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var key = list[i];

                for (int j = 0; j < lanTypes.Count; j++)
                {
                    var type = lanTypes[j];
                    map[type].Remove(key);
                }

                keys.Remove(key);
            }
        }
    }
}

