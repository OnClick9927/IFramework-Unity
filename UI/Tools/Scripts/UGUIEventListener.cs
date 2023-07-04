/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.198
 *UnityVersion:   2019.4.22f1c1
 *Date:           2021-12-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

namespace IFramework.UI
{
    public class UGUIEventListener : MonoBehaviour,
IPointerClickHandler,
IPointerDownHandler,
IPointerEnterHandler,
IPointerExitHandler,
IPointerUpHandler,
ISelectHandler,
IUpdateSelectedHandler,
IDeselectHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler,
IDropHandler,
IScrollHandler,
IMoveHandler
    {
        public class UIEvent<T> where T : BaseEventData
        {
            public UIEvent() { }
            public void AddListener(UIEventHandle<T> handle)
            {
                m_UIEventHandle += handle;
            }

            public void RemoveListener(UIEventHandle<T> handle)
            {
                m_UIEventHandle -= handle;
            }

            public void RemoveAllListeners()
            {
                m_UIEventHandle -= m_UIEventHandle;
                m_UIEventHandle = null;
            }

            public void Invoke(GameObject go, T eventData)
            {
                m_UIEventHandle?.Invoke(go, eventData);
            }

            private event UIEventHandle<T> m_UIEventHandle = null;
        }
        public delegate void UIEventHandle<T>(GameObject go, T eventData) where T : BaseEventData;
        public static UGUIEventListener Get(GameObject go)
        {
            UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
            if (listener == null)
            {
                listener = go.AddComponent<UGUIEventListener>();
            }
            return listener;
        }
        public void RemoveAllListeners()
        {
            onClick.RemoveAllListeners();
            onDoubleClick.RemoveAllListeners();
            onPointDown.RemoveAllListeners();
            onPointup.RemoveAllListeners();
            onEnter.RemoveAllListeners();
            onExit.RemoveAllListeners();
            onSelect.RemoveAllListeners();
            onUpdateSelect.RemoveAllListeners();
            onDeselect.RemoveAllListeners();
            onDrag.RemoveAllListeners();
            onEndDrag.RemoveAllListeners();
            onDrop.RemoveAllListeners();
            onScroll.RemoveAllListeners();
            onMove.RemoveAllListeners();
        }
        public UIEvent<PointerEventData> onClick = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onPointDown = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onEnter = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onExit = new UIEvent<PointerEventData>();
        public UIEvent<BaseEventData> onSelect = new UIEvent<BaseEventData>();
        public UIEvent<BaseEventData> onUpdateSelect = new UIEvent<BaseEventData>();
        public UIEvent<BaseEventData> onDeselect = new UIEvent<BaseEventData>();
        public UIEvent<PointerEventData> onBeginDrag = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDrag = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onEndDrag = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDrop = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onScroll = new UIEvent<PointerEventData>();
        public UIEvent<AxisEventData> onMove = new UIEvent<AxisEventData>();
        public UIEvent<PointerEventData> onDoubleClick = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onPress = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onPointup = new UIEvent<PointerEventData>();

        public void OnPointerEnter(PointerEventData eventData) { onEnter.Invoke(gameObject, eventData); }
        public void OnPointerExit(PointerEventData eventData) { onExit.Invoke(gameObject, eventData); }
        public void OnSelect(BaseEventData eventData) { onSelect.Invoke(gameObject, eventData); }
        public void OnUpdateSelected(BaseEventData eventData) { onUpdateSelect.Invoke(gameObject, eventData); }
        public void OnDeselect(BaseEventData eventData) { onDeselect.Invoke(gameObject, eventData); }
        public void OnBeginDrag(PointerEventData eventData) { onBeginDrag.Invoke(gameObject, eventData); }
        public void OnDrag(PointerEventData eventData) { onDrag.Invoke(gameObject, eventData); }
        public void OnEndDrag(PointerEventData eventData) { onEndDrag.Invoke(gameObject, eventData); }
        public void OnDrop(PointerEventData eventData) { onDrop.Invoke(gameObject, eventData); }
        public void OnScroll(PointerEventData eventData) { onScroll.Invoke(gameObject, eventData); }
        public void OnMove(AxisEventData eventData) { onMove.Invoke(gameObject, eventData); }

        public float doubleClickGap = 0.2f;
        public float pressGap = 0.5f;

        private float lastPointdownTime = 0f;
        private bool isPointDown = false;
        public bool isPress { get { return _isPress; } }
        private int clickCount = 0;
        private PointerEventData m_OnUpEventData = null;
        private bool _isPress;

        private void Update()
        {
            if (isPointDown)
            {
                if (Time.unscaledTime - lastPointdownTime >= pressGap)
                {
                    _isPress = true;
                    lastPointdownTime = Time.unscaledTime;
                    onPress.Invoke(gameObject, null);
                }
            }

            if (clickCount > 0)
            {
                if (Time.unscaledTime - lastPointdownTime >= doubleClickGap)
                {
                    if (clickCount < 2)
                    {
                        onClick.Invoke(gameObject, m_OnUpEventData);
                        m_OnUpEventData = null;
                    }
                    clickCount = 0;
                }

                if (clickCount >= 2)
                {
                    onDoubleClick.Invoke(gameObject, m_OnUpEventData);
                    m_OnUpEventData = null;
                    clickCount = 0;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointDown = true;
            _isPress = false;
            lastPointdownTime = Time.unscaledTime;
            onPointDown?.Invoke(gameObject, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointup?.Invoke(gameObject, eventData);
            isPointDown = false;
            m_OnUpEventData = eventData;
            if (!_isPress)
            {
                clickCount++;
            }
        }
    }
}
