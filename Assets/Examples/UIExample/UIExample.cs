/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework
{
	internal class UIExample:UnityEngine.MonoBehaviour
	{
        private void Start()
        {
            UIManager.AddLoader((type, path,pt,name,arg) =>
            {
                GameObject go = Resources.Load<GameObject>(path);
                return go.GetComponent<UIPanel>();
            });
             //p = UIManager.LoadUIPanel(typeof(Panel1), "Canvas", UIPanelLayer.Guide, "p1", new UIEventArgs());

        }
        UIPanel p;
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                 //UIManager.Push(p, new UIEventArgs());
                UIManager.Get(typeof(Panel1), "Canvas", UIPanelLayer.Background, "p1", new UIEventArgs(), false);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                UIManager.Get(typeof(Panel2), "Canvas1", UIPanelLayer.Guide, "2",new UIEventArgs(), false);
            }
        }
    }
}
