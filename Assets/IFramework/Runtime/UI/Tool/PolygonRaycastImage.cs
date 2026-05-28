/*********************************************************************************
 *Author:         A.墨城
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-05-06
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace IFramework.UI
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("IFramework/PolygonRaycastImage")]
    public class PolygonRaycastImage : Image, IPolygonRaycastGraphic
    {
        [SerializeField]
        private List<Vector2> points = new List<Vector2>();
        public List<Vector2> Points
        {
            get => points ??= new List<Vector2>();
            set => points = value;
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (points == null || points.Count <= 0)
                return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return Empty4Raycast.PolygonOverlap(points, pos.x, pos.y);

        }
    }
}
