/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [CustomGUIElement(typeof(ParentGUIElement))]
    public class ParentGUIElementEditor : GUIElementEditor
    {
        private ParentGUIElement haveChildElement { get { return element as ParentGUIElement; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "Children Elements", null, ContentGUI);
        }
        private void ContentGUI()
        {
            Event e = Event.current;
            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < haveChildElement.Children.Count; i++)
                {
                    EditorGUILayout.TextField(haveChildElement.Children[i].GetType().Name, haveChildElement.Children[i].name, "ObjectField");
                    Rect r = GUILayoutUtility.GetLastRect();
                    if (r.Contains(e.mousePosition) && e.clickCount == 2)
                        GUIElementSelection.element = haveChildElement.Children[i] as GUIElement;
                }
            }
        }

    }
}
