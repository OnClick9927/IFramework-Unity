/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.GUITool
{
    [Serializable]
    public class TableViewCalculator
    {
        public TableViewCalculator() { ListView = new ListViewCalculator(); }
        private ListViewCalculator ListView;
        public Rect Position { get; private set; }
        public Rect View { get { return ListView.View; } }
        public Rect Content { get { return ListView.Content; } }
        public ListViewCalculator.Row TitleRow { get; private set; }
        public List<ListViewCalculator.Row> Rows { get { return ListView.Rows; } }
        public int FirstVisibleRow { get { return ListView.FirstVisibleRow; } }
        public int LastVisibleRow { get { return ListView.LastVisibleRow; } }
        public float TitleRowOffsetY = 0.2f;
        public List<ListViewCalculator.Row> SelectedRows { get { return ListView.SelectedRows; } }


        public void Calc(Rect position, Vector2 contentOffset, Vector2 scroll, float rowHeight, int rowCount, ListViewCalculator.ColumnSetting[] setting)
        {
            this.Position = position;
            float Mutl = TitleRowOffsetY + 1;
            TitleRow = new ListViewCalculator.Row()
            {
                Height = rowHeight,
                RowID = 0,
                Position = new Rect(position.x,
                                        position.y,
                                        position.width,
                                        rowHeight * Mutl)
            };
            ListViewCalculator.ColumnSetting[] Tsetting = new ListViewCalculator.ColumnSetting[setting.Length];
            for (int i = 0; i < Tsetting.Length; i++)
            {
                Tsetting[i] = new ListViewCalculator.ColumnSetting()
                {
                    Width = setting[i].Width,
                    Name = setting[i].Name,
                    OffsetX = setting[i].TitleoffsetX,
                    OffSetY = -TitleRowOffsetY * rowHeight,
                    AnchorType = setting[i].TitleAnchorType,
                    TitleAnchorType = setting[i].TitleAnchorType
                };
            }
            TitleRow.Calc(Tsetting);
            TitleRow.CalacLocal(new Rect(position.x,
                                        position.y,
                                        position.width,
                                        rowHeight * Mutl));

            if (View.width > TitleRow.Width)
            {
                float offset = View.width - TitleRow.Width;
                TitleRow.Width += offset;
                TitleRow.LocalPostion.width += offset;
                TitleRow.Columns[TitleRow.ColumnCount - 1].Position.width += offset;
                TitleRow.Columns[TitleRow.ColumnCount - 1].LocalPostion.width += offset;
            }

            for (int i = 0; i < TitleRow.ColumnCount; i++)
            {
                ListViewCalculator.Column item = TitleRow.Columns[i];
                item.Position.x -= scroll.x;
                item.LocalPostion.x -= scroll.x;
            }

            TitleRow.Position.x = View.x;
            TitleRow.Position.width = View.width;

            ListView.Calc(new Rect(position.x,
                                position.y + rowHeight * Mutl,
                                position.width,
                                position.height - rowHeight), contentOffset, scroll,
                                    rowHeight,
                                    rowCount, setting);

            ListView.View.height -= TitleRowOffsetY * rowHeight;



        }




        public void ShiftSelectRow(int index)
        {
            ListView.ShiftSelectRow(index);
        }
        public void ControlSelectRow(int index)
        {
            ListView.ControlSelectRow(index);
        }
        public void SelectRow(int index)
        {
            ListView.SelectRow(index);
        }
        public void SelectNone()
        {
            ListView.SelectNone();
        }
        public void SelectAll()
        {
            ListView.SelectAll();
        }
    }
}
