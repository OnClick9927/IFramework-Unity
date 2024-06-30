/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
namespace IFramework
{
    public abstract class LocalizationComponent : MonoBehaviour
    {
        public LocalizationObject context;
        public List<string> GetLocalizationTypes()
        {
            if (context == null)
                return Localization.instance.GetLocalizationTypes();
            return Localization.instance.GetLocalizationTypes(context);
        }
        public List<string> GetLocalizationKeys()
        {
            if (context == null)
                return Localization.instance.GetLocalizationKeys();
            return Localization.instance.GetLocalizationKeys(context);
        }
        public string GetLocalization(string key)
        {
            if (context == null)
                return Localization.instance.GetLocalization(key);
            return Localization.instance.GetLocalization(context, key);
        }

        private Events.EventEntity entity;
        protected virtual void Awake()
        {
            actors = GetActors();
        }

        protected abstract List<ILocalizationActor> GetActors();

        protected virtual void OnEnable()
        {
            entity = Events.Subscribe(Localization.LocalizationChange, OnLocalizationChange);
            FreshView();
        }
        private List<ILocalizationActor> actors;
        public void AddActor(ILocalizationActor actor)
        {
            if (actors.Contains(actor)) return;
            actors.Add(actor);
            actor.Execute(Localization.instance.GetLocalizationType(),this);
        }
        public void RemoveActor(ILocalizationActor actor)
        {
            actors.Remove(actor);
        }
        private void OnLocalizationChange(IEventArgs args)
        {
            FreshView();
        }
        private void FreshView()
        {
            var _type = Localization.instance.GetLocalizationType();
            for (int i = 0; i < actors.Count; i++)
                actors[i].Execute(_type, this);
        }

        protected virtual void OnDisable()
        {
            entity.Dispose();
            entity = null;
        }
    }
}
