/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-04
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [EditorWindowCache("EditorIcon")]
    internal partial class IFEditorIconWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            GetWindow<IFEditorIconWindow>();
        }
        private class Buildt_inIconView : IRectGUIDrawer, ILayoutGUIDrawer
        {
            public Buildt_inIcon content;
            public void OnGUI(Rect worldRect)
            {
                this.Button(() => {
                    IFEditorIconWindow.Instance.ShowNotification(new GUIContent(content.iconName));
                    Selection.activeObject = content;
                }, worldRect, content.content);
            }
            public bool isMatchRules(string input)
            {
                if (string.IsNullOrEmpty(input)) return true;
                if (content.iconName.ToLower().Contains(input.ToLower()))
                    return true;
                return false;
            }
        }
        private class MouseCursorIconView : IRectGUIDrawer, ILayoutGUIDrawer
        {
            public MouseCursor content;
            public void OnGUI( Rect worldRect)
            {
               this. Button(() => {
                   IFEditorIconWindow.Instance.ShowNotification(new GUIContent(Enum.GetName(typeof(MouseCursor), content)));

               }, Enum.GetName(typeof(MouseCursor), content), GUILayout.Height(25));
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), content);
            }
        }

    }

    internal partial class IFEditorIconWindow:EditorWindow,IRectGUIDrawer,ILayoutGUIDrawer
	{

        private static IFEditorIconWindow instance;
        private IFEditorIconWindow() { instance = this; }
        public static IFEditorIconWindow Instance { get { return instance; } }

        private IFEditorIconSto sto;
        private string StoPath;
        private Vector2 leftPos,rightPos;

        private List<Buildt_inIcon> Icons = new List<Buildt_inIcon>();

        private List<Buildt_inIconView> IconView = new List<Buildt_inIconView>();
        private List<MouseCursorIconView> mouseView = new List<MouseCursorIconView>();

        private void OnEnable()
        {
            searchField = new SearchFieldDrawer();
            searchField.onValueChange += (str) => {
                input = str;
            };
            StoPath = FrameworkConfig.UtilPath.CombinePath("Built_inIcon/Resources").CombinePath(IFIcons .StoName+ ".asset");
            if (!System.IO.File.Exists(StoPath)) Fresh();
            sto = ScriptableObj.Load<IFEditorIconSto>(StoPath);
            Icons.Clear();
            IconView.Clear();
            mouseView.Clear();
            Icons.AddRange(sto.Buildt_inIcon);

            foreach (MouseCursor item in Enum.GetValues(typeof(MouseCursor)))
            {
                MouseCursorIconView view = new MouseCursorIconView();
                view.content = item;
                mouseView.Add(view);
            }
            for (int i = Icons.Count - 1; i >= 0; i--)
            {
                EditorUtility.DisplayProgressBar(string.Format("Check {0}", Icons[i].iconName),
                string.Empty, (float)Icons.Count - i / Icons.Count);
                if (!EditorGUIUtility.FindTexture(Icons[i].iconName))
                {
                    Buildt_inIcon icon = Icons[i];
                    Icons.Remove(icon);
                }
            }

            for (int i = 0; i < Icons.Count; i++)
            {
                EditorUtility.DisplayProgressBar(string.Format("Load {0}", Icons[i].iconName),
          string.Empty, (float) i / Icons.Count);
                Buildt_inIconView view = new Buildt_inIconView();
                view.content = Icons[i];
                IconView.Add(view);
            }
            EditorUtility.ClearProgressBar();
        }
        private float TopHeight=50;
        private float leftWith=200;
        private string input;

        private ListViewCalculator listView = new ListViewCalculator();
        private List<Buildt_inIconView> MathList = new List<Buildt_inIconView>();
        private List<ListViewCalculator.ColumnSetting> Setting = new List<ListViewCalculator.ColumnSetting>();
        private float IconSize = 60;
        void OnGUI()
        {
            this.DrawArea(() =>
            {
                 this.DrawScrollView(() =>
                {
                    for (int i = 0; i < mouseView.Count; i++)
                    {
                        mouseView[i].OnGUI(/*new Rect(),*/ new Rect());
                    }
                },ref leftPos);
            }, new Rect(0, TopHeight, leftWith, position.height-TopHeight));

            MathList.Clear();
            for (int i = 0; i < IconView.Count; i++)
                if (IconView[i].isMatchRules(input))
                    MathList.Add(IconView[i]);

            Rect listviewRect = new Rect(leftWith, TopHeight, position.width - leftWith, position.height - TopHeight);

            int colCount =(int)( listviewRect.width / IconSize);
            int rowCount = MathList.Count / colCount+1;
            Setting.Clear();
            for (int i = 0; i < colCount; i++)
            {
                Setting.Add(new ListViewCalculator.ColumnSetting() { Name = i.ToString(), Width = IconSize });
            }
            listView.Calc(listviewRect, listviewRect.position,
                rightPos, IconSize, rowCount, Setting.ToArray());
            this.DrawScrollView(() => {

                int helpIndex = listView.FirstVisibleRow * colCount;
                int rowIndex = listView.FirstVisibleRow;
                while (helpIndex <= MathList.Count)
                {
                    ListViewCalculator.Row row;
                    try
                    {

                        row = listView.Rows[rowIndex];
                    }
                    catch (Exception)
                    {
                        Debug.Log(rowIndex);
                        throw;
                    }
                    
                    int j = 0;
                    if (row.ColumnCount <= 0) break;
                    while (helpIndex < MathList.Count)
                    {
                        ListViewCalculator.Column item;
                        try
                        {
                             item= row.Columns[j];
                            if (item.Position.width != item.Position.height) item.Position.width = item.Position.height;
                            MathList[helpIndex].OnGUI(item.Position);
                            if (++helpIndex >= MathList.Count) break;
                            if (++j >= row.Columns.Count ) break;
                        }
                        catch (Exception)
                        {
                            Debug.Log(j);
                            throw;
                        }
                       
                    }
                    if (rowIndex++ > listView.LastVisibleRow) break;
                    if (rowIndex >= listView.RowCount) break;

                }

            }, listView.View,ref rightPos, listView.Content);


            this.DrawHorizontal(() =>
            {
                this.Space(10);
                this.Button(() =>
                {
                     Fresh();
                }, EditorGUIUtility.IconContent("d_TreeEditor.Refresh"),
                   GUILayout.Width(TopHeight),
                   GUILayout.Height(TopHeight));
                this.Button(() =>
                {
                   ShowNotification(new GUIContent("Click And See \nInfo"));
                }, "Tip", GUILayout.Height(TopHeight));
                GUILayout.Label("",GUILayout.Height(TopHeight));
                 searchField.OnGUI(GUILayoutUtility.GetLastRect());
                this.Space(10);
            }, GUILayout.Height(TopHeight));
        }
        private SearchFieldDrawer searchField;

        private void Fresh()
        {
            string[] text = Resources.Load<TextAsset>("Built_inIcon").text.Replace("\r\n", "\n").Split("\n"[0]);
            Icons.Clear();
            IconView.Clear();
            List<Texture2D> txs = new List<Texture2D>();

            for (int i = 0; i < text.Length; i++)
            {
                if (!EditorGUIUtility.FindTexture(text[i])) continue;
              
                Buildt_inIcon buildt_inIcon = CreateInstance<Buildt_inIcon>();
                buildt_inIcon.iconName = text[i];
                buildt_inIcon.OnEnable();

                Texture tx = EditorGUIUtility.IconContent(text[i]).image;

                EditorUtility.DisplayProgressBar(text[i],
                                    string.Format("Fresh {0}/{1}", i, text.Length), 
                                    (float)i / text.Length);

               
                buildt_inIcon.content = new GUIContent(tx.CreateReadableTexture());
                txs.Add(buildt_inIcon.content.image as Texture2D);
                Icons.Add(buildt_inIcon);
                Buildt_inIconView view = new Buildt_inIconView();
                view.content = buildt_inIcon;
                IconView.Add(view);
            }
            EditorUtility.ClearProgressBar();

            if (!System.IO.File.Exists(StoPath)) ScriptableObj.Create<IFEditorIconSto>(StoPath);
            sto = ScriptableObj.Load<IFEditorIconSto>(StoPath);

            sto.Buildt_inIcon.Clear();
            //sto.MouseCursorIcon.Clear();
            sto.Buildt_inIcon.AddRange(Icons);
            //sto.MouseCursorIcon .AddRange( MouseCursorIcon);

            ScriptableObj.Update<IFEditorIconSto>(sto, sto.Buildt_inIcon.ToArray());
            //ScriptableObj.Update<IFEditorIconSto>(sto, sto.MouseCursorIcon.ToArray());
            ScriptableObj.Update<IFEditorIconSto>(sto, txs.ToArray());

        }



    }

}
