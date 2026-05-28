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
    [AddComponentMenu("UI/Effects/ImagePolygonMeshEffect")]
    [RequireComponent(typeof(Image))]
    public class ImagePolygonMeshEffect : BaseMeshEffect
    {
        protected ImagePolygonMeshEffect()
        { }

        // GC Friendly
        private static Vector3[] fourCorners = new Vector3[4];
        private static UIVertex vertice = new UIVertex();
        private RectTransform rectTransform = null;
        private Image image = null;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled) return;

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if (image == null)
            {
                image = GetComponent<Image>();
            }
            if (image.type != Image.Type.Simple)
            {
                return;
            }
       
            Sprite sprite = image.overrideSprite;
            if (sprite == null || sprite.triangles.Length == 6)
            {
                // only 2 triangles
                return;
            }

            // Kanglai: at first I copy codes from Image.GetDrawingDimensions
            // to calculate Image's dimensions. But now for easy to read, I just take usage of corners.
            if (vh.currentVertCount != 4)
            {
                return;
            }

            rectTransform.GetLocalCorners(fourCorners);

            // Kanglai: recalculate vertices from Sprite!
            int len = sprite.vertices.Length;
            var vertices = new List<UIVertex>(len);
            Vector2 Center = sprite.bounds.center;
            Vector2 invExtend = new Vector2(1 / sprite.bounds.size.x, 1 / sprite.bounds.size.y);
            for (int i = 0; i < len; i++)
            {
                // normalize
                float x = (sprite.vertices[i].x - Center.x) * invExtend.x + 0.5f;
                float y = (sprite.vertices[i].y - Center.y) * invExtend.y + 0.5f;
                // lerp to position
                vertice.position = new Vector2(Mathf.Lerp(fourCorners[0].x, fourCorners[2].x, x), Mathf.Lerp(fourCorners[0].y, fourCorners[2].y, y));
                vertice.color = image.color;
                vertice.uv0 = sprite.uv[i];
                vertices.Add(vertice);
            }

            len = sprite.triangles.Length;
            var triangles = new List<int>(len);
            for (int i = 0; i < len; i++)
            {
                triangles.Add(sprite.triangles[i]);
            }

            vh.Clear();
            vh.AddUIVertexStream(vertices, triangles);
        }
    }
}
