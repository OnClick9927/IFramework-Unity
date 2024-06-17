/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.198
 *UnityVersion:   2019.4.22f1c1
 *Date:           2021-12-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class Empty4Raycast : Image
    {
        private PolygonCollider2D areaPolygon;

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (areaPolygon == null)
                return true;

            if (eventCamera != null)
                return areaPolygon.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
            return areaPolygon.OverlapPoint(screenPoint);

        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            areaPolygon = GetComponent<PolygonCollider2D>();
            if (areaPolygon == null)
                return;
            transform.localPosition = Vector3.zero;
            var w = rectTransform.sizeDelta.x * 0.5f + 0.1f;
            var h = rectTransform.sizeDelta.y * 0.5f + 0.1f;
            areaPolygon.points = new[]
                {
            new Vector2(-w, -h),
            new Vector2(w, -h),
            new Vector2(w, h),
            new Vector2(-w, h)
        };
        }
#endif
        protected Empty4Raycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
        protected override void Awake()
        {
            base.Awake();
            areaPolygon = GetComponent<PolygonCollider2D>();

        }
    }
}
