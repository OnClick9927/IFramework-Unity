/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework.AB
{
    internal class ABBuildWindow 
    {

        private ABBuildListView Listview = new ABBuildListView();

        public ABBuildItem ChossedAssetBundle;
        public AssetDependenceInfo ChoosedAsset;


        
        public ABBuildWindow()
        {
            leftMunu = new ABBuildWindowLeftMenu();
            buttom = new ABBuildWindowBottom();
        }
        private ABBuildWindowLeftMenu leftMunu;
        private ABBuildWindowBottom buttom;

        private Rect position;
        public void OnGUI( Rect position)
        {
            this.position = position;
            position = position.Zoom(AnchorType.MiddleCenter, -10);
            leftMunu.OnGUI(leftRect);
            Listview.OnGUI(listViewRect);
            buttom.OnGUI(RightButtomRect);

            CalcSplit();
        }


        private bool Vdragging;
        private bool Hdragging;

        private float LeftWidth = 300;
        private float listViewHeight = 300;

        Rect leftRect;
        Rect midVerticalRect;
        Rect midHorizontalRect;

        Rect RightRect;
        Rect listViewRect;
        Rect RightButtomRect;


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
                        LeftWidth = Mathf.Clamp(LeftWidth, 150, position.width - 150);
                      ABWindow.Instance.  Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (Vdragging)
                    {
                        Vdragging = false;
                    }
                    break;
            }

            var rs=  position.VerticalSplit(LeftWidth, 2, true);
            midVerticalRect = position.VerticalSplitRect(LeftWidth, 4);



            leftRect = rs[0];
            RightRect = rs[1];


            switch (Event.current.rawType)
            {
                case EventType.MouseDown:
                    if (midHorizontalRect.Contains(Event.current.mousePosition))
                    {
                        Hdragging = true;
                    }
                    break;
                case EventType.MouseDrag:
                    if (Hdragging)
                    {
                        listViewHeight += Event.current.delta.y;
                        listViewHeight = Mathf.Clamp(listViewHeight, 150, position.height - 150);
                        ABWindow.Instance.Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (Hdragging)
                    {
                        Hdragging = false;
                    }
                    break;
            }

            rs = RightRect.HorizontalSplit(listViewHeight, 2, true);
            midHorizontalRect= RightRect.HorizontalSplitRect(listViewHeight, 4);
            listViewRect = rs[0];
            RightButtomRect = rs[1];
            

            EditorGUIUtility.AddCursorRect(midVerticalRect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(midHorizontalRect, MouseCursor.ResizeVertical);

        }

       
    }

}
