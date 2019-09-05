/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-11-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.GUIDesign.RectDesign
{
    public class ElementSelection
    {
        public delegate void OnElementChange(Element element);
        public static event OnElementChange onElementChange;
        private static Element m_element;
        public static Element element
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

        public static Element copyElement { get; set; }
        public static Element dragElement { get; set; }
    }
}
