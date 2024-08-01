/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/

using UnityEngine;

namespace IFramework.UI
{
    public abstract class GameObjectView
    {
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;
            InitComponents();
        }

        protected Transform GetTransform(string path)
        {
            if (string.IsNullOrEmpty(path))
                return transform;
            return transform.Find(path);
        }
        protected GameObject GetGameObject(string path)
        {
            return GetTransform(path)?.gameObject;
        }

        protected T GetComponent<T>(string path)
        {
            var trans = GetTransform(path);
            if (trans != null)
                return trans.GetComponent<T>();
            return default;
        }
        protected abstract void InitComponents();
    }
}
