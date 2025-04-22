/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEditor.UI;

namespace IFramework.UI
{
    [CustomEditor(typeof(Empty4Raycast))]
    class Empty4RaycastEditor : GraphicEditor
    {

        private PolygonRaycastGraphicDrawTool<Empty4Raycast> tool;

        protected override void OnEnable()
        {
            base.OnEnable();
            tool = new PolygonRaycastGraphicDrawTool<Empty4Raycast>();
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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RaycastTarget"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("points"));

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            tool.OnSceneGUI();
        }

    }

}
