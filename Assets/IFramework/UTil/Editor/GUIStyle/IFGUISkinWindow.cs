/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace IFramework
{
    [EditorWindowCache("GUIStyle")]
    public partial class IFGUISkinWindow : EditorWindow
    {


    }

    public partial class IFGUISkinWindow:EditorWindow,IRectGUIDrawer,ILayoutGUIDrawer
	{
        private const string Name = "Name";
        private const string Style = "Style";
        private const string Btn = "Copy";
        private bool Tog ;

        private string StoPath;
        private IFGUISKin Skin;
        private Vector2 ScrollPos;
        private float TopHeight=30;
        private string input=string .Empty;
        private SearchFieldDrawer searchField = new SearchFieldDrawer();

        private void OnEnable()
        {
            StoPath = FrameworkConfig.EditorPath.
                CombinePath("GUIStyle/Resources/" + IFGUISkinUtil.AssetName+".asset");
            if (!System.IO.File.Exists(StoPath)) ScriptableObj.Create<IFGUISKin>(StoPath);
            Skin = ScriptableObj.Load<IFGUISKin>(StoPath);
            searchField = new SearchFieldDrawer();
            searchField.onValueChange += (str) =>
            {
                input = str;
            };
        }
        private ListViewCalculator listView = new ListViewCalculator();
        private List<GUIStyle> mathList = new List<GUIStyle>();
        private void OnGUI()
        {
            this.DrawHorizontal(() =>
            {
                this.Space(10);
                this.Button(() =>
                {
                    Fresh();
                }, EditorGUIUtility.IconContent("d_TreeEditor.Refresh"),
                   GUILayout.Width(TopHeight),
                    GUILayout.Height(TopHeight));
                this.Space(10);
                this.Button(() =>
                {
                    ShowNotification(new GUIContent("Click And See \nInfo"));
                }, "Tip", GUILayout.Height(TopHeight));
                GUILayout.Label("", GUILayout.Height(TopHeight));
                searchField.OnGUI(GUILayoutUtility.GetLastRect());
                this.Space(10);
                this.Toggle(ref Tog, GUILayout.Width(20));
            }, GUILayout.Height(TopHeight));

            mathList.Clear();
            for (int i = 0; i < Skin.Styles.Count; i++)
                if (Skin.Styles[i].name.ToLower().Contains(input.ToLower()))
                    mathList.Add(Skin.Styles[i]);

            Rect rect = new Rect(0,  TopHeight, position.width, position.height - TopHeight).Zoom(AnchorType.MiddleCenter, -5);
            listView.Calc(rect, rect.position, ScrollPos, 30, mathList.Count, new ListViewCalculator.ColumnSetting[] {
                new ListViewCalculator.ColumnSetting(){ Name=Style,Width=600,OffSetY=-4},

                new ListViewCalculator.ColumnSetting(){ Name=Name,Width=200,OffSetY=-4,OffsetX=-10},
                new ListViewCalculator.ColumnSetting(){ Name=Btn,Width=100,OffSetY=-4},
            });
            this.DrawScrollView(() =>
            {
                for (int i = listView.FirstVisibleRow; i < listView.LastVisibleRow + 1; i++)
                {
                    this.Label(listView.Rows[i][Name].Position, mathList[i].name);
                    this.Button(()=> {
                        GUIUtility.systemCopyBuffer = mathList[i].name;
                    },listView.Rows[i][Btn].Position, Btn);
                    if (Event.current.type == EventType.Repaint)
                        if (Tog)
                        {
                            mathList[i].Draw(listView.Rows[i][Style].Position, mathList[i].name, false, false, false, false);
                        }
                        else
                        {
                            mathList[i].Draw(listView.Rows[i][Style].Position,  false, false, false, false);
                        }

                }
            }, listView.View,ref ScrollPos, listView.Content);
                      
        }
        private void Fresh()
        {
            if (!System.IO.File.Exists(StoPath)) ScriptableObj.Create<IFGUISKin>(StoPath);
                Skin = ScriptableObj.Load<IFGUISKin>(StoPath);
            Skin.Styles.Clear();
            PropertyInfo[] infos = typeof(EditorStyles).
                GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < infos.Length; i++)
            {
                EditorUtility.DisplayProgressBar(string .Format( "Fresh{0}/{1}",i,infos.Length), 
                                                        string.Empty,
                                                        (float)i/ infos.Length);
                PropertyInfo info = infos[i];
                object o = info.GetValue(null, null);
                
                if (o.GetType() == typeof(GUIStyle))
                {
                    GUIStyle style = o as GUIStyle;
                 
                    Skin.Styles.Add(style);
                }
            }

            Skin.Styles.Add(new GUIStyle("CN Box"));
            Skin.Styles.Add(new GUIStyle("Button"));
            Skin.Styles.Add(new GUIStyle("CN CountBadge"));
            Skin.Styles.Add(new GUIStyle("ToolbarButton"));
            Skin.Styles.Add(new GUIStyle("Toolbar"));
            Skin.Styles.Add(new GUIStyle("CN EntryInfo"));
            Skin.Styles.Add(new GUIStyle("CN EntryWarn"));
            Skin.Styles.Add(new GUIStyle("CN EntryError"));
            Skin.Styles.Add(new GUIStyle("CN EntryBackEven") );
            Skin.Styles.Add(new GUIStyle("CN EntryBackodd"));
            Skin.Styles.Add(new GUIStyle("CN Message"));
            Skin.Styles.Add(new GUIStyle("CN StatusError"));
            Skin.Styles.Add(new GUIStyle("CN StatusWarn"));
            Skin.Styles.Add(new GUIStyle("CN StatusInfo"));
   
            Skin.Styles.Add(new GUIStyle("LODBlackBox"));
            Skin.Styles.Add(new GUIStyle("GameViewBackground"));
            Skin.Styles.Add(new GUIStyle("WindowBackground"));
            Skin.Styles.Add(new GUIStyle("MiniToolbarButton"));
            Skin.Styles.Add(new GUIStyle("dockarea"));
            Skin.Styles.Add(new GUIStyle("hostview"));
            Skin.Styles.Add(new GUIStyle("dragtabdropwindow"));
            Skin.Styles.Add(new GUIStyle("PaneOptions"));
            Skin.Styles.Add(new GUIStyle("SelectionRect"));
            Skin.Styles.Add(new GUIStyle("window"));
            Skin.Styles.Add(new GUIStyle("WindowBottomResize"));
            Skin.Styles.Add(new GUIStyle("dragtab"));
            Skin.Styles.Add(new GUIStyle("IN LockButton"));
            Skin.Styles.Add(new GUIStyle("WinBtnClose"));
           
            foreach (GUIStyle item in GUI.skin)
            {
                Skin.Styles.Add(item);

            }
            EditorUtility.ClearProgressBar();
            ScriptableObj.Update<IFGUISKin>(Skin);
        }
    }
}
