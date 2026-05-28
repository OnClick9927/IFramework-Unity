/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
namespace IFramework
{
    public abstract class GameObjectView
    {
        private IScriptCreatorContext context;
        public GameObject gameObject { get; protected set; }
        public Transform transform { get; private set; }

        public virtual void SetActive(bool active) => gameObject.SetActive(active);
        protected abstract void InitComponents();

        protected virtual void OnSetGameObject(GameObject gameObject)
        {
            ClearPrefabs();
            this.gameObject = gameObject;
            transform = gameObject.transform;
            context = GetComponent<IScriptCreatorContext>(string.Empty);
        }
        public bool SetGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Log.FE($"{GetType()}-->  Can not SetGameObject With Null GameObject");
                return false;
            }
            if (this.gameObject != gameObject)
            {
                OnSetGameObject(gameObject);
                InitComponents();
                return true;
            }
            return false;
        }
        public Transform GetTransform(string path)
        {
            var tans = string.IsNullOrEmpty(path) ? transform : transform.Find(path);
            if (tans == null)
                Log.FE($"{GetType()}-->  GetTransform result is Null! path-->{path}");
            return tans;
        }
        public GameObject GetGameObject(string path) => GetTransform(path)?.gameObject;
        public T GetComponent<T>(string path)
        {
            var trans = GetTransform(path);
            if (trans != null)
                return trans.GetComponent<T>();
            return default;
        }

        Dictionary<string, GameObject> _prefabsName;
        public GameObject FindPrefab(string name)
        {
            if (_prefabsName == null) _prefabsName = new Dictionary<string, GameObject>();
            GameObject prefab = null;
            if (_prefabsName.TryGetValue(name, out prefab))
            {
                return prefab;
            }
            var prefabs = context.GetPrefabs();
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i].name != name) continue;
                prefab = prefabs[i];
                break;
            }
            if (prefab != null)
                _prefabsName[name] = prefab;
            else
                Log.FE($"Not Find Prefab with Name {name} in {context.name}");
            return prefab;

        }
        internal protected void ClearPrefabs()
        {
            _prefabsName?.Clear();
            _prefabsName = null;
        }
    }
}

