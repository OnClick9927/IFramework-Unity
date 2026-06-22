using System;
using System.Collections.Generic;
using System.IO;

namespace IFramework
{
    class PrefService : IFramework.ServiceBase
    {
        private IPrefLoader loader;
        private IPrefConverter converter;
        private readonly string baseKey;
        private string dir;
        private Dictionary<string, object> prefs = new Dictionary<string, object>();
        private Dictionary<Type, IPrefContext> contexts = new Dictionary<Type, IPrefContext>();
        public PrefService(IPrefConverter converter, IPrefLoader loader, string baseKey)
        {
            this.converter = converter;
            this.baseKey = baseKey;
            this.loader = loader;
#if UNITY_EDITOR
            dir = $"Assets/Editor/{baseKey}";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
#endif
        }
        public void SaveAllPref()
        {
            if (prefs.Count <= 0) return;
            foreach (var item in prefs)
            {
                SavePref(item.Key, item.Value);
            }
        }
        public void SetPrefContext<T>(PrefContext<T> context) where T : class, new() => contexts[typeof(T)] = context;

        public void ClearPref()
        {
            prefs.Clear();
            foreach (var item in contexts.Values)
            {
                item.ClearValue();
            }
        }
        public object LoadPref(Type type, string key)
        {
            var real_key = $"{baseKey}_{key}_{type}";
            if (prefs.TryGetValue(real_key, out var result))
                return result;
            var str = LoadString(real_key);
            var pref = string.IsNullOrEmpty(str) ? Activator.CreateInstance(type) : converter.FromString(type,str);
            prefs[real_key] = pref;


            if (contexts.TryGetValue(type, out var context))
            {
                (context).SetValue(pref);
                context.key = real_key;
            }

            return pref;
        }


        public void SavePref(string key, object obj)
        {
            var type = obj.GetType();
            var real_key = $"{baseKey}_{key}_{type}";
            SaveString(real_key, converter.ToString(obj, type));
        }


        private string LoadString(string key)
        {
            string json = string.Empty;
#if UNITY_EDITOR
            string path = $"{dir}/{key}.json";
            if (File.Exists(path)) json = File.ReadAllText(path);
#else
                json = loader.Load(key);
#endif

            return json;
        }
        private void SaveString(string key, string json)
        {
#if UNITY_EDITOR
            string path = $"{dir}/{key}.json";
            File.WriteAllText(path, json);
            UnityEditor.AssetDatabase.Refresh();
#else
            loader.Save(key, json);

#endif
        }

        protected override void OnQuit(IServiceCollection game)
        {
            if (prefs.Count <= 0) return;
            SaveAllPref();
            prefs.Clear();
            contexts.Clear();
        }

        protected override void OnUse(IServiceCollection game)
        {

        }
    }

}



