/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using IFramework.GUITool;

namespace IFramework.AB
{
    internal class ABBuildListView : IRectGUIDrawer, ILayoutGUIDrawer
    {
        private const string TitleStyle = "IN BigTitle";
        private const string EntryBackodd = "CN EntryBackodd";
        private const string EntryBackEven = "CN EntryBackEven";

        private const string Preview = "Preview";
        private const string AssetName = "AssetName";
        private const string Bundle = "Bundle";
        private const string Size = "Size";
        private const string CrossRef = "CrossRef";
        private const float LineHeight = 20;

        private TableViewCalculator table = new TableViewCalculator();
        private List<ABBuildItem> Info { get { return ABWindow.Instance.Info.TempInfo.Builds; } }
        private List<AssetDependenceInfo> dpInfo { get { return ABWindow.Instance.Info.TempInfo.dps; } }
        private ABBuildWindow BuildWindow { get { return ABWindow.Instance.BuildWindow; } }

        private ListViewCalculator.ColumnSetting[] Setting { get {
                return new ListViewCalculator.ColumnSetting[] {
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=Preview,
                        Width=40
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=AssetName,
                        Width=320
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=Bundle,
                        Width=100
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=Size,
                        Width=100
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=CrossRef,
                        Width=40
                    },

                };
                    
        } }
        private AssetDependenceInfo GetDpByName(string AssetPath)
        {
            for (int i = 0; i < dpInfo.Count; i++)
            {
                if (dpInfo[i].AssetPath == AssetPath) return dpInfo[i];
            }
            return default(AssetDependenceInfo);
        }

        private Vector2 ScrollPos;
        private int lineCount;
        public ABBuildItem ViewInfo { get { return ABWindow.Instance.BuildWindow.ChossedAssetBundle; } }
        //public AssetDependenceInfo ChoosedAsset;
        private Rect position;
        public  void OnGUI( Rect position)
        {
            this.position = position;
            position = position.Zoom(AnchorType.MiddleCenter, -2);
            DrawListView();
            Eve();
        }


        private void DrawListView()
        {
            if (ViewInfo==null) lineCount = 0;
            else lineCount = ViewInfo.assetNames.Count;


            this.Box(position);
            table.Calc(position, new Vector2(position.x, position.y + LineHeight), ScrollPos, LineHeight, lineCount, Setting);

           this. DrawArea(() =>
            {
                EditorGUI.Toggle(table.TitleRow.LocalPostion, true, new GUIStyle(TitleStyle));

                EditorGUI.LabelField(table.TitleRow[AssetName].LocalPostion, AssetName);
                EditorGUI.LabelField(table.TitleRow[Bundle].LocalPostion, Bundle);
                EditorGUI.LabelField(table.TitleRow[Size].LocalPostion, Size);

            }, table.TitleRow.Position);
            this.DrawScrollView(() =>
            {
                for (int i = table.FirstVisibleRow; i < table.LastVisibleRow+1; i++)
                {


                    AssetDependenceInfo asset = GetDpByName(ViewInfo.assetNames[i]);
                    GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                    if (Event.current.type == EventType.Repaint)
                        style.Draw(table.Rows[i].Position, false, false, table.Rows[i].Selected, false);
                    if (Event.current.modifiers == EventModifiers.Control &&
                            Event.current.button == 0 && Event.current.clickCount == 1 &&
                            table.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        table.ControlSelectRow(i);
                        ABWindow.Instance.Repaint();
                    }
                    else if (Event.current.modifiers == EventModifiers.Shift &&
                                    Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    table.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        table.ShiftSelectRow(i);

                        ABWindow.Instance.Repaint();
                    }
                    else if (Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    table.Rows[i].Position.Contains(Event.current.mousePosition))
                    {
                        table.SelectRow(i);
                        ABWindow.Instance.BuildWindow.ChoosedAsset = asset;
                        ABWindow.Instance.Repaint();
                    }



                    this.Label(table.Rows[i][Size].Position, asset.Size);
                    this.Label(table.Rows[i][AssetName].Position, asset.AssetName);
                    this.Label(table.Rows[i][Preview].Position, asset.ThumbNail);
                    this.Label(table.Rows[i][Bundle].Position, asset.BundleName);
                    if (asset.AssetBundles.Count == 1)
                    {
                        this. Label(table.Rows[i][CrossRef].Position, EditorGUIUtility.IconContent("Collab"));
                    }
                    else
                    {
                        this.Label(table.Rows[i][CrossRef].Position, asset.AssetBundles.Count.ToString(), new GUIStyle("CN CountBadge"));

                    }


                }

            }, table.View,
           ref ScrollPos,
            table.Content, false, false
        );

            Handles.color = Color.black;

            for (int i = 0; i < table.TitleRow.Columns.Count; i++)
            {
                var item = table.TitleRow.Columns[i];

                if (i != 0)
                    Handles.DrawAAPolyLine(1, new Vector3(item.Position.x - 2,
                                                            item.Position.y,
                                                            0),
                                              new Vector3(item.Position.x - 2,
                                                            item.Position.yMax - 2,
                                                            0));

            }
            table.Position.DrawOutLine(2, Color.black);
           
            Handles.color = Color.white;
        }


        private void Eve()
        {
            Event eve = Event.current;
            if (eve.button == 0 && eve.clickCount == 1 &&
             
                 (table.View.Contains(eve.mousePosition) &&
                  !table.Content.Contains(eve.mousePosition)))
            {
                table.SelectNone();
                BuildWindow.ChoosedAsset = null; 
                ABWindow.Instance. Repaint();
            }


            if (eve.button == 1 && eve.clickCount == 1 &&
                        table.Content.Contains(eve.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () => {

                    table.SelectedRows.ReverseForEach((row) =>
                    {
                        ABWindow.Instance.RemoveAsset(ViewInfo.assetNames[row.RowID], ViewInfo.assetBundleName);

                    });
                    ScriptableObj.Update(ABWindow.Instance.Info);
                    BuildWindow.ChoosedAsset = null;

                });

                menu.ShowAsContext();
                if (eve.type != EventType.Layout)
                    eve.Use();

            }
        }

       
    }

}
