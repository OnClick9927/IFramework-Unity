/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace IFramework
{
    public abstract class LocalizationActor<T> : ILocalizationActor where T : LocalizationComponent
    {
        public bool enable { get => _enable; set => _enable = value; }
        [UnityEngine.SerializeField] private bool _enable = true;
        void ILocalizationActor.Execute(string localizationType, LocalizationComponent component)
        {
            if (!enable) return;
            Execute(localizationType, component as T);
        }
        protected abstract void Execute(string localizationType, T component);

    }
}
