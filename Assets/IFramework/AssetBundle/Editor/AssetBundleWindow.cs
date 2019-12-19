/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using IFramework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using IFramework.Serialization;

namespace IFramework.AB
{
    [EditorWindowCache("IFramework.AssetBundle")]
    partial class AssetBundleWindow : EditorWindow,ILayoutGUIDrawer
    {
        private abstract class AssetBundleWindowBase
        {
            protected  AssetBundleWindow window { get; private set; }
            protected Rect position { get; private set; }
            protected AssetBundleWindowBase(AssetBundleWindow window)
            {
                this.window = window;
            }
            public virtual void OnGUI(Rect position)
            {
                this.position = position;
            }
            public abstract void OnDisable();
        }
        private class ToolWindow : AssetBundleWindowBase,ILayoutGUIDrawer
        {
            public ToolWindow(AssetBundleWindow window) : base(window) { }
            private ABBuiidInfo ABBuiidInfo { get { return window.Info.ABBuiidInfo; } }

            public override void OnDisable()
            {
                
            }

            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                this.BeginArea(position)
                        .Label("BuildSetting")
                        .Label("", new GUIStyle("IN Title"), GUILayout.Height(5))
                        .Label("Build  Target:")
                        .Label(EditorUserBuildSettings.activeBuildTarget.ToString())
                        .Label("", new GUIStyle("IN Title"), GUILayout.Height(5))
                        .Label("AssetBundle OutPath:")
                        .Label("Assets/../AssetBundles")
                        .Label("", new GUIStyle("IN Title"), GUILayout.Height(5))
                        .Label("Manifest FilePath:")
                        .Label(ABTool.ManifestPath)
                        .Label("", new GUIStyle("IN Title"), GUILayout.Height(5))
                        .Space(10)
                        .Label("LoadSetting In Editor")
                        .Pan(()=> {
                            ABTool.ActiveBundleMode = !EditorGUILayout.Toggle(new GUIContent("AssetDataBase Load"), !ABTool.ActiveBundleMode);
                        })
                        .Space(10)
                        .Button(() => {
                            ABBuild.DeleteBundleFile();
                        }, "Clear Bundle Files")
                        .Button(() => {
                            ABBuild.BuildManifest(ABTool.ManifestPath, ABBuiidInfo.GetAssetBundleBuilds());
                        }, "Build Manifest")
                        .Button(() => {
                            ABBuild.BuildManifest(ABTool.ManifestPath, ABBuiidInfo.GetAssetBundleBuilds());
                            ABBuild.BuildAssetBundles(ABBuiidInfo.GetAssetBundleBuilds(), EditorUserBuildSettings.activeBuildTarget);
                            ProcessUtil.OpenFloder(ABTool.AssetBundlesOutputDirName);
                        }, "Build AssetBundle")
                        .Button(() => {
                            ABBuild.CopyAssetBundlesTo(Application.streamingAssetsPath);
                        }, "copy to Stream")
                    .EndArea();
            }
        }
        private class DirCollectWindow : AssetBundleWindowBase, ILayoutGUIDrawer, IRectGUIDrawer
        {
            public class AssetChooseWindow : IFPopupWindow, IRectGUIDrawer, ILayoutGUIDrawer
            {
                public ABDirCollectItem.ABSubFile assetinfo;
                public AssetChooseWindow() { this.windowSize = new Vector2(200, 300); }
                public override void OnGUI(Rect rect)
                {
                    base.OnGUI(rect);
                    if (assetinfo == null) return;
                    this.DrawScrollView(() => {
                        Draw(assetinfo, 0);
                    }, ref scroll);
                }
                private Vector2 scroll;

                private void Draw(ABDirCollectItem.ABSubFile assetinfo, float offset)
                {
                    this.DrawHorizontal(() =>
                    {
                        this.Space(offset);
                        if (assetinfo.fileType == ABDirCollectItem.ABSubFile.FileType.InValidFile)
                            GUI.enabled = false;
                        bool s = assetinfo.Selected;
                        this.Toggle(ref s, new GUIContent(assetinfo.ThumbNail), GUILayout.Height(16), GUILayout.Width(40));
                        assetinfo.Selected = s;
                        if (assetinfo.SubFiles.Count > 0)
                            this.Foldout(ref assetinfo.isOpen, assetinfo.name);
                        else
                            this.Label(assetinfo.name);
                        GUI.enabled = true;
                    });

                    if (assetinfo.isOpen)
                        for (int i = 0; i < assetinfo.SubFiles.Count; i++)
                            Draw(assetinfo.SubFiles[i], offset + 20);
                }
            }

            private const string CollectType = "CollectType";
            private const string BundleName = "BundleName";
            private const string SearchPath = "SearchPath";
            private const string SelectButton = "Set";
            private const string TitleStyle = "IN BigTitle";
            private const string EntryBackodd = "CN EntryBackodd";
            private const string EntryBackEven = "CN EntryBackEven";
            private const float lineHeight = 20;

            private Vector2 ScrollPos;
            private AssetChooseWindow chosseWindow;
            private TableViewCalculator tableViewCalc;
            private ABDirCollect DirCollect { get { return window.Info.DirCollect; } }
            private ListViewCalculator.ColumnSetting[] Setting
            {
                get
                {

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
                }
            }


            public DirCollectWindow(AssetBundleWindow window) : base(window) {
                chosseWindow = new AssetChooseWindow();
                tableViewCalc = new TableViewCalculator();
            }

            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                Event e = Event.current;
                ListView(e);
                Eve(e);
            }
            private void ListView(Event e)
            {
                tableViewCalc.Calc(position, new Vector2(position.x, position.y + lineHeight), ScrollPos, lineHeight, DirCollect.DirCollectItems.Count, Setting);
                if (Event.current.type == EventType.Repaint)
                    new GUIStyle(EntryBackodd).Draw(tableViewCalc.Position, false, false, false, false);

                bool tog = true;
                this.Toggle(tableViewCalc.TitleRow.Position, ref tog, new GUIStyle(TitleStyle))
                    .LabelField(tableViewCalc.TitleRow[CollectType].Position, CollectType)
                    .LabelField(tableViewCalc.TitleRow[BundleName].Position, BundleName)
                    .LabelField(tableViewCalc.TitleRow[SearchPath].Position, SearchPath);
                this.DrawScrollView(() =>
                {
                    for (int i = tableViewCalc.FirstVisibleRow; i < tableViewCalc.LastVisibleRow + 1; i++)
                    {
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (e.type == EventType.Repaint)
                            style.Draw(tableViewCalc.Rows[i].Position, false, false, tableViewCalc.Rows[i].Selected, false);
                        if (e.modifiers == EventModifiers.Control &&
                                e.button == 0 && e.clickCount == 1 &&
                                tableViewCalc.Rows[i].Position.Contains(e.mousePosition))
                        {
                            tableViewCalc.ControlSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.modifiers == EventModifiers.Shift &&
                                        e.button == 0 && e.clickCount == 1 &&
                                        tableViewCalc.Rows[i].Position.Contains(e.mousePosition))
                        {
                            tableViewCalc.ShiftSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.button == 0 && e.clickCount == 1 &&
                                        tableViewCalc.Rows[i].Position.Contains(e.mousePosition))
                        {
                            tableViewCalc.SelectRow(i);
                            window.Repaint();
                        }

                        ABDirCollectItem item = DirCollect.DirCollectItems[i];

                        int index = (int)item.CollectType;
                        this.Popup(tableViewCalc.Rows[i][CollectType].Position,
                                    ref index,
                                    Enum.GetNames(typeof(ABDirCollectItem.ABDirCollectType)));
                        item.CollectType = (ABDirCollectItem.ABDirCollectType)index;
                        this.Button(() =>
                        {
                            chosseWindow.assetinfo = item.subAsset;
                            PopupWindow.Show(tableViewCalc.Rows[i][SelectButton].Position, chosseWindow);
                        }
                        , tableViewCalc.Rows[i][SelectButton].Position, SelectButton);

                        this.Label(tableViewCalc.Rows[i][SearchPath].Position, item.SearchPath);
                        if (item.CollectType == ABDirCollectItem.ABDirCollectType.ABName)
                            this.TextField(tableViewCalc.Rows[i][BundleName].Position, ref item.BundleName);
                    }
                }, tableViewCalc.View,ref ScrollPos,tableViewCalc.Content, false, false);

                Handles.color = Color.black;
                for (int i = 0; i < tableViewCalc.TitleRow.Columns.Count; i++)
                {
                    var item = tableViewCalc.TitleRow.Columns[i];

                    if (i != 0)
                        Handles.DrawAAPolyLine(1, new Vector3(item.Position.x - 2,
                                                                item.Position.y,
                                                                0),
                                                  new Vector3(item.Position.x - 2,
                                                                item.Position.yMax - 2,
                                                                0));

                }
                tableViewCalc.Position.DrawOutLine(2, Color.black);
                Handles.color = Color.white;
            }
            private void Eve(Event e)
            {
                if (e.button == 0 && e.clickCount == 1 &&
                        (!tableViewCalc.View.Contains(e.mousePosition) ||
                            (tableViewCalc.View.Contains(e.mousePosition) &&
                             !tableViewCalc.Content.Contains(e.mousePosition))))
                {
                    tableViewCalc.SelectNone();
                    window.Repaint();
                }
                DragAndDropIInfo info = DragAndDropUtil.Drag(e, tableViewCalc.View);
                if (info.EnterArera && info.Finsh)
                {
                    for (int i = 0; i < info.paths.Length; i++)
                    {
                        AddCollectItem(info.paths[i]);
                    }
                }
                if (e.button == 1 && e.clickCount == 1 &&
                          tableViewCalc.Content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        for (int i = tableViewCalc.Rows.Count - 1; i >= 0; i--)
                        {
                            if (tableViewCalc.Rows[i].Selected)
                                DirCollect.RemoveCollectItem(DirCollect.DirCollectItems[i]);
                        }
                        window.UpdateInfo();
                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();

                }
            }

            private void AddCollectItem(string path)
            {
                if (string.IsNullOrEmpty(path) || !path.Contains("Assets")) return;
                if (!Directory.Exists(path)) return;
                DirCollect.AddCollectItem(path);
                window.UpdateInfo();
            }

            public override void OnDisable()
            {
                
            }
        }
        private class AssetBundleBulidWindow : AssetBundleWindowBase, IRectGUIDrawer, ILayoutGUIDrawer
        {
            private SubWinTree abBuildInfoTree;
            private const string ABBWin = "AssetBundleBuild";
            private const string ABBItemWin = "ABBItemWin";
            private const string ABBContentWin = "ABBContent";
            private const string ABBContentItemWin = "ABBContentItem";
            private const float LineHeight = 20;

            public AssetBundleBuild_Class ChossedABB;
            public ABDeprndence ChoosedAsset;
            private List<AssetBundleBuild_Class> AssetbundleBuilds { get { return window.Info.ABBuiidInfo.AssetbundleBuilds; } }

            private const string ABName = "ABName";
            private const string RefCount = "RefCount";
            private const string TitleStyle = "IN BigTitle";
            private const string EntryBackodd = "CN EntryBackodd";
            private const string EntryBackEven = "CN EntryBackEven";
            private ListViewCalculator ABBListViewCalc;
            private Vector2 ABBScrollPos;
            private ListViewCalculator.ColumnSetting[] ABBSetting
            {
                get
                {
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
                }
            }

            private Vector2 ABBContentScrollPos;
            private TableViewCalculator ABBContentTable = new TableViewCalculator();
            private List<ABDeprndence> dpInfo { get { return window.Info.ABBuiidInfo.Dependences; } }

            private const string Preview = "Preview";
            private const string AssetName = "AssetName";
            private const string Bundle = "Bundle";
            private const string Size = "Size";
            private const string CrossRef = "CrossRef";
            private ListViewCalculator.ColumnSetting[] ABBContentSetting
            {
                get
                {
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

                }
            }


            private ListViewCalculator.ColumnSetting[] ABBContentItemSetting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[] {
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=Preview,
                        Width=40
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=AssetName,
                        Width=400
                    },
                };
                }
            }
            private Vector2 ABBContentItemScrollPos;
            private TableViewCalculator ABBContentItemTableCalc = new TableViewCalculator();


            public AssetBundleBulidWindow(AssetBundleWindow window,string layout) : base(window)
            {
                ABBListViewCalc = new ListViewCalculator();
                abBuildInfoTree = new SubWinTree();
                abBuildInfoTree.repaintEve += window.Repaint;
                if (!string.IsNullOrEmpty(layout))
                    abBuildInfoTree.DeSerialize(layout);
                else
                {
                    for (int i =0; i <= 3; i++)
                    {
                        string userdata = i == 1 ? ABBContentItemWin : i == 2 ? ABBItemWin : i == 3 ? ABBContentWin : ABBWin;
                        SubWinTree.TreeLeaf L = abBuildInfoTree.CreateLeaf(new GUIContent(userdata));
                        L.userData = userdata;
                        abBuildInfoTree.DockLeaf(L,(SubWinTree.DockType)i);
                    }
                }
                abBuildInfoTree.isShowTitle = false;
                abBuildInfoTree.drawCursorEve += (rect, sp) =>
                {
                    if (sp == SplitType.Vertical)
                        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                    else
                        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
                };

                abBuildInfoTree[ABBWin].titleContent = new GUIContent(ABBWin);
                abBuildInfoTree[ABBWin].minSize = new Vector2(200, 300);
                abBuildInfoTree[ABBWin].paintDelegate += ABBWinGUI;

                abBuildInfoTree[ABBItemWin].titleContent = new GUIContent(ABBItemWin);
                abBuildInfoTree[ABBItemWin].minSize = new Vector2(200, 150);
                abBuildInfoTree[ABBItemWin].paintDelegate += ABBItemWinGUI;

                abBuildInfoTree[ABBContentWin].titleContent = new GUIContent(ABBContentWin);
                abBuildInfoTree[ABBContentWin].minSize = new Vector2(400, 300);
                abBuildInfoTree[ABBContentWin].paintDelegate += ABBContentWinGUI;

                abBuildInfoTree[ABBContentItemWin].titleContent = new GUIContent(ABBContentItemWin);
                abBuildInfoTree[ABBContentItemWin].minSize = new Vector2(400, 150);
                abBuildInfoTree[ABBContentItemWin].paintDelegate += ABBContentItemWinGUI;

            }

            private void ABBWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                Event e = Event.current;
                ABBListViewCalc.Calc(rect, rect.position, ABBScrollPos, LineHeight, AssetbundleBuilds.Count, ABBSetting);
                this.DrawScrollView(() =>
                {
                    for (int i = ABBListViewCalc.FirstVisibleRow; i < ABBListViewCalc.LastVisibleRow + 1; i++)
                    {
                        if (e.modifiers == EventModifiers.Control &&
                                e.button == 0 && e.clickCount == 1 &&
                                ABBListViewCalc.Rows[i].Position.Contains(e.mousePosition))
                        {
                            ABBListViewCalc.ControlSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.modifiers == EventModifiers.Shift &&
                                        e.button == 0 && e.clickCount == 1 &&
                                        ABBListViewCalc.Rows[i].Position.Contains(e.mousePosition))
                        {
                            ABBListViewCalc.ShiftSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.button == 0 && e.clickCount == 1 &&
                                        ABBListViewCalc.Rows[i].Position.Contains(e.mousePosition)
                                      /*  && ListView.viewPosition.Contains(Event.current.mousePosition) */)
                        {
                            ABBListViewCalc.SelectRow(i);
                            ChossedABB = AssetbundleBuilds[i];
                            window.Repaint();
                        }

                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (e.type == EventType.Repaint)
                            style.Draw(ABBListViewCalc.Rows[i].Position, false, false, ABBListViewCalc.Rows[i].Selected, false);
                        this.Label(ABBListViewCalc.Rows[i][ABName].Position, AssetbundleBuilds[i].assetBundleName);
                        if (AssetbundleBuilds[i].CrossRefence)
                            this.Label(ABBListViewCalc.Rows[i][RefCount].Position, EditorGUIUtility.IconContent("console.warnicon.sml"));
                        else
                            this.Label(ABBListViewCalc.Rows[i][RefCount].Position, EditorGUIUtility.IconContent("Collab"));

                    }
                }, ABBListViewCalc.View,
                ref ABBScrollPos,
                ABBListViewCalc.Content, false, false);

                if (e.button == 0 && e.clickCount == 1 &&
                            (ABBListViewCalc.View.Contains(e.mousePosition) &&
                             !ABBListViewCalc.Content.Contains(e.mousePosition)))
                {
                    ABBListViewCalc.SelectNone();
                    window.Repaint();
                    ChoosedAsset = null;
                    ChossedABB = null;
                }

                if (e.button == 1 && e.clickCount == 1 &&
                  ABBListViewCalc.Content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {

                        ChoosedAsset = null;
                        ChossedABB = null;
                        ABBListViewCalc.SelectedRows.ReverseForEach((row) => {

                            int index = row.RowID;
                            window.DeleteBundle(AssetbundleBuilds[index].assetBundleName);
                        });


                        window.UpdateInfo();

                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();

                }
            }
            private void ABBItemWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                if (ChossedABB == null) return;
                this.BeginArea(rect)
                        .Label(ChossedABB.assetBundleName)
                        .Label(ChossedABB.Size)
                        .Pan(() => {
                            if (ChossedABB.CrossRefence)
                                this.Label(EditorGUIUtility.IconContent("console.warnicon.sml"));
                            else
                                this.Label(EditorGUIUtility.IconContent("Collab"));
                        })
                    .EndArea();
            }



            private ABDeprndence GetDpByName(string AssetPath)
            {
                for (int i = 0; i < dpInfo.Count; i++)
                {
                    if (dpInfo[i].AssetPath == AssetPath) return dpInfo[i];
                }
                return default(ABDeprndence);
            }
            private void ABBContentWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                int lineCount = ChossedABB == null ? 0 : ChossedABB.assetNames.Count;
                ABBContentTable.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ABBContentScrollPos, LineHeight, lineCount, ABBContentSetting);
                    this.Label(ABBContentTable.TitleRow.Position, "", TitleStyle)
                        .Label(ABBContentTable.TitleRow[AssetName].Position, AssetName)
                        .Label(ABBContentTable.TitleRow[Bundle].Position, Bundle)
                        .Label(ABBContentTable.TitleRow[Size].Position, Size);
                Event e = Event.current;
                this.DrawScrollView(() =>
                {
                    for (int i = ABBContentTable.FirstVisibleRow; i < ABBContentTable.LastVisibleRow + 1; i++)
                    {
                        ABDeprndence asset = GetDpByName(ChossedABB.assetNames[i]);
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                        if (e.type == EventType.Repaint)
                            style.Draw(ABBContentTable.Rows[i].Position, false, false, ABBContentTable.Rows[i].Selected, false);

                        this.Label(ABBContentTable.Rows[i][Size].Position, asset.Size)
                            .Label(ABBContentTable.Rows[i][AssetName].Position, asset.AssetName)
                            .Label(ABBContentTable.Rows[i][Preview].Position, asset.ThumbNail)
                            .Label(ABBContentTable.Rows[i][Bundle].Position, asset.BundleName)
                            .Pan(() => {
                                if (asset.AssetBundles.Count == 1)
                                    this.Label(ABBContentTable.Rows[i][CrossRef].Position, EditorGUIUtility.IconContent("Collab"));
                                else
                                    this.Label(ABBContentTable.Rows[i][CrossRef].Position, asset.AssetBundles.Count.ToString(), new GUIStyle("CN CountBadge"));
                            });

                        if (e.modifiers == EventModifiers.Control &&
                                e.button == 0 && e.clickCount == 1 &&
                                ABBContentTable.Rows[i].Position.Contains(Event.current.mousePosition))
                        {
                            ABBContentTable.ControlSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.modifiers == EventModifiers.Shift &&
                                        e.button == 0 &&e.clickCount == 1 &&
                                        ABBContentTable.Rows[i].Position.Contains(e.mousePosition))
                        {
                            ABBContentTable.ShiftSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.button == 0 && e.clickCount == 1 &&
                                        ABBContentTable.Rows[i].Position.Contains(e.mousePosition))
                        {
                            ABBContentTable.SelectRow(i);
                            ChoosedAsset = asset;
                            window.Repaint();
                        }
                    }

                }, ABBContentTable.View,ref ABBContentScrollPos,ABBContentTable.Content, false, false);

                Handles.color = Color.black;

                for (int i = 0; i < ABBContentTable.TitleRow.Columns.Count; i++)
                {
                    var item = ABBContentTable.TitleRow.Columns[i];

                    if (i != 0)
                        Handles.DrawAAPolyLine(1, new Vector3(item.Position.x - 2,
                                                                item.Position.y,
                                                                0),
                                                  new Vector3(item.Position.x - 2,
                                                                item.Position.yMax - 2,
                                                                0));

                }
                ABBContentTable.Position.DrawOutLine(2, Color.black);

                Handles.color = Color.white;

                if (e.button == 0 && e.clickCount == 1 &&
                     (ABBContentTable.View.Contains(e.mousePosition) &&
                      !ABBContentTable.Content.Contains(e.mousePosition)))
                {
                    ABBContentTable.SelectNone();
                    ChoosedAsset = null;
                    window.Repaint();
                }
                if (e.button == 1 && e.clickCount == 1 && ABBContentTable.Content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        ABBContentTable.SelectedRows.ReverseForEach((row) =>
                        {
                            window.RemoveAsset(ChossedABB.assetNames[row.RowID], ChossedABB.assetBundleName);
                        });
                       window.UpdateInfo();
                        ChoosedAsset = null;
                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();
                }
            }
            private void ABBContentItemWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                if (ChoosedAsset == null) return;
                ABBContentItemTableCalc.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ABBContentItemScrollPos, LineHeight, ChoosedAsset.AssetBundles.Count, ABBContentItemSetting);
                this.Label(ABBContentItemTableCalc.TitleRow[AssetName].Position, ChoosedAsset.AssetPath)
                    .Label(ABBContentItemTableCalc.TitleRow[Preview].Position, ChoosedAsset.ThumbNail);

                this.DrawScrollView(() =>
                {
                    for (int i = 0; i < ABBContentItemTableCalc.Rows.Count; i++)
                    {
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (Event.current.type == EventType.Repaint)
                            style.Draw(ABBContentItemTableCalc.Rows[i].Position, false, false, ABBContentItemTableCalc.Rows[i].Selected, false);
                        this.Label(ABBContentItemTableCalc.Rows[i][AssetName].Position, ChoosedAsset.AssetBundles[i]);
                        //this.Label(table.Rows[i][Preview].Position, choo.ThumbNail);
                    }
                }, ABBContentItemTableCalc.View,ref ABBContentItemScrollPos,ABBContentItemTableCalc.Content, false, false);
            }


            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                abBuildInfoTree.OnGUI(position);
               window. WinTree[ABBuildInfoWin].minSize = abBuildInfoTree.minSize;

            }
            public override void OnDisable()
            {
                window.tmpLayout_ABBuildInfo = abBuildInfoTree.Serialize();
            }
        }

        private const string ToolWin = "Tool";
        private const string DirCollectWin = "DirCollectWin";
        private const string ABBuildInfoWin = "AssetBundleBuild";

        [SerializeField]
        private string tmpLayout;
        private string tmpLayoutPath;
        private string tmpLayout_ABBuildInfo;
        private string tmpLayout_ABBuildInfoPath;
        private const float ToolBarHeight = 17;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private SubWinTree WinTree;
        private ToolBarTree ToolBarTree;
        private void SubWinInit()
        {
            tmpLayoutPath= FrameworkConfig.FrameworkPath.CombinePath("AssetBundle/Editor/Layout.xml");
            if (File.Exists(tmpLayoutPath))
                tmpLayout = File.ReadAllText(tmpLayoutPath);

            WinTree = new SubWinTree();
            WinTree.repaintEve += Repaint;
            WinTree.drawCursorEve += (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };
            if (!string.IsNullOrEmpty(tmpLayout))
                WinTree.DeSerialize(tmpLayout);
            else
                for (int i = 1; i <= 3; i++)
                {
                    string userdata = i == 1 ? ToolWin : i == 2 ? DirCollectWin : ABBuildInfoWin;
                    SubWinTree.TreeLeaf L = WinTree.CreateLeaf(new GUIContent(userdata));
                    L.userData = userdata;
                    WinTree.DockLeaf(L, SubWinTree.DockType.Left);
                }


            WinTree[ToolWin].titleContent = new GUIContent(ToolWin);
            WinTree[DirCollectWin].titleContent = new GUIContent(DirCollectWin);
            WinTree[ABBuildInfoWin].titleContent = new GUIContent(ABBuildInfoWin);
            WinTree[ToolWin].minSize = new Vector2(200, 250);
            WinTree[DirCollectWin].minSize = new Vector2(350, 250);
            //WinTree[ABBuildInfoWin].minSize = new Vector2(600, 500);
            WinTree[ToolWin].paintDelegate += toolWindow.OnGUI;
            WinTree[DirCollectWin].paintDelegate += dirCollectWindow.OnGUI;
            WinTree[ABBuildInfoWin].paintDelegate += abBulidWindow.OnGUI;
            ToolBarTree = new ToolBarTree();
            ToolBarTree.DropDownButton(new GUIContent("Views"), (rect) =>
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < WinTree.allLeafCount; i++)
                {
                    SubWinTree.TreeLeaf leaf = WinTree.allLeafs[i];
                    menu.AddItem(leaf.titleContent, !WinTree.closedLeafs.Contains(leaf), () =>
                    {
                        if (WinTree.closedLeafs.Contains(leaf))
                            WinTree.DockLeaf(leaf, SubWinTree.DockType.Left);
                        else
                            WinTree.CloseLeaf(leaf);
                    });
                }
                menu.DropDown(rect);
                Event.current.Use();
            }, 60)
                            .Toggle(new GUIContent("Title"), (bo) => { WinTree.isShowTitle = bo; }, WinTree.isShowTitle, 60)
                            .Toggle(new GUIContent("Lock"), (bo) => { WinTree.isLocked = bo; }, WinTree.isLocked, 60)
                            .Button(new GUIContent("Fresh"), (rect) =>
                            {
                                ReFreashValue();
                            })
                            //.Button(new GUIContent(),(rect)=> {
                            //    File.WriteAllText(tmpLayout_ABBuildInfoPath, abBulidWindow.abBuildInfoTree.Serialize());
                            //})
                            ;

        }


        public ABEditorInfo Info;
        private string EditorInfoPath;
        private AssetBundleBulidWindow abBulidWindow;
        private DirCollectWindow dirCollectWindow;
        private ToolWindow toolWindow;
        private void ABInit()
        {
            tmpLayout_ABBuildInfoPath = FrameworkConfig.FrameworkPath.CombinePath("AssetBundle/Editor/ABBLayout.xml");
            LoadCollectInfo();
            if (string.IsNullOrEmpty(tmpLayout_ABBuildInfo))
                if (File.Exists(tmpLayout_ABBuildInfoPath))
                    tmpLayout_ABBuildInfo = File.ReadAllText(tmpLayout_ABBuildInfoPath);
            if (toolWindow == null)
                toolWindow = new ToolWindow(this);
            if (dirCollectWindow == null)
                dirCollectWindow = new DirCollectWindow(this);
            if (abBulidWindow == null)
                abBulidWindow = new AssetBundleBulidWindow(this,tmpLayout_ABBuildInfo);
        }
        private void LoadCollectInfo()
        {
            EditorInfoPath = FrameworkConfig.FrameworkPath.CombinePath("AssetBundle/Editor/ABEditorInfo.xml");
            if (!File.Exists(EditorInfoPath))
            {
                string xm= Xml.ToXmlString(new ABEditorInfo());
                File.WriteAllText(EditorInfoPath, xm);
                AssetDatabase.Refresh();
            }
            Info = Xml.ToObject<ABEditorInfo>(File.ReadAllText(EditorInfoPath));
        }


        public void ReFreashValue()
        {
            abBulidWindow.ChoosedAsset = null;
            abBulidWindow.ChossedABB = null;
            Info.ABBuiidInfo.ReadAssetbundleBuild(ABBuildCollect.Collect(ABTool.ManifestPath));
            UpdateInfo();
        }
        private void DeleteBundle(string bundleName)
        {
            Info.ABBuiidInfo.RemoveBundle(bundleName);
        }
        private void RemoveAsset(string assetPath, string bundleName)
        {
            Info.ABBuiidInfo.RemoveAsset(bundleName, assetPath);
        }
        private void UpdateInfo()
        {
            string xm = Xml.ToXmlString(Info);
            File.WriteAllText(EditorInfoPath, xm);
            AssetDatabase.Refresh();
            Info = Xml.ToObject<ABEditorInfo>(File.ReadAllText(EditorInfoPath));
        }
    }
    partial class AssetBundleWindow
    {
        private List<ABBuildCollecter> LoadCollecter()
        {
            var collecters = Info.DirCollect.DirCollectItems.ConvertAll<ABBuildCollecter>((item) =>
            {
                if (string.IsNullOrEmpty(item.SearchPath)) return null;
                switch (item.CollectType)
                {
                    case ABDirCollectItem.ABDirCollectType.ABName:
                        if (!string.IsNullOrEmpty(item.BundleName))
                        {
                            return new CollectByABName()
                            {
                                bundleName = item.BundleName,
                                searchPath = item.SearchPath,
                                MeetFiles = item.GetSubAssetPaths()
                            };
                        }
                        return null;
                    case ABDirCollectItem.ABDirCollectType.DirName:
                        return new CollectByDirName()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    case ABDirCollectItem.ABDirCollectType.FileName:
                        return new CollectByFileName()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    case ABDirCollectItem.ABDirCollectType.Scene:
                        return new CollectByScenes()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    default:
                        return null;
                }

            });
            collecters = collecters.ReverseForEach((col) =>
            {
                if (col == null)
                    collecters.Remove(col);
            });
            return collecters;
        }

        private void OnEnable()
        {
            ABBuildCollect.onLoadBuilders -= LoadCollecter;
            ABBuildCollect.onLoadBuilders += LoadCollecter;
            ABInit();
            SubWinInit();
        }
        private void OnDisable()
        {
            AssetBundleWindowBaseOndisable();
            tmpLayout = WinTree.Serialize();
            File.WriteAllText(tmpLayoutPath, tmpLayout);
            UpdateInfo();
        }
        private void OnDestroy()
        {
            File.WriteAllText(tmpLayout_ABBuildInfoPath, tmpLayout_ABBuildInfo);
            AssetDatabase.Refresh();
        }
        private void AssetBundleWindowBaseOndisable()
        {
            abBulidWindow.OnDisable();
            dirCollectWindow.OnDisable();
            toolWindow.OnDisable();
        }
        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            WinTree.OnGUI(rs[1]);
            ToolBarTree.OnGUI(rs[0]);
            this.minSize = WinTree.minSize + new Vector2(0, ToolBarHeight);
        }
    }
}
