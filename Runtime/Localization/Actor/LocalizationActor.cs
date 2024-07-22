/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

using UnityEngine;

namespace IFramework.Localization
{
    public abstract class LocalizationActor<T> : ILocalizationActor where T : LocalizationBehavior
    {
        [UnityEngine.SerializeField] private bool _enable = true;
        [UnityEngine.SerializeField] private string _name = string.Empty;
        [UnityEngine.SerializeField] private bool _canRemove = false;

        [System.NonSerialized] private string _localizationType;
        bool ILocalizationActor.enable
        {
            get => _enable; set
            {
                if (value != _enable)
                {
                    _enable = value;
                    if (value)
                    {
                        ((ILocalizationActor)this).Execute();
                    }
                }

            }
        }
        protected LocalizationActor(bool enable)
        {
            this._enable = enable;
        }
        protected T behavior { get; private set; }

        string ILocalizationActor.name => _name;

        public bool canRemove => _canRemove;

        public LocalizationActor<T> SetCanRemove(bool canRemove)
        {
            _canRemove = canRemove;
            return this;
        }
        public LocalizationActor<T> SetName(string name)
        {
            _name = name;
            return this;
        }
        void ILocalizationActor.Execute()
        {
            (this as ILocalizationActor).Execute(this.behavior.GetLocalizationType(), this.behavior);
        }
        void ILocalizationActor.SetBehavior(LocalizationBehavior behavior)
        {
            this.behavior = behavior as T;
        }
        protected virtual bool NeedExecute(string localizationType)
        {
            if (!((ILocalizationActor)this).enable) return false;
#if UNITY_EDITOR
            if (Application.isPlaying)
                if (_localizationType == localizationType)
                    return false;

#else
            if (_localizationType == localizationType) return false;
#endif
            return true;
        }
        void ILocalizationActor.Execute(string localizationType, LocalizationBehavior component)
        {
            if (!NeedExecute(localizationType)) return;
            _localizationType = localizationType;
            T behavior = component as T;
            if (behavior == null) return;
            Execute(localizationType, behavior);
        }
        protected abstract void Execute(string localizationType, T component);


    }
}
