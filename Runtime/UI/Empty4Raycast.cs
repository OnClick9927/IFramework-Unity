/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.198
 *UnityVersion:   2019.4.22f1c1
 *Date:           2021-12-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("IFramework/Empty4Raycast")]
    public class Empty4Raycast : Graphic, ICanvasRaycastFilter
    {
        [SerializeField]
        private List<Vector2> points = new List<Vector2>();

        public List<Vector2> Points
        {
            get => points ??= new List<Vector2>();
            set => points = value;
        }


        bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (points == null || points.Count <= 0)
                return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return PolygonOverlap(points, pos.x, pos.y);
        }



        // 可以将该方法移至你的工具类
        static bool PolygonOverlap(IList<Vector2> points, float targetX, float targetY)
        {
            bool result = false;

            for (int i = 0, j = points.Count - 1; i < points.Count; i++)
            {
                bool betweenY = points[i].y > targetY != points[j].y > targetY;
                if (betweenY)
                {
                    // 检查x是在i、j交点的左边还是右边
                    if (targetX < (points[j].x - points[i].x) / (points[j].y - points[i].y) * (targetY - points[i].y) + points[i].x)
                    {
                        result = !result;
                    }
                }

                j = i;
            }

            return result;
        }
        protected Empty4Raycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }

    }
}
