/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Localization
{

    [ExecuteAlways]
    public abstract class LocalizationBehavior : MonoBehaviour
    {
        public LocalizationData context;
        public List<string> GetLocalizationTypes()
        {
            if (context == null)
                return Localization.GetLocalizationTypes();
            return Localization.GetLocalizationTypes(context);
        }
        public List<string> GetLocalizationKeys()
        {
            if (context == null)
                return Localization.GetLocalizationKeys();
            return Localization.GetLocalizationKeys(context);
        }
        public string GetLocalization(string key)
        {
            if (context == null)
                return Localization.GetLocalization(key);
            return Localization.GetLocalization(context, key);
        }
        public string GetLocalizationType()
        {
            return Localization.GetLocalizationType();
        }

        protected List<ILocalizationActor> actors { get; private set; }

        public void EnableAll(bool enable)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                actors[i].enable = enable;
            }
        }
        private Events.EventEntity entity;

        public List<ILocalizationActor> LoadActors()
        {
            actors = GetActors();

            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                actor.SetBehavior(this);
            }
            return actors;
        }
        protected virtual void Awake()
        {
            LoadActors();
        }
        protected virtual void OnDisable()
        {
            entity.Dispose();
            entity = null;
        }
        protected virtual void OnEnable()
        {
            entity = Events.Subscribe(Localization.LocalizationChange, OnLocalizationChange);
            Execute();
        }

        protected abstract List<ILocalizationActor> GetActors();

        public void AddActor(ILocalizationActor actor)
        {
            if (actors.Contains(actor)) return;
            actors.Add(actor);
            actor.SetBehavior(this);
            actor.Execute(Localization.GetLocalizationType(), this);
        }
        public void RemoveActor(ILocalizationActor actor)
        {
            actors.Remove(actor);
        }
        private void OnLocalizationChange(IEventArgs args)
        {
            Execute();
        }
        private void Execute()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LoadActors();
            }
#endif
            var _type = Localization.GetLocalizationType();
            for (int i = 0; i < actors.Count; i++)
                actors[i].Execute(_type, this);
        }


    }
}
