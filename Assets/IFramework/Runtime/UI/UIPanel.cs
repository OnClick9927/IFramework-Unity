/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace IFramework.UI
{
    //[ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/UIPanel")]

    public class UIPanel : MonoBehaviour, IScriptCreatorContext
    {
        [SerializeField] private List<GameObject> Prefabs = new List<GameObject>();

        [HideInInspector][SerializeField] private List<MarkContext> marks = new List<MarkContext>();

        List<MarkContext> IScriptCreatorContext.GetMarks() => this.marks;

        List<GameObject> IScriptCreatorContext.GetPrefabs() => Prefabs;

        void IScriptCreatorContext.Read(IScriptCreatorContext @base)
        {
            marks = @base.GetMarks();
            Prefabs = @base.GetPrefabs();
        }





        public enum PanelState
        {
            None, OnLoad, OnShow, OnHide, OnClose
        }
        private string path;

        private CanvasGroup _canvasGroup;
        private CanvasGroup canvasGroup
        {
            get
            {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }
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
        internal void SetState(PanelState type) => _lastState = type;

        public bool visible => canvasGroup.blocksRaycasts;
        internal bool SwitchVisible(bool visible)
        {
            //if (visible == this.visible) return false;
            if (canvasGroup.blocksRaycasts == visible) return false;
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = visible ? true : false;
            canvasGroup.interactable = visible ? true : false;
            return true;
        }



        [Flags]
        public enum AdaptType
        {
            Top = 1 << 2,
            Left = 1 << 3,
            Right = 1 << 4,
            Bottom = 1 << 5,
        }
        [SerializeField]
        [Space(20)]
        private AdaptType adaptType = AdaptType.Top | AdaptType.Right | AdaptType.Left | AdaptType.Bottom;
        [SerializeField]
        internal RectTransform adaptRect;
        public static float ScreenWidth { get => _screenWidth; set { _screenWidth = value; } }
        public static float ScreenHeight { get => _screenHeight; set { _screenHeight = value; } }
        public static Rect SafeArea
        {
            get
            {
                if (_safeArea == Rect.zero)
                    _safeArea = Screen.safeArea;
                return _safeArea;
            }
            set { _safeArea = value; }
        }

        private static float _screenWidth = Screen.width;
        private static float _screenHeight = Screen.height;
        private static Rect _safeArea /*= Screen.safeArea*/;


//        private void Awake()
//        {
//#if UNITY_EDITOR
//            if (Application.isPlaying) return;
//            if (adaptRect == null)
//                adaptRect = GetComponent<RectTransform>();
//#endif
//        }
        private void OnEnable()
        {
            if (adaptRect)
                AdaptNotchScreen();
        }
        public void AdaptNotchScreen()
        {
            AdaptNotchScreen(adaptRect, adaptType);

        }



        public static void AdaptNotchScreen(RectTransform transform, AdaptType type)
        {
            if (type.HasFlag(AdaptType.Bottom))
            {
                float offset = SafeArea.yMin;
                float ratio = offset / ScreenHeight;
                offset = transform.rect.height * ratio;
                transform.offsetMin = new Vector2(transform.offsetMin.x, offset);
            }
            if (type.HasFlag(AdaptType.Top))
            {
                float offset = ScreenHeight - SafeArea.yMax;
                float ratio = offset / ScreenHeight;
                offset = transform.rect.height * ratio;
                transform.offsetMax = new Vector2(transform.offsetMax.x, -offset);
            }
            if (type.HasFlag(AdaptType.Left))

            {
                float offset = SafeArea.xMin;
                float ratio = offset / ScreenWidth;
                offset = transform.rect.width * ratio;
                transform.offsetMin = new Vector2(offset, transform.offsetMin.y);
            }
            if (type.HasFlag(AdaptType.Right))
            {
                float offset = ScreenWidth - SafeArea.xMax;
                float ratio = offset / ScreenWidth;
                offset = transform.rect.width * ratio;
                transform.offsetMax = new Vector2(-offset, transform.offsetMax.y);
            }
        }

 
    }
}
