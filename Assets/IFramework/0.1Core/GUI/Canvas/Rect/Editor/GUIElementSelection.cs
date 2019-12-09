/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-11-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.GUITool.RectDesign
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

                if (UnityEditor.EditorWindow.focusedWindow != null)
                    UnityEditor.EditorWindow.focusedWindow.Repaint();
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
