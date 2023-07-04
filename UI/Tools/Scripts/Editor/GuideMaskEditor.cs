/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.155
 *UnityVersion:   2019.4.16f1
 *Date:           2021-07-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
namespace IFramework.UI
{
    [CustomEditor(typeof(GuideMask))]
    class GuideMaskEditor : Editor
    {
        GuideMask gm { get { return target as GuideMask; } }
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            gm.material= EditorGUILayout.ObjectField("Material", gm.material, typeof(Material), false) as Material;
            gm.color = EditorGUILayout.ColorField("Color", gm.color);
            gm.background = EditorGUILayout.ColorField("Background Color", gm.background);
            gm.raycastTarget = EditorGUILayout.Toggle("Raycast Target", gm.raycastTarget);
            gm.raycastInRect = EditorGUILayout.Toggle("Raycast In Rect", gm.raycastInRect);
            gm.raycastPass = EditorGUILayout.Toggle("Raycast Pass", gm.raycastPass);

            var center = EditorGUILayout.Vector2Field("Center", gm.center);
            var size = EditorGUILayout.Vector2Field("Size", gm.size);
            gm.margin = EditorGUILayout.Vector2Field("Margin", gm.margin);
            var radian= EditorGUILayout.Slider("Radian", gm.radian, 0, 1);


            if (EditorGUI.EndChangeCheck())
            {
                gm.SetRadian(radian);
                gm.SetRect(new Rect() { size = size, center = center, });
               
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
