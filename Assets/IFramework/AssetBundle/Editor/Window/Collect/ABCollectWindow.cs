/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using UnityEditor;

namespace IFramework.AB
{
    internal class ABCollectWindow 
    {
        private ABCollectWindowLeftMenu LeftMenu=new ABCollectWindowLeftMenu();
        private ABCollectWindowListview ListView=new ABCollectWindowListview();


        private Rect position;
        public  void OnGUI( Rect position)
        {
            position = position.Zoom(AnchorType.MiddleCenter, -10);
            this.position = position;
            LeftMenu.OnGUI(leftRect);
            ListView.OnGUI(RightRect);
            CalcSplit();
        }

        private Rect midVerticalRect;
        private Rect leftRect;

        public Rect RightRect;

        private bool Vdragging;
        private float LeftWidth = 200;
        private void CalcSplit()
        {
            switch (Event.current.rawType)
            {
                case EventType.MouseDown:
                    if (midVerticalRect.Contains(Event.current.mousePosition))
                    {
                        Vdragging = true;
                    }
                    break;
                case EventType.MouseDrag:
                    if (Vdragging)
                    {
                        LeftWidth += Event.current.delta.x;
                        LeftWidth = Mathf.Clamp(LeftWidth, 150, 300);
                        ABWindow.Instance.Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (Vdragging)
                    {
                        Vdragging = false;
                    }
                    break;
            }

            var rs = position.VerticalSplit(LeftWidth, 2, true);
            midVerticalRect = position.VerticalSplitRect(LeftWidth, 4);



            leftRect = rs[0];
            RightRect = rs[1];


         

            EditorGUIUtility.AddCursorRect(midVerticalRect, UnityEditor.MouseCursor.ResizeHorizontal);

        }

       
    }

}
