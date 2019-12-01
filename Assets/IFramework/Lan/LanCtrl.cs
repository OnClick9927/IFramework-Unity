/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace IFramework
{
    public class LanCtrl : ISingleton
    {
        private LanCtrl() { }
        public void Dispose()
        {
            SingletonProperty<LanCtrl>.Dispose();
            lanPairs.Clear();
            lanPairLoaders.Clear();
            lanObservers.Clear();
        }
        void ISingleton.OnSingletonInit()
        {
            lanPairs = new List<LanPair>();
            lanPairLoaders = new List<Func<List<LanPair>>>();
            lanObservers = new List<LanObserver>();
            Init();
        }
        private void Init() {
            List<LanPair> tmpPairs = new List<LanPair>();
            var types = typeof(ILanguageLoader).GetSubTypesInAssemblys()
                   .ToList()
                   .FindAll((type) => { return type.IsDefined(typeof(LanguageLoaderAttribute), true); });
            types.ForEach((type) => {
                var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                     .ToList()
                     .Find((c) => { return c.GetParameters().Length == 0; });
                ILanguageLoader loader = (ILanguageLoader)ctor.Invoke(null);

                tmpPairs.AddRange(loader.Load());
                loader.Dispose();
            });

            tmpPairs.ForEach((tmpPair) => {
                LanPair pair = lanPairs.Find((p) => { return p.Lan == tmpPair.Lan && p.key == tmpPair.key; });
                if (pair != null && pair.Value != tmpPair.Value)
                    pair.Value = tmpPair.Value;
                else
                    lanPairs.Add(tmpPair);
            });
            tmpPairs.Clear();
            Fresh();
        }
        private static LanCtrl Instance { get { return SingletonProperty<LanCtrl>.Instance; } }
        private List<LanPair> lanPairs;
        private Dictionary<string, List<LanPair>> keyDic;
        public static List<Func<List<LanPair>>> lanPairLoaders;
        public static void AddlanPairLoader(Func<List<LanPair>> loader)
        {
            lanPairLoaders.Add(loader);
        }
        public static void LoadLanPairs(bool reWrite = true)
        {
            List<LanPair> tmpPairs = new List<LanPair>();
            lanPairLoaders.ForEach((loader) => {
                List<LanPair> result = loader.Invoke();
                if (result != null && result.Count > 0)
                    tmpPairs.AddRange(result);
            });
            tmpPairs.ForEach((tmpPair) => {
                LanPair pair = Instance.lanPairs.Find((p) => { return p.Lan == tmpPair.Lan && p.key == tmpPair.key; });
                if (pair != null && reWrite && pair.Value != tmpPair.Value)
                    pair.Value = tmpPair.Value;
                else
                    Instance.lanPairs.Add(tmpPair);
            });
            tmpPairs.Clear();
            Instance.Fresh();
        }
        public static void LoadLanPair(Func<List<LanPair>> loader, bool reWrite = true)
        {
            List<LanPair> tmpPairs = loader.Invoke();
            tmpPairs.ForEach((tmpPair) => {
                LanPair pair = Instance.lanPairs.Find((p) => { return p.Lan == tmpPair.Lan && p.key == tmpPair.key; });
                if (pair != null && reWrite && pair.Value != tmpPair.Value)
                    pair.Value = tmpPair.Value;
                else
                    Instance.lanPairs.Add(tmpPair);
            });
            tmpPairs.Clear();
            Instance.Fresh();
        }
        private void Fresh()
        {
            keyDic = lanPairs.GroupBy(lanPair => { return lanPair.key; }, (key, list) => { return new { key, list }; })
                    .ToDictionary((v) => { return v.key; }, (v) => { return v.list.ToList(); });
            if (ObserveEvent != null) ObserveEvent();
        }

        private event Action ObserveEvent;
        private List<LanObserver> lanObservers;
        private SystemLanguage lan = SystemLanguage.Unknown;
        public static SystemLanguage Lan
        {
            get { return Instance.lan; }
            set
            {
                if (Instance.lan == value) return;
                Instance.lan = value;
                if (Instance.ObserveEvent != null)
                    Instance.ObserveEvent();
            }
        }

        public static LanObserver CreatLanObserver(string key, SystemLanguage fallback, bool autoStart = true)
        {
            if (!Instance.keyDic.ContainsKey(key)) throw new Exception(string.Format("Key Not Found {0}", key));
            List<LanPair> pairs = Instance.keyDic[key];
            var fallbackPair = pairs.Find((p) => { return p.Lan == fallback; });
            if (fallbackPair == null) Log.W(string.Format("Fallback Language Not Exist Key: {0} : Lan: {1}", key, fallback));
            LanObserver observer = new LanObserver(key, fallback, autoStart);
            return observer;
        }
        public class LanObserver : IDisposable
        {
            private bool isDisposed;
            private bool isPaused;
            public string Key { get; private set; }
            public SystemLanguage CurLan { get { return Instance.lan; } }
            public SystemLanguage Fallback { get; private set; }
            private List<LanPair> Pairs { get { return Instance.keyDic[Key]; } }

            internal LanObserver(string key, SystemLanguage fallback, bool autoStart)
            {
                Instance.lanObservers.Add(this);
                this.Key = key;
                this.Fallback = fallback;
                Instance.ObserveEvent += ObserveEvent;
                isDisposed = false;
                Pause();
                if (autoStart) Start();
            }
            private Action<string, SystemLanguage, string> observeEvent;
            public LanObserver ObserveEvent(Action<string, SystemLanguage, string> eve)
            {
                this.observeEvent = eve;
                return this;
            }
            private string GetValueInPairs()
            {
                LanPair pair = Pairs.Find(p => { return p.Lan == CurLan; });
                if (pair == null)
                    pair = Pairs.Find(p => { return p.Lan == Fallback; });
                return pair == null ? string.Empty : pair.Value;
            }
            private void ObserveEvent()
            {
                if (isDisposed || isPaused) return;
                if (observeEvent != null)
                    observeEvent(Key, CurLan, GetValueInPairs());
            }
            public void Start() { UnPause(); }

            public void Pause()
            {
                isPaused = true;
            }
            public void UnPause()
            {
                isPaused = false;
            }
            public void Dispose()
            {
                Pause();
                isDisposed = true;
                observeEvent = null;
                Instance.ObserveEvent -= ObserveEvent;
                Instance.lanObservers.Remove(this);
            }
        }
    }

}
