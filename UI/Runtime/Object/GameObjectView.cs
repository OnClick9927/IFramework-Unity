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
        protected Transform transform { get; private set; }

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
        protected abstract void InitComponents();
    }
}
