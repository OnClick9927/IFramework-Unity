/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    class GUIElementSelection
    {
        public delegate void OnElementChange(GUIElement element);
        public static event OnElementChange onElementChange;
        private static GUIElement m_element;
        public static GUIElement element
        {
            get { return m_element; }
            set
            {
                m_element = value;

                if (EditorWindow.focusedWindow != null)
                    EditorWindow.focusedWindow.Repaint();

                if (Event.current != null && Event.current.type != EventType.Layout)
                    Event.current.Use();
                if (onElementChange != null)
                    onElementChange(m_element);
            }
        }

        public static GUIElement copyElement { get; set; }
        public static GUIElement dragElement { get; set; }
    }
}
