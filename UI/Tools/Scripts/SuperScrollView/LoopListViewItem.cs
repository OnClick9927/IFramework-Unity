using UnityEngine;
using static IFramework.UI.SuperScrollView.LoopListView;

namespace IFramework.UI.SuperScrollView
{
    public class LoopListViewItem : MonoBehaviour
    {
        RectTransform _rect;

        public float distanceWithViewPortSnapCenter { get; set; } = 0;

        public float offset { get; set; } = 0;

        public int check { get; set; } = 0;

        public float padding { get; set; }

        public RectTransform rect
        {
            get
            {
                if (_rect == null)
                {
                    _rect = gameObject.GetComponent<RectTransform>();
                }
                return _rect;
            }
        }

        public string path { get; set; }

        public int index { get; set; } = -1;

        public bool initlized { get; set; } = false;

        public LoopListView list { get; set; } = null;

        public float top
        {
            get
            {
                ArrangeType arrageType = list.arrangeType;
                if (arrageType == ArrangeType.TopToBottom)
                {
                    return rect.localPosition.y;
                }
                else if (arrageType == ArrangeType.BottomToTop)
                {
                    return rect.localPosition.y + rect.rect.height;
                }
                return 0;
            }
        }

        public float bottom
        {
            get
            {
                ArrangeType arrageType = list.arrangeType;
                if (arrageType == ArrangeType.TopToBottom)
                {
                    return rect.localPosition.y - rect.rect.height;
                }
                else if (arrageType == ArrangeType.BottomToTop)
                {
                    return rect.localPosition.y;
                }
                return 0;
            }
        }


        public float left
        {
            get
            {
                ArrangeType arrageType = list.arrangeType;
                if (arrageType == ArrangeType.LeftToRight)
                {
                    return rect.localPosition.x;
                }
                else if (arrageType == ArrangeType.RightToLeft)
                {
                    return rect.localPosition.x - rect.rect.width;
                }
                return 0;
            }
        }

        public float right
        {
            get
            {
                ArrangeType arrageType = list.arrangeType;
                if (arrageType == ArrangeType.LeftToRight)
                {
                    return rect.localPosition.x + rect.rect.width;
                }
                else if (arrageType == ArrangeType.RightToLeft)
                {
                    return rect.localPosition.x;
                }
                return 0;
            }
        }

        public float size => list.vertical ? rect.rect.height : rect.rect.width;

        public float sizeByPadding => size + padding;

    }
}
