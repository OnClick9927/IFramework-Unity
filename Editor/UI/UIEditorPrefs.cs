/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.UI
{
    public class UIEditorPrefs
    {
        [SerializeField] public int plan_index = 0;

        [SerializeField] public List<EditorPanelCollectionPlan> _plans = new();

        [System.Serializable]
        public class ScrpitSeg
        {
            public string prefab;
            public string ScriptPath;
            public List<string> Paths;
        }

        public List<ScrpitSeg> segs = new List<ScrpitSeg>();

        [System.Serializable]
        public class UiLayerEdit
        {
            public int mode;
            public string layerObjectPath;

        }
        public UiLayerEdit layerEdit;
        [System.Serializable]
        public class UIGenCodeCS_PUB
        {
            public string NameSpace;
            public int viewBaseIndex;
            public int widgetBaseIndex;
        }
        public UIGenCodeCS_PUB pubsave = new UIGenCodeCS_PUB();






        private static UIEditorPrefs _context;
        public static void Save() => EditorTools.SaveToPrefs(_context, nameof(UIEditorPrefs), false);

        public static UIEditorPrefs context
        {
            get
            {

                if (_context == null)
                {
                    _context = EditorTools.GetFromPrefs<UIEditorPrefs>(nameof(UIEditorPrefs), false);
                    if (_context == null)
                        _context = new UIEditorPrefs();
                    OnLoad(_context);
                    Save();
                }
                return _context;
            }
        }

        private static void OnLoad(UIEditorPrefs context)
        {
            EditorPanelCollectionPlans.OnLoad(context);
        }
    }
}
