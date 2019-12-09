/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using UnityEngine;
namespace IFramework_Demo
{
	internal class UIExample: MonoBehaviour
    {
        private void Start()
        {
            UIManager.AddLoader((type, path,pt,name,arg) =>
            {
                GameObject go = Resources.Load<GameObject>(path);
                return go.GetComponent<UIPanel>();
            });
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UIManager.Get(typeof(Panel1), "Canvas", UIPanelLayer.Background, "Panel1", new UIEventArgs(), false);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                UIManager.Get(typeof(Panel2), "Canvas1", UIPanelLayer.Guide, "Panel2", new UIEventArgs(), false);
            }
        }
    }
}
