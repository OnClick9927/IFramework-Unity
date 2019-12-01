/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using UnityEditor;
namespace IFramework
{
    public class DragAndDropIInfo
    {
        public bool Dragging;
        public bool EnterArera;
        public bool Finsh;
        public Object[] objectReferences { get { return DragAndDrop.objectReferences; } }
        public string[] paths { get { return DragAndDrop.paths; } }
        public DragAndDropVisualMode visualMode { get { return DragAndDrop.visualMode; } }
        public int activeControlID { get { return DragAndDrop.activeControlID; } }
    }

    public class DragAndDropUtil
	{
        private static bool Dragging;
        private static bool EnterArera;
        private static bool Finsh;
        private static DragAndDropIInfo info=new DragAndDropIInfo();
        public static DragAndDropIInfo Drag(Event eve ,Rect Content, DragAndDropVisualMode mode = DragAndDropVisualMode.Generic)
        {
            switch (eve.type)
            {
                case EventType.DragUpdated:
                    Dragging = true; Finsh = false;
                    EnterArera = Content.Contains(eve.mousePosition);
                    if (EnterArera)
                    {
                        DragAndDrop.visualMode = mode;
                        Event.current.Use();
                    }
                    break;
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    EnterArera = Content.Contains(eve.mousePosition);
                    Finsh = true;Dragging = false;
                    Event.current.Use();

                    break;
                case EventType.DragExited:
                    Dragging = false; Finsh = true;
                    EnterArera = Content.Contains(eve.mousePosition);
                    break;
                default:
                    Dragging = false; Finsh = false;
                    EnterArera = Content.Contains(eve.mousePosition);
                    break;
            }
            info.Finsh = Finsh;
            info.EnterArera = EnterArera;
            info.Dragging = Dragging;
            return info;
        }
	}
}
