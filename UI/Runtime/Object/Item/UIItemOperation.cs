/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.UI
{
    public class UIItemOperation : UIAsyncOperation<GameObject>
    {
        private UIModule.ItemPool pool;
        public GameObject gameObject { get { return this.value; } }
        public UIItemOperation(UIModule.ItemPool pool)
        {
            this.pool = pool;
            Wait(pool);
        }
        private async void Wait(UIModule.ItemPool op)
        {
            await op;
            var _gameObject = UnityEngine.GameObject.Instantiate(pool.prefab);
            base.SetValue(_gameObject);
        }
    }


}
