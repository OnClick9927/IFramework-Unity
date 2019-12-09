/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IFramework.GUITool
{
    [Serializable]
    public class ListViewCalculator
    {
        [Serializable]
        public class ColumnSetting
        {
            public float Width;
            public string Name;
            public float OffsetX = -2;
            public float OffSetY = -2;
            public float TitleoffsetX = -8;
            public AnchorType AnchorType = AnchorType.MiddleCenter;
            public AnchorType TitleAnchorType = AnchorType.LowerCenter;
        }
        [Serializable]
        public class Column
        {
            public string Name;
            public int RowID;
            public int ColumnID;
            public Rect Position;
            public Rect LocalPostion;
        }
        [Serializable]
        public class Row : IComparable
        {
            public Column this[int Key] { get { return Columns[Key]; } }
            public Column this[string Key]
            {
                get
                {
                    for (int i = 0; i < Columns.Count; i++)
                        if (Columns[i].Name == Key) return Columns[i];
                    return default(Column);
                }
            }
            public float Width;
            public float Height;
            public bool Selected;
            public int RowID;
            public Rect Position;
            public Rect LocalPostion;
            public Row() { Columns = new List<Column>(); }
            public int ColumnCount { get { return Columns.Count; } }
            public readonly List<Column> Columns;
            internal void Calc(ColumnSetting[] settings)
            {
                while (Columns.Count > settings.Length) Columns.RemoveAt(Columns.Count - 1);
                while (Columns.Count < settings.Length) Columns.Add(new Column());
                Width = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    Column item = Columns[i];
                    item.RowID = RowID;
                    item.ColumnID = i;
                    item.Name = settings[i].Name;
                    item.Position = new Rect(Position.x + Width,
                                            Position.y,
                                            settings[i].Width,
                                            Position.height).
                                            Zoom(settings[i].AnchorType, new Vector2(settings[i].OffsetX, settings[i].OffSetY));
                    Width += settings[i].Width;
                }
                Position.width = Width;
            }
            internal void CalacLocal(Rect parent)
            {
                LocalPostion = Position;
                LocalPostion.x -= parent.x;
                LocalPostion.y -= parent.y;
                for (int i = 0; i < Columns.Count; i++)
                {
                    Columns[i].LocalPostion = Columns[i].Position;
                    Columns[i].LocalPostion.x -= Position.x;
                    Columns[i].LocalPostion.y -= Position.y;
                }
            }

            public int CompareTo(object obj)
            {
                Row row = obj as Row;
                return this.RowID.CompareTo(row.RowID);
            }
        }
        public ListViewCalculator() {
            SelectedRows = new List<Row>();
            Rows = new List<Row>();
        }
        public readonly List<Row> SelectedRows;
        public readonly List<Row> Rows;
        private Rect content;
        public Rect Content { get { return content; } }
        public Rect View;
        public float RowHeight { get; set; }
        public int RowCount { get; private set; }
        public int FirstVisibleRow { get; private set; }
        public int LastVisibleRow { get; private set; }

        public void Calc(Rect view, Vector2 contentOffset, Vector2 scroll, float rowHeight, int rowCount, ColumnSetting[] setting)
        {
            FirstVisibleRow = int.MaxValue;
            LastVisibleRow = int.MinValue;
            this.RowHeight = rowHeight;
            this.RowCount = rowCount;

            View = view;
            if (View.height <= rowHeight) View.height = rowHeight;

            this.content.position = contentOffset;
            content.height = rowHeight * rowCount;

            while (Rows.Count > rowCount) Rows.RemoveAt(Rows.Count - 1);
            while (Rows.Count < rowCount) Rows.Add(new Row());

            for (int i = 0; i < Rows.Count; i++)
            {
                if (Content.yMin - scroll.y + (i +1) * rowHeight > View.yMax) break;
                if (Content.yMin - scroll.y + (i + 1) * rowHeight < View.yMin) continue;
                FirstVisibleRow = FirstVisibleRow > i ? i : FirstVisibleRow;
                LastVisibleRow = LastVisibleRow > i ? LastVisibleRow : i;

                Row row = Rows[i];
                row.Height = rowHeight;
                row.RowID = i;
                row.Position = new Rect(view.x + 2,
                                        view.y + i * rowHeight,
                                        view.width - 4,
                                        rowHeight).Zoom(AnchorType.MiddleCenter, new Vector2(0, -1));
                row.Calc(setting);
                if (View.width > row.Width)
                {
                    float offset = View.width - 4 - row.Width;
                    row.Width += offset;
                    row.Columns[row.ColumnCount - 1].Position.width += offset;
                }
                row.Position.width = row.Width;
                content.width = row.Width - 10;
                row.CalacLocal(Content);

             
       
            }
        }


        private void Sort()
        {
            SelectedRows.Sort();
        }
        public void SelectRow(int index)
        {
            if (index < 0 || index >= Rows.Count) return;
            SelectedRows.Clear();

            for (int i = 0; i < Rows.Count; i++)
            {
                if (i == index)
                {
                    Rows[i].Selected = true;
                    if (!SelectedRows.Contains(Rows[i]))
                    {
                        SelectedRows.Add(Rows[i]);
                    }
                }
                else
                    Rows[i].Selected = false;
            }
        }
        public void ControlSelectRow(int index)
        {
            if (index < 0 || index >= Rows.Count) return;
            Rows[index].Selected = true;
            if (!SelectedRows.Contains(Rows[index]))
            {
                SelectedRows.Add(Rows[index]);
            }
            Sort();

        }
        public void ShiftSelectRow(int index)
        {
            if (index < 0 || index >= Rows.Count) return;
            int temp = -1;
            for (int j = 0; j < Rows.Count; j++)
                if (Rows[j].Selected)
                {
                    temp = j; break;
                }
            if (temp == -1 || temp == index) Rows[index].Selected = true;
            else if (temp < index)
                for (int j = temp; j <= index; j++)
                {
                    Rows[j].Selected = true;
                    if (!SelectedRows.Contains(Rows[j]))
                    {
                        SelectedRows.Add(Rows[j]);
                    }

                }
            else if (temp > index)
                for (int j = index; j < temp; j++)
                {
                    Rows[j].Selected = true;
                    if (!SelectedRows.Contains(Rows[j]))
                    {
                        SelectedRows.Add(Rows[j]);
                    }
                }

            Sort();


        }
        public void SelectNone()
        {
            for (int j = 0; j < Rows.Count; j++)
                Rows[j].Selected = false;
            SelectedRows.Clear();
        }
        public void SelectAll()
        {
            SelectedRows.Clear();
            for (int j = 0; j < Rows.Count; j++)
            {

                Rows[j].Selected = true;
                SelectedRows.Add(Rows[j]);
            }
        }
    }

}
