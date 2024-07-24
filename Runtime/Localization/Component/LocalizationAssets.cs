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
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/LocalizationAssets")]
    public class LocalizationAssets : LocalizationBehavior
    {

        [System.Serializable]
        public class ObjectActor<T> : LocalizationMapActor<LocalizationBehavior, T> where T : UnityEngine.Object
        {
            public SerializableDictionary<string, T> objects = new SerializableDictionary<string, T>();

            public ObjectActor(bool enable) : base(enable)
            {
            }

            public override T GetDefault()
            {
                return null;
            }

            protected override void Execute(string localizationType, LocalizationBehavior component)
            {

            }
        }
        public List<ObjectActor<Object>> objects = new List<ObjectActor<Object>>();
        private Dictionary<string, ObjectActor<Object>> objectMap = new Dictionary<string, ObjectActor<Object>>();
     
        
        public Object GetObject(string key)
        {
            if (!objectMap.ContainsKey(key))return null;
            var actor = objectMap[key];
            return actor.GetValue();
        }
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                objectMap.Add((obj as ILocalizationActor).name, obj);
            }
        }
        protected override List<ILocalizationActor> GetActors()
        {
            var list = new List<ILocalizationActor>();
            list.AddRange(objects);

            return list;
        }
    }
}
