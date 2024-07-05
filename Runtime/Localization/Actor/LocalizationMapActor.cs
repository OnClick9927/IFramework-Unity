/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace IFramework.Localization
{
    public abstract class LocalizationMapActor<Behavior, Value> : LocalizationActor<Behavior> where Behavior : LocalizationBehavior
    {
        public SerializableDictionary<string, Value> map = new SerializableDictionary<string, Value>();

        public Value GetValue()
        {
            return GetValue(Localization.GetLocalizationType());
        }
        public Value GetValue(string localizationType)
        {
            Value v;
            map.TryGetValue(localizationType, out v);
            return v;
        }
    }
}
