/*********************************************************************************
 *Author:         A.墨城
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-05-06
*********************************************************************************/
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace IFramework.UI
{

    [CustomEditor(typeof(PolygonRaycastImage))]
    class PolygonRaycastImageEditor : ImageEditor
    {
        [MenuItem("CONTEXT/Image/Replace To PolygonRaycastImage")]
        static void Replace(MenuCommand command)
        {
            var img = command.context as Image;
            var sp = img.sprite;
            var go = img.gameObject;
            Object.DestroyImmediate(img);
            img = go.AddComponent<PolygonRaycastImage>();
            img.sprite = sp;
            EditorUtility.SetDirty(img);
            AssetDatabase.SaveAssetIfDirty(img);
        }
        private PolygonRaycastGraphicDrawTool<PolygonRaycastImage> tool;

        protected override void OnEnable()
        {
            base.OnEnable();
            tool = new PolygonRaycastGraphicDrawTool<PolygonRaycastImage>();
            tool.OnEnable(target);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            tool.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            tool.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("points"));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            GUILayout.Space(20);
            base.OnInspectorGUI();
        }

        public void OnSceneGUI() => tool.OnSceneGUI();


    }




}
