/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/

namespace IFramework.Localization
{
    public abstract class LocalizationMapActorEditor<Actor, Value, Behavior> : LocalizationActorEditor<Actor>
        where Actor : LocalizationMapActor<Behavior, Value>
        where Behavior : LocalizationBehavior
    {
        protected override void OnGUI(LocalizationBehavior component, Actor context)
        {
            if (component.context == null) return;
            var keys = component.GetLocalizationTypes();
            var map = context.map;
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
        protected abstract Value GetDefault();
        protected abstract Value Draw(string lan, Value value);
    }
}
