/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    public class MeshUtil
	{
        public void Combine(MeshFilter[] Filters, Renderer[] rs,ref GameObject go)
        {
            Material[] ms = new Material[rs.Length];
            CombineInstance[] CoI = new CombineInstance[Filters.Length];
            for (int i = 0; i < Filters.Length; i++)
            {
                ms[i] = rs[i].sharedMaterial;
                CoI[i].mesh = Filters[i].sharedMesh;
                CoI[i].transform = go.transform.worldToLocalMatrix * Filters[i].transform.localToWorldMatrix;
            }
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(CoI, false);
            if (go.GetComponent<MeshFilter>()==null)
            {
                go.AddComponent<MeshFilter>();
            }
            if (go.GetComponent<Renderer>() == null)
            {
                go.AddComponent<Renderer>();
            }
            go.GetComponent<MeshFilter>().sharedMesh = mesh;
            go.GetComponent<Renderer>().sharedMaterials = ms;
        }
	}
}
