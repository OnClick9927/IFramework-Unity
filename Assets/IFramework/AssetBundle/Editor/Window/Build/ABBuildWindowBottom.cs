/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using UnityEditor;
using UnityEngine;

namespace IFramework.AB
{
    public class ABBuildWindowBottom : IRectGUIDrawer,ILayoutGUIDrawer
    {
        private const string EntryBackodd = "CN EntryBackodd";
        private const string EntryBackEven = "CN EntryBackEven";

        private const string Preview = "Preview";
        private const string AssetName = "AssetName";
        private const float LineHeight = 20;

        private ListViewCalculator.ColumnSetting[] Setting
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


        private Vector2 scrollPos;
        private AssetDependenceInfo Info { get { return ABWindow.Instance.BuildWindow.ChoosedAsset; } }

        private TableViewCalculator table=new TableViewCalculator();

        public void OnGUI(Rect position)
        {
            position = position.Zoom(AnchorType.LowerCenter, -2);

            this.Box(position);
            Handles.color = Color.black;
            position.DrawOutLine(2, Color.black);
   

            Handles.color = Color.white;

            if (Info == null) return;
            table.Calc(position, new Vector2(position.x, position.y+LineHeight), scrollPos, LineHeight, Info.AssetBundles.Count, Setting);

           this. DrawArea(() =>
            {

                this.Label(table.TitleRow[AssetName].LocalPostion, Info.AssetPath);
                this.Label(table.TitleRow[Preview].LocalPostion, Info.ThumbNail);

            }, table.TitleRow.Position);
           this.DrawScrollView(() =>
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {

                    GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                    if (Event.current.type == EventType.Repaint)
                        style.Draw(table.Rows[i].Position, false, false, table.Rows[i].Selected, false);
                    this.Label(table.Rows[i][AssetName].Position, Info.AssetBundles[i]);
                    //this.Label(table.Rows[i][Preview].Position, choo.ThumbNail);


                }
            }, table.View,
              ref  scrollPos,
                table.Content, false, false);


        }

     
    }

}
