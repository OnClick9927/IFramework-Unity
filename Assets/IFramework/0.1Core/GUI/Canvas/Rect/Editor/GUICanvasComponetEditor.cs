/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace IFramework.GUITool.RectDesign
{
    [CustomEditor(typeof(GUICanvasComponet))]
	public class GUICanvasComponetEditor:Editor
	{
        private GUICanvasHierarchyTree HierarchyView = new GUICanvasHierarchyTree();
        private GUIElementInspectorView InspectorView = new GUIElementInspectorView();
        private GUICanvasComponet componet { get { return (target as GUICanvasComponet); } }
        private TextAsset textAsset { get { return componet.textAsset; }set { componet.textAsset = value; } }
        private string assetPath
        {
            get
            {
                if (textAsset == null)
                    return string.Empty;
                return AssetDatabase.GetAssetPath(textAsset);
            }
        }
        private GUICanvas canvas { get { return (target as GUICanvasComponet).guiCanvas; } }
        float scrollViewHeight=200;
        private void OnEnable()
        {
            HierarchyView.HandleEve = true;

        }
        private int index = 0;
        public override void OnInspectorGUI()
        {
            scrollViewHeight = EditorGUILayout.FloatField("ScrollViewHeight", scrollViewHeight);
            textAsset = (TextAsset)EditorGUILayout.ObjectField("Text Asset", textAsset, typeof(TextAsset), false);
            if (string.IsNullOrEmpty(assetPath) || !assetPath.EndsWith(".xml")) textAsset = null;
            using (new EditorGUI.DisabledScope(textAsset == null))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Refresh"), GUILayout.Width(25)))
                        HierarchyView.canvas = canvas;
                    if (GUILayout.Button("Load"))
                    {
                        componet.LoadCanvas((target as GUICanvasComponet).textAsset);
                        HierarchyView.canvas = canvas;
                        if (SceneView.lastActiveSceneView!=null)
                            SceneView.lastActiveSceneView.Repaint();

                    }
                    if (GUILayout.Button("Save"))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.AppendChild(canvas.Serialize(doc));
                        doc.Save(assetPath);
                        AssetDatabase.Refresh();
                    }
                }

            }

            index = GUILayout.Toolbar(index, new string[] { "Tree", "Design" });
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(scrollViewHeight));
            GUI.Box(rect, "");

            if (canvas == null) return;
            if (HierarchyView.canvas == null)
                HierarchyView.canvas = canvas;
            if (index == 0)
            {
                HierarchyView.OnCanvasTreeGUI(rect);
            }
            else
            {
                Rect rect2 = rect;
                rect2.width = EditorWindowUtil.Find("InspectorWindow").position.width-20;
                rect2.height = scrollViewHeight;
                GUI.BeginGroup(rect2);
                InspectorView.OnGUI(rect2);
                GUI.EndGroup();
                Repaint();
            }
        }
        private void OnSceneGUI()
        {
            if (canvas!=null)
            {
                canvas.OnGUI();
            }
        }
    }
}
