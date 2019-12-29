/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-05
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IFramework
{

    public static class ReorderableListUtil
    {
        public static ReorderableList Create(SerializedProperty property, float space = 10f)
        {
            return Create(property, true, true, true, true, null, space);
        }

        public static ReorderableList Create(SerializedProperty property, List<Column> cols, float space = 10f)
        {
            return Create(property, true, true, true, true, cols, space);
        }
        public static ReorderableList Create(SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, float space = 10f)
        {
            return Create(property, draggable, displayHeader, displayAddButton, displayRemoveButton, null, space);
        }
        public static ReorderableList Create(SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, List<Column> cols, float space = 10f)
        {
            var list = new ReorderableList(property.serializedObject, property, draggable, displayHeader, displayAddButton, displayRemoveButton);

            list.drawElementCallback = DrawElement(list, cols, space);
            list.drawHeaderCallback = DrawHeader(list, Calc(list, cols), space);

            return list;
        }
        private static ReorderableList.ElementCallbackDelegate DrawElement(ReorderableList list, List<Column> cols, float space)
        {
            return (rect, index, isActive, isFocused) =>
            {
                var property = list.serializedProperty;
                var columns = Calc(list,cols);
                var layouts = CalcWidth(columns, rect, space);
                rect.height = EditorGUIUtility.singleLineHeight;
                for (var j = 0; j < columns.Count; j++)
                {
                    var c = columns[j];
                    rect.width = layouts[j];

                    //Log.L(c.Width + "  " + layouts[j]);

                    var p = property.GetArrayElementAtIndex(index).FindPropertyRelative(c.PropertyName);
                    //if (p!=null)
                    {
                        EditorGUI.PropertyField(rect, p, GUIContent.none);
                        rect.x += rect.width + space;
                    }

                }
            };
        }

        private static ReorderableList.HeaderCallbackDelegate DrawHeader(ReorderableList list, List<Column> cols, float space)
        {
            return (rect) =>
            {
                var columns = cols;

                if (list.draggable)
                {
                    rect.width -= 15;
                    rect.x += 15;
                }

                var layouts = CalcWidth(columns, rect, space);
                rect.height = EditorGUIUtility.singleLineHeight;
                for (var j = 0; j < columns.Count; j++)
                {
                    var c = columns[j];

                    rect.width = layouts[j];
                    EditorGUI.LabelField(rect, c.DisplayName);
                    rect.x += rect.width + space;
                }
            };
        }

        private static List<Column> Calc(ReorderableList list, List<Column> cols)
        {
            var property = list.serializedProperty;
            if (cols == null) cols = new List<Column>();
            property = list.serializedProperty;
            if (property.isArray && property.arraySize > 0)
            {
                SerializedProperty it = property.GetArrayElementAtIndex(0).Copy();
                var prefix = it.propertyPath; 
                var index = 0;
                if (it.Next(true))
                {
                    do
                    {
                        if (it.propertyPath.StartsWith(prefix))
                        {
                            if (index >= cols.Count)
                            {
                                var c = new Column();
                                c.PropertyName = it.propertyPath.Substring(prefix.Length + 1); 
                                c.DisplayName = string.IsNullOrEmpty(c.DisplayName) ? c.PropertyName : c.DisplayName;
                                cols.Add(c);
                            }
                            else
                            {
                                var c = cols[index];
                                c.PropertyName = it.propertyPath.Substring(prefix.Length + 1);
                                c.DisplayName = string.IsNullOrEmpty(c.DisplayName) ? c.PropertyName : c.DisplayName;
                            }
                        }
                        else
                        {
                            break;
                        }
                        index += 1;
                    }
                    while (it.Next(false));
                }
            }
            return cols;
        }

        private static List<float> CalcWidth(List<Column> columns, Rect rect, float space)
        {
            var autoWidth = rect.width;
            var autoCount = 0;
            foreach (var column in columns)
            {
                if (column.Width!=0)
                {
                    autoWidth -= column.Width;
                }
                else
                {
                    autoCount += 1;
                }
            }

            autoWidth -= (columns.Count - 1) * space;
            autoWidth /= autoCount;

            var widths = new List<float>(columns.Count);
            foreach (var column in columns)
            {
                if (column.Width!=0)
                {
                    widths.Add(column.Width);
                }
                else
                {
                    widths.Add(autoWidth);
                    //column.Width = autoWidth;
                }
            }
            
            return widths;
        }
      

        public static bool DrawWithFold(ReorderableList list, string label = null)
        {
            var property = list.serializedProperty;
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label != null ? label : property.displayName);
            if (property.isExpanded)
            {
                list.DoLayoutList();
            }
            return property.isExpanded;
        }
        public static void Draw(ReorderableList list)
        {
            list.DoLayoutList();
        }

        public class Column
        {
            public string DisplayName;
            internal string PropertyName;
            public float Width;
        }

    }
}
