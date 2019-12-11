/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    public static partial class RectTransformExtension
    {
        public static Vector2 GetPosInRootTrans(this RectTransform self, Transform rootTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, self).center;
        }

        public static RectTransform AnchorPosX(this RectTransform self, float anchorPosX)
        {
            var anchorPos = self.anchoredPosition;
            anchorPos.x = anchorPosX;
            self.anchoredPosition = anchorPos;
            return self;
        }

        public static RectTransform SetSizeWidth(this RectTransform self, float sizeWidth)
        {
            var sizeDelta = self.sizeDelta;
            sizeDelta.x = sizeWidth;
            self.sizeDelta = sizeDelta;
            return self;
        }

        public static RectTransform SetSizeHeight(this RectTransform self, float sizeHeight)
        {
            var sizeDelta = self.sizeDelta;
            sizeDelta.y = sizeHeight;
            self.sizeDelta = sizeDelta;
            return self;
        }

        public static Vector2 GetWorldSize(this RectTransform self)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(self).size;
        }
    }
}