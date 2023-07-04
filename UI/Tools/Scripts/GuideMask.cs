/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.184
 *UnityVersion:   2019.4.22f1c1
 *Date:           2021-12-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace IFramework.UI
{

    public class GuideMask : Graphic, ICanvasRaycastFilter
    {
        [SerializeField] private Vector2 _center;
        [SerializeField] private Vector2 _size;
        [SerializeField] private Vector2 _margin=Vector2.one*10;

        [SerializeField] private float _radian = 0.2f;
        [SerializeField] private bool _raycastInRect;
        [SerializeField] private bool _raycastPass;

        public Vector2 center
        {
            get { return _center; }
        }
        public Vector2 size
        {
            get { return _size; }
        }
        public Rect rect { get { return new Rect() { size = size, center = center, }; } }
        public bool raycastInRect { get { return _raycastInRect; } set { _raycastInRect = value; } }
        public bool raycastPass { get { return _raycastPass; } set { _raycastPass = value; } }
        public Vector2 margin { get { return _margin; } set { _margin = value; } }

        public float radian { get { return _radian; } }
        public Color background
        {
            get
            {
                if (material == null) return Color.white;
                return material.GetColor("_BackgroundColor");
            }
            set
            {
                if (material == null) return;
                material.SetColor("_BackgroundColor", value);
            }
        }
        public override Color color
        {
            get
            {
                if (material == null) return Color.white;
                return material.GetColor("_FrontColor");
            }
            set
            {
                if (material == null) return;
                material.SetColor("_FrontColor", value);
            }
        }

        public void SetRect(Rect rect)
        {
            this._center = rect.center;
            this._size = rect.size;
            if (material == null) return;
            material.SetVector("_Rect", new Vector4(center.x, center.y, size.x, size.y));
        }
        public void SetRadian(float radian)
        {
            this._radian = radian;
            if (material == null) return;

            material.SetFloat("_Radian", radian);
        }

        public Rect GetFocusRect(RectTransform trans,Vector2 offset)
        {
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(trans);
            Vector2 size = bounds.extents * 2;
            Canvas canvas = trans.GetComponentInParent<Canvas>();
            Vector2 center = Vector2.zero;
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    center = RectTransformUtility.WorldToScreenPoint(null, trans.position);
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    center = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, trans.position);
                    break;
                default:
                    break;
            }
            return new Rect(center - (Vector2)bounds.extents + offset - margin/2, size+ margin);
        }
        public Rect GetFocusRect(Transform trans, Vector2 size, Camera camera)
        {
            if (camera == null)
                camera = Camera.main;
            Vector2 center = camera.WorldToScreenPoint(trans.position);
            return new Rect() { size = size +margin, center = center - margin / 2, };
        }

        public void Focus(RectTransform trans, Vector2 offset)
        {
            SetRect(GetFocusRect(trans,offset));
        }
        public void Focus(Transform trans, Vector2 size, Camera camera)
        {
            SetRect(GetFocusRect(trans, size, camera));
        }
        private bool Contains(Rect rect, Vector2 point)
        {
            return rect.Contains(point);
        }


        protected override void Awake()
        {
            SetRect(this.rect);
            SetRadian(this._radian);
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (!raycastPass) return true;
            if (raycastInRect) return !Contains(rect, sp);
            return false;
        }
    }
}
