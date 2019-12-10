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
using System.Collections.Generic;
using IFramework.GUITool;

namespace IFramework.AB
{
    internal class ABBuildWindowLeftMenu : IRectGUIDrawer,ILayoutGUIDrawer
    {
        private const string ABName = "ABName";
        private const string RefCount = "RefCount";
        private const string TitleStyle = "IN BigTitle";
        private const string EntryBackodd = "CN EntryBackodd";
        private const string EntryBackEven = "CN EntryBackEven";

        private List< ABBuildItem> Builds { get { return ABWindow.Instance.Info.TempInfo.Builds; } }
        private ABBuildWindow BuildWindow { get { return ABWindow.Instance.BuildWindow; } }
        private ListViewCalculator ListView = new ListViewCalculator();
        private Vector2 ScrollPos;
        private ListViewCalculator.ColumnSetting[] Setting { get {
                return new ListViewCalculator.ColumnSetting[]
                {
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=ABName,
                        Width=300
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=RefCount,
                        Width=40
                    }
                };
            } }
        private float LineHeight = 20;


        private void DrawListView(Rect rect)
        {
            ListView.Calc(rect, rect.position,ScrollPos, LineHeight, Builds.Count, Setting);
             this.DrawScrollView(() =>
            {
                for (int i = ListView.FirstVisibleRow; i < ListView.LastVisibleRow+1; i++)
                {

                    GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                    if (Event.current.type == EventType.Repaint)
                        style.Draw(ListView.Rows[i].Position, false, false, ListView.Rows[i].Selected, false);
                    if (Event.current.modifiers == EventModifiers.Control &&
                            Event.current.button == 0 && Event.current.clickCount == 1 &&
                            ListView.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        ListView.ControlSelectRow(i);
                        
                        ABWindow.Instance.Repaint();
                    }
                    else if (Event.current.modifiers == EventModifiers.Shift &&
                                    Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    ListView.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        ListView.ShiftSelectRow(i);
                        ABWindow.Instance.Repaint();

                    }
                    else if (Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    ListView.Rows[i].Position.Contains(Event.current.mousePosition) 
                                  /*  && ListView.viewPosition.Contains(Event.current.mousePosition) */)
                    {
                        ListView.SelectRow(i);
                        BuildWindow.ChossedAssetBundle = Builds[i];
                        ABWindow.Instance.Repaint();
                    }

                    this.Label(ListView.Rows[i][ABName].Position, Builds[i].assetBundleName);
                    if (Builds[i].CrossRef)
                        this.Label(ListView.Rows[i][RefCount].Position, EditorGUIUtility.IconContent("console.warnicon.sml"));
                    else
                        this.Label(ListView.Rows[i][RefCount].Position, EditorGUIUtility.IconContent("Collab"));

                }
            }, ListView.View,
            ref ScrollPos,
             ListView.Content, false, false);
        }

        private Rect position;
        public void OnGUI( Rect position)
        {
            position = position.Zoom(AnchorType.MiddleCenter, -2);
            this.position = position;

            this.Box(position);
            position.DrawOutLine(2, Color.black);

            DrawListView(listViewrect.Zoom( AnchorType.MiddleCenter,-10));
            DrawPreview(Buttom);
            Eve();
            CalcSplit();
        }

        private void DrawPreview(Rect rect)
        {
            rect.DrawOutLine(1,Color.black);
            if (ABWindow.Instance.BuildWindow.ChossedAssetBundle == null) return;
            if (Event.current.type==  EventType.Repaint)
                new GUIStyle(TitleStyle).Draw(rect, false, false, false, false);

            this.Box(rect);
            rect.DrawOutLine(2, Color.black);


          
            this.DrawArea(() => {
                this.Label(ABWindow.Instance.BuildWindow.ChossedAssetBundle.assetBundleName);
                this.Label(ABWindow.Instance.BuildWindow.ChossedAssetBundle.Size);
                if (ABWindow.Instance.BuildWindow.ChossedAssetBundle.CrossRef)
                    this. Label(EditorGUIUtility.IconContent("console.warnicon.sml"));
                else
                    this.Label( EditorGUIUtility.IconContent("Collab"));
            }, rect.Zoom( AnchorType.MiddleCenter,-10));
        }


        private void Eve()
        {
            Event eve = Event.current;

            if (eve.button == 0 && eve.clickCount == 1 &&
                        (ListView.View.Contains(eve.mousePosition) &&
                         !ListView.Content.Contains(eve.mousePosition)))
            {
                ListView.SelectNone();
                ABWindow.Instance.Repaint();
                BuildWindow.ChoosedAsset = null;
                BuildWindow.ChossedAssetBundle = null;
            }

            if (eve.button == 1 && eve.clickCount == 1 &&
              ListView.Content.Contains(eve.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () => {

                    BuildWindow.ChoosedAsset = null;
                    BuildWindow.ChossedAssetBundle = null;
                    ListView.SelectedRows.ReverseForEach((row) => {

                        int index = row.RowID;
                        ABWindow.Instance.DeleteBundle(Builds[index].assetBundleName);
                    });
                 

                    ScriptableObj.Update(ABWindow.Instance.Info);

                });

                menu.ShowAsContext();
                if (eve.type != EventType.Layout)
                    eve.Use();

            }
        }
        private bool dragging;
        private float ButtumHeight = 100;
        Rect midRect;
        Rect listViewrect;
        Rect Buttom;

        private void CalcSplit()
        {
            switch (Event.current.rawType)
            {
                case EventType.MouseDown:
                    if (midRect.Contains(Event.current.mousePosition))
                    {
                        dragging = true;
                    }
                    break;
                case EventType.MouseDrag:
                    if (dragging)
                    {
                        ButtumHeight -= Event.current.delta.y;
                        ButtumHeight = Mathf.Clamp(ButtumHeight, 110, position.height - 150);
                        ABWindow.Instance.Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (dragging)
                    {
                        dragging = false;
                    }
                    break;
            }
            listViewrect.Set(position.x, position.y, position.width, position.height - ButtumHeight);
            Buttom.Set(position.x, position.yMax - ButtumHeight, position.width  , ButtumHeight);
            midRect.Set(position.x, position.yMax - ButtumHeight, position.width, 5);
            EditorGUIUtility.AddCursorRect(midRect, UnityEditor.MouseCursor.ResizeVertical);

        }

       
    }
   
}
