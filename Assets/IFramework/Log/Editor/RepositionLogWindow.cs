/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-17
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.IO;
using UnityEngine;
using IFramework.GUITool;

namespace IFramework
{
    [EditorWindowCacheAttribute("IFramework.Log")]
     partial class RepositionLogWindow : EditorWindow,IRectGUIDrawer,ILayoutGUIDrawer
    {

        private const string Preview = "Preview";
        private const string Name = "Name";
        private const string Path = "Path";

        private const string TitleStyle = "IN BigTitle";
        private const string EntryBackodd = "CN EntryBackodd";
        private const string EntryBackEven = "CN EntryBackEven";

    }

     partial class RepositionLogWindow : EditorWindow
    {
        private LogSetting info;
        private string infoPath;
        private Vector2 ScrollPos;
        private ListViewCalculator.ColumnSetting[] ViewSetting
        {
            get
            {
                return new ListViewCalculator.ColumnSetting[3]
                {
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=Preview,
                        Width=100,
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name=Name,
                        Width=200,
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        Name = Path,
                        Width = 600
                    }
                };

            }
        }

        private void OnEnable()
        {
            LoadInfo();

        }
        public void LoadInfo()
        {
            infoPath = FrameworkConfig.FrameworkPath.
                CombinePath("Log/Resources/" + RepositionLog.StoName + ".asset");
            if (!File.Exists(infoPath)) ScriptableObj.Create<LogSetting>(infoPath);
            info = ScriptableObj.Load<LogSetting>(infoPath);
            for (int i = 0; i < info.Infos.Count; i++)
            {
                LogEliminateItem item = info.Infos[i];
                if (item.Text == null)
                {
                    if (string.IsNullOrEmpty(item.Path))
                    {
                        ShowNotification(new GUIContent("Null Err"));
                    }
                    else
                    {
                        TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>(item.Path);
                        if (txt == null)
                        {
                            ShowNotification(new GUIContent("Not Found Err"));
                        }
                        else
                        {
                            item.Text = txt;
                            item.Name = item.Path.GetFileName();
                        }
                    }
                }
                else
                {
                    string path = AssetDatabase.GetAssetPath(item.Text);
                    item.Path = path;
                    item.Name = path.GetFileName();
                }
            }
            EditorUtility.ClearProgressBar();
        }


        private void OnDisable()
        {
            ScriptableObj.Update(info);
        }


        private TableViewCalculator table = new TableViewCalculator();
        public void ListView(Rect rect)
        {
            float LineHeight = 20;
            table.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ScrollPos, LineHeight, info.Infos.Count, ViewSetting);
            if (Event.current.type == EventType.Repaint)
                new GUIStyle(EntryBackodd).Draw(table.Position, false, false, false, false);

            this.LabelField(table.TitleRow.Position, "",new GUIStyle(TitleStyle))
                .LabelField(table.TitleRow[Preview].Position, Preview)
                .LabelField(table.TitleRow[Name].Position, Name)
                .LabelField(table.TitleRow[Path].Position, Path)
                .DrawScrollView(() =>
                    {
                        for (int i = table.FirstVisibleRow; i < table.LastVisibleRow + 1; i++)
                        {

                            GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                            if (Event.current.type == EventType.Repaint)
                                style.Draw(table.Rows[i].Position, false, false, table.Rows[i].Selected, false);
                            if (Event.current.modifiers == EventModifiers.Control &&
                                    Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    table.Rows[i].Position.Contains(Event.current.mousePosition))
                            {
                                table.ControlSelectRow(i);
                                Repaint();
                            }
                            else if (Event.current.modifiers == EventModifiers.Shift &&
                                            Event.current.button == 0 && Event.current.clickCount == 1 &&
                                            table.Rows[i].Position.Contains(Event.current.mousePosition))
                            {
                                table.ShiftSelectRow(i);

                                Repaint();
                            }
                            else if (Event.current.button == 0 && Event.current.clickCount == 1 &&
                                            table.Rows[i].Position.Contains(Event.current.mousePosition))
                            {
                                table.SelectRow(i);
                                Repaint();
                            }
                            Texture2D tx = AssetPreview.GetMiniThumbnail(info.Infos[i].Text);
                            this.Label(table.Rows[i][Preview].Position, new GUIContent(tx));
                            this.Label(table.Rows[i][Name].Position, info.Infos[i].Name);
                            this.Label(table.Rows[i][Path].Position, info.Infos[i].Path);
                        }
                    }, 
                    table.View,ref ScrollPos,table.Content, false, false
            );



            Handles.color = Color.black;
            for (int i = 0; i < table.TitleRow.Columns.Count; i++)
            {
                var item = table.TitleRow.Columns[i];

                if (i != 0)
                    Handles.DrawAAPolyLine(1, new Vector3(item.Position.x,
                                                            item.Position.y,
                                                            0),
                                              new Vector3(item.Position.x,
                                                            item.Position.y + item.Position.height - 2,
                                                            0));
            }
            table.Position.DrawOutLine(2, Color.black);


            Handles.color = Color.white;
        }

        private void OnGUI()
        {

            GUIStyle title = new GUIStyle("IN BigTitle");
            title.fontSize = 20;
            this.Label(new Rect(10, 5, position.width - 20, 35), "Log Ignore List", title);
            ListView(new Rect(10, 40, position.width - 20, position.height - 200));
            Eve();
            this.DrawArea(() => {
                this.DrawHorizontal(() =>
                {
                    this.Toggle("Enable",ref info.Enable);
                    using (new EditorGUI.DisabledScope(!info.Enable))
                    {
                        this.Toggle("LogEnable",ref info.LogEnable);
                        this.Toggle("WarnningEnable",ref info.WarnningEnable);
                        this.Toggle("ErrEnable",ref info.ErrEnable);
                    }
                });
                using (new EditorGUI.DisabledScope(!info.Enable))
                {
                    this.IntSlider("LogLevel ",ref info.LogLevel, 0, 100);
                    this.IntSlider("WarnningLevel ",ref info.WarnningLevel, 0, 100);
                    this.IntSlider("ErrLevel ",ref info.ErrLevel, 0, 100);
                }


            }, new Rect(10, position.height - 160, position.width - 20, 180));
        }


        private void Eve()
        {
            Event eve = Event.current;
            if (eve.button == 0 && eve.clickCount == 1 &&
                    (!table.View.Contains(eve.mousePosition) ||
                        (table.View.Contains(eve.mousePosition) &&
                         !table.Content.Contains(eve.mousePosition))))
            {
                table.SelectNone();
                Repaint();
            }
            DragAndDropIInfo dragInfo = DragAndDropUtil.Drag(eve, table.View);
            if (table.View.Contains(eve.mousePosition) && dragInfo.EnterArera && dragInfo.Finsh)
            {
                for (int i = 0; i < dragInfo.paths.Length; i++)
                {
                    float progress = (float)i / dragInfo.paths.Length;
                    EditorUtility.DisplayProgressBar(string.Format("Add Script {0}/{1}",i, dragInfo.paths.Length), dragInfo.paths[i], progress);
                    SaveInfo(dragInfo.paths[i]);
                }
                ScriptableObj.Update(info);
                EditorUtility.ClearProgressBar();
            }

            if (eve.button == 1 && eve.clickCount == 1 &&
                        table.Content.Contains(eve.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () => {

                    for (int i = table.SelectedRows.Count - 1; i >= 0; i--)
                    {
                        float progress = (float)i / table.SelectedRows.Count;
                        EditorUtility.DisplayProgressBar(string.Format("Delete Script {0}/{1}", i, table.SelectedRows.Count),"", progress);
                        this.info.Infos.RemoveAt(table.SelectedRows[i].RowID);
                    }
                    EditorUtility.ClearProgressBar();
                    ScriptableObj.Update(info);
                    table.SelectNone();
                });
                menu.AddItem(new GUIContent("Select Script"), false, () =>
                {
                    for (int i = table.Rows.Count - 1; i >= 0; i--)
                    {
                        if (table.Rows[i].Position.Contains(eve.mousePosition))
                        {
                            Selection.activeObject = this.info.Infos[i].Text;
                            break;
                        }
                    }
                });
                menu.ShowAsContext();
                if (eve.type != EventType.Layout)
                    eve.Use();
            }
        }

        public void SaveInfo(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (txt == null) return;
            for (int i = 0; i < info.Infos.Count; i++)
            {
                if (info.Infos[i].Path == path) return;
            }
            info.Infos.Add(new LogEliminateItem()
            {
                Path = path,
                Name = path.GetFileName(),
                Text = txt
            });
        }
    }
}

