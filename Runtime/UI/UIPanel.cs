/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.UI
{

    public class UIPanel:MonoBehaviour
    {
        private string path;
        public int GetSiblingIndex() => transform.GetSiblingIndex();
        public void SetSiblingIndex(int sbindex) => transform.SetSiblingIndex(sbindex);
        public void SetPath(string path)
        {
            this.path = path;
            string panelName = System.IO.Path.GetFileNameWithoutExtension(path);
            this.name = panelName;
        }
        public string GetPath() => this.path;
        private PanelState _lastState = PanelState.None;
        public PanelState lastState { get { return _lastState; } }
        public void SetState(PanelState type)
        {
            _lastState = type;
        }
        //public UIPanel Clone(Transform parent) => UnityEngine.Object.Instantiate(this, parent);
    }
}
