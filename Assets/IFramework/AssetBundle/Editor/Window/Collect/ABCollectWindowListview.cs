/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IFramework.AB
{
    internal class ABCollectWindowListview  :IRectGUIDrawer, ILayoutGUIDrawer
    {
        private const string CollectType = "CollectType";
        private const string BundleName = "BundleName";
        private const string SearchPath = "SearchPath";
        private const string SelectButton = "Set";


        private const string TitleStyle = "IN BigTitle";
        private const string EntryBackodd = "CN EntryBackodd";
        private const string EntryBackEven = "CN EntryBackEven";

        private ListViewCalculator.ColumnSetting[] Setting { get {

                return new ListViewCalculator.ColumnSetting[]
                    {
                        new ListViewCalculator.ColumnSetting()
                        {
                            Name = BundleName,
                            Width = 100
                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            Name=CollectType,
                            Width=80,
                        },
                        //new ListViewItemSetting()
                        //{
                        //    name=SearchOption,
                        //    width=100
                        //},
                        //new ListViewItemSetting()
                        //{
                        //    name = SearchPattern,
                        //    width=100
                        //},
                        new ListViewCalculator.ColumnSetting()
                        {
                            Name=SelectButton,
                            Width=50,
                            OffSetY=-4,
                            OffsetX=-10
                           
                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            Name=SearchPath,
                            Width=200
                        },
                    };
            } }

        private AssetChooseWindow chosseWindow=new AssetChooseWindow();

        private ABCollectWindow CollectWindow { get { return ABWindow.Instance.CollectWindow; } }

        private ABCollectInfo Info { get { return ABWindow.Instance.Info.CollectInfo; } }
        private Vector2 ScrollPos;

        private TableViewCalculator Table = new TableViewCalculator();
        private const float lineHeight= 20;

        private void ListView()
        {
            Table.Calc(position, new Vector2(position.x, position.y + lineHeight), ScrollPos, lineHeight, Info.Collects.Count, Setting);
            if (Event.current.type == EventType.Repaint)
                new GUIStyle(EntryBackodd).Draw(Table.Position, false, false, false, false);

           this. DrawArea(() => {

               bool tog = true;
                this.Toggle(Table.TitleRow.LocalPostion,ref tog, new GUIStyle(TitleStyle));
                this.LabelField(Table.TitleRow[CollectType].LocalPostion, CollectType);
                this.LabelField(Table.TitleRow[BundleName].LocalPostion, BundleName);
                this.LabelField(Table.TitleRow[SearchPath].LocalPostion, SearchPath);

            }, Table.TitleRow.Position);


            this.DrawScrollView(() =>
            {
                for (int i = Table.FirstVisibleRow; i < Table.LastVisibleRow+1; i++)
                {

                    GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                    if (Event.current.type == EventType.Repaint)
                        style.Draw(Table.Rows[i].Position, false, false, Table.Rows[i].Selected, false);
                    if (Event.current.modifiers == EventModifiers.Control &&
                            Event.current.button == 0 && Event.current.clickCount == 1 &&
                            Table.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        Table.ControlSelectRow(i);
                        ABWindow.Instance.Repaint();
                    }
                    else if (Event.current.modifiers == EventModifiers.Shift &&
                                    Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    Table.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        Table.ShiftSelectRow(i);

                        ABWindow.Instance.Repaint();
                    }
                    else if (Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    Table.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        Table.SelectRow(i);
                        ABWindow.Instance.Repaint();
                    }

                    ABCollectItem item = Info.Collects[i];


                    int index = (int)item.CollectType;
                    this.Popup(Table.Rows[i][CollectType].Position,
                                                              ref index,
                                                                Enum.GetNames(typeof(ABCollectType)));
                    item.CollectType = (ABCollectType)index;
                    this.Button(() =>
                    {
                        chosseWindow.assetinfo = item.subAsset;
                        PopupWindow.Show(Table.Rows[i][SelectButton].Position, chosseWindow);
                    }
                    , Table.Rows[i][SelectButton].Position, SelectButton);

                    this.Label(Table.Rows[i][SearchPath].Position, item.SearchPath);
                    if (item.CollectType == ABCollectType.ABName)
                    {
                        this.TextField(Table.Rows[i][BundleName].Position,ref item.BundleName);
                       
                    }
                }
            }, Table.View,
            ref ScrollPos,
            Table.Content, false, false
            );


            Handles.color = Color.black;
            for (int i = 0; i < Table.TitleRow.Columns.Count; i++)
            {
                var item = Table.TitleRow.Columns[i];

                if (i != 0)
                    Handles.DrawAAPolyLine(1, new Vector3(item.Position.x-2,
                                                            item.Position.y,
                                                            0),
                                              new Vector3(item.Position.x-2,
                                                            item.Position.yMax-2,
                                                            0));

            }
            Table.Position.DrawOutLine(2, Color.black);


            Handles.color = Color.white;


        }
        private Rect position;
        public  void OnGUI(Rect position)
        {
            position = position.Zoom(AnchorType.MiddleCenter, -2);
            this.position = position;
            ListView();
            Eve();
        }
        private void Eve()
        {
            Event eve = Event.current;

            if (eve.button == 0 && eve.clickCount == 1 &&
                    (!Table.View.Contains(eve.mousePosition) ||
                        (Table.View.Contains(eve.mousePosition) &&
                         !Table.Content.Contains(eve.mousePosition))))
            {
                Table.SelectNone();
                ABWindow.Instance. Repaint();
            }
            DragAndDropIInfo info = DragAndDropUtil.Drag(eve, Table.View);
            if (info.EnterArera && info.Finsh)
            {
                for (int i = 0; i < info.paths.Length; i++)
                {
                    SaveInfo(info.paths[i]);
                }
            }
            if (eve.button == 1 && eve.clickCount == 1 &&
                      Table.Content.Contains(eve.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () => {
                    for (int i = Table.Rows.Count - 1; i >= 0; i--)
                    {
                        if (Table.Rows[i].Selected)
                        {
                           Info.Collects .RemoveAt(i);
                        }
                    }
                    ScriptableObj.Update(ABWindow.Instance.Info);
                });

                menu.ShowAsContext();
                if (eve.type != EventType.Layout)
                    eve.Use();

            }
        }

        private void SaveInfo(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.Contains("Assets")) return;
            if (!Directory.Exists(path)) return;
            for (int i = 0; i < Info.Collects.Count; i++)
            {
                if (Info.Collects[i].SearchPath.Contains(path)) return;
            }
            Info.AddCollecter(path);
            ScriptableObj.Update(ABWindow.Instance.Info);
        }



        public class AssetChooseWindow : IFPopupWindow, IRectGUIDrawer, ILayoutGUIDrawer
        {
            public SubAssetInfo assetinfo;
            public AssetChooseWindow() { this.windowSize = new Vector2(200, 300); }
            public override void OnGUI(Rect rect)
            {
                base.OnGUI(rect);
                if (assetinfo == null) return;
                this.DrawScrollView(() => {

                    Draw(assetinfo,0);



                },ref scroll);
            }
            private Vector2 scroll;

            private void Draw(SubAssetInfo assetinfo ,float offset)
            {
               this. DrawHorizontal(() =>
                {
                   this. Space(offset);
                    if (assetinfo.fileType == FileType.InValidFile)
                    {
                        GUI.enabled = false;
                    }
                    bool s = assetinfo.Selected;
                    this. Toggle(ref s, new GUIContent(assetinfo.ThumbNail), GUILayout.Height(16), GUILayout.Width(40));
                    assetinfo.Selected = s;
                    if (assetinfo.Sub.Count > 0)
                    {
                        this. Foldout(ref assetinfo.isOpen, assetinfo.name); 
                    }
                    else
                    {
                        this.Label(assetinfo.name);
                    }
                    GUI.enabled = true;
                   

                });

                if (assetinfo.isOpen)
                {
                    for (int i = 0; i < assetinfo.Sub.Count; i++)
                    {
                        Draw(assetinfo.Sub[i],offset+20);
                    }
                }

            }
        }


       
    }

}
