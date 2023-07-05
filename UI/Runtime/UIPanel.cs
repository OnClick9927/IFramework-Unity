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
    /// <summary>
    /// ui 基类
    /// </summary>
    public class UIPanel : MonoBehaviour {
       [HideInInspector] public string path;
        private PanelState _lastState = PanelState.None;
        public PanelState lastState { get { return _lastState; } }
        public void SetState(PanelState type)
        {
            _lastState = type;
        }
    }
}
