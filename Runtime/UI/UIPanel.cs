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

    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/UIPanel")]

    public class UIPanel : MonoBehaviour
    {
        public enum PanelState
        {
            None, OnLoad, OnShow, OnHide, OnClose
        }
        private string path;

        private CanvasGroup _canvasGroup;
        private PanelState _lastState = PanelState.None;
        public PanelState lastState => _lastState;
        internal bool show => lastState == PanelState.OnShow;

        internal int GetSiblingIndex() => transform.GetSiblingIndex();
        internal void SetSiblingIndex(int index) => transform.SetSiblingIndex(index);
        internal void SetPath(string path)
        {
            this.path = path;
            string panelName = System.IO.Path.GetFileNameWithoutExtension(path);
            this.name = panelName;
        }
        public string GetPath() => this.path;
        internal void SetState(PanelState type)
        {
            _lastState = type;
            if (type == PanelState.OnLoad)
            {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public bool visible => _canvasGroup.blocksRaycasts;
        internal bool SwitchVisible(bool visible)
        {
            //if (visible == this.visible) return false;
            if (_canvasGroup.blocksRaycasts == visible) return false;
            _canvasGroup.alpha = visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = visible ? true : false;
            _canvasGroup.interactable = visible ? true : false;
            return true;
        }
        //public UIPanel Clone(Transform parent) => UnityEngine.Object.Instantiate(this, parent);
    }
}
