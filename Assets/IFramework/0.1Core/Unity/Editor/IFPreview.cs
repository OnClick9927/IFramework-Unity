/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IFramework
{
    public class IFPreview
    {
        public PreviewRenderUtility Preview { get { return preview; } }
        private PreviewRenderUtility preview;
        private MeshFilter[] filters;
        private Renderer[] renderers;
        private GameObject go;
        private Dictionary<Renderer, Material[]> dic = new Dictionary<Renderer, Material[]>();
        public Color SkyboxColor = Color.white;
        public bool UseLight;
        public Rect rect;
        public GUIStyle PreviewStyle = GUIStyle.none;
        public IFPreview(GameObject go, MeshFilter[] filters, Renderer[] renderers)
        {
            this.go = GameObject.Instantiate(go);
            this.filters = filters;
            this.renderers = renderers;
            for (int i = 0; i < renderers.Length; i++)
            {
                dic.Add(renderers[i], renderers[i].sharedMaterials);
            }
            preview = new PreviewRenderUtility(false);
            preview.AddSingleGO(this.go);
            GameObject.DestroyImmediate(this.go);
        }



        public Texture Render(Rect rect, Matrix4x4 matrix)
        {
            this.rect = rect;
            preview.BeginPreview(rect, PreviewStyle);
            for (int i = 0; i < filters.Length; i++)
            {
                for (int j = 0; j < dic[renderers[i]].Length; j++)
                {
                    preview.DrawMesh(filters[i].sharedMesh, matrix, dic[renderers[i]][j], j);

                }
            }
            if (UseLight)
            {
                InternalEditorUtility.SetCustomLighting(preview.lights, SkyboxColor);
                preview.camera.Render();
                InternalEditorUtility.RemoveCustomLighting();
            }
            else
            {
                preview.camera.Render();
            }
            return preview.EndPreview();
        }

        public void Cleanup()
        {
            if (preview != null)
            {
                preview.Cleanup();
                //UnityEngine.Object.DestroyImmediate(this.mPreviewMaterial.shader, false);
                // UnityEngine.Object.DestroyImmediate(this.mPreviewMaterial, false);
                dic.Clear();
            }
            preview = null;
        }


    }
}
