/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace IFramework
{
    public abstract class LocalizationMapActorEditor<T, V> : LocalizationActorEditor<T> where T : class, ILocalizationActor
    {
        protected override void OnGUI(LocalizationBehavior component, T context)
        {
            if (component.context == null) return;
            var keys = component.GetLocalizationTypes();
            var map = GetMap(context);
            for (int i = 0; keys.Count > i; i++)
            {
                var key = keys[i];
                if (!map.ContainsKey(key))
                {
                    map.Add(key, GetDefault());
                    SetDirty(component);
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                var lan = keys[i];
                var src = map[lan];
                var tmp = Draw(lan, src);
                bool change = false;
                if (src == null)
                {
                    if (tmp != null) change = true;
                }
                else
                {
                    if (tmp == null) change = true;
                    else change = !src.Equals(tmp);
                }
                if (change)
                {
                    map[lan] = tmp;
                    SetDirty(component);
                }

            }
        }
        protected abstract SerializableDictionary<string, V> GetMap(T context);
        protected abstract V GetDefault();
        protected abstract V Draw(string lan, V value);
    }
}
