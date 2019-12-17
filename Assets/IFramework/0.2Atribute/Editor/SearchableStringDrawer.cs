/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using IFramework.GUITool;

namespace IFramework
{
    [CustomPropertyDrawer(typeof(SearchableStringAttribute))]
    public class SearchableStringDrawer : PropertyDrawer
    {
        private int idHash;
        private object GetParentObjectOfProperty(string path, object obj)
        {
            string[] fields = path.Split('.');
            if (fields.Length == 1) return obj;
            FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            obj = fi.GetValue(obj);
            return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type != "string")
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
            SearchableStringAttribute sr = this.attribute as SearchableStringAttribute;
            object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);

            string[] array = parent.GetType().GetField(sr.searchArray).GetValue(parent) as string[];

            if (array == null || array.Length==0)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
            if (idHash == 0) idHash = "SearchableStringDrawer".GetHashCode();
            int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText =new GUIContent(property.stringValue);
            if (DropdownButton(id, position, buttonText))
            {
                int j = -1;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i]==property.stringValue)
                    {
                        j = i;
                        break;
                    }
                }
                SearchablePopup.Show(position, array,j, (index,str)=> {
                        property.stringValue = str;
                        property.serializedObject.ApplyModifiedProperties();
                    });
            }
            EditorGUI.EndProperty();
        }

        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    EditorStyles.toolbarTextField.Draw(position, content, id, false);
                    break;
            }
            return false;
        }

        private class SearchablePopup : PopupWindowContent
        {
            public class InnerSearchField : FocusAbleGUIDrawer
            {
                private class Styles
                {
                    public static GUIStyle SearchTextFieldStyle = new GUIStyle("ToolbarSeachTextField");
                    public static GUIStyle SearchCancelButtonStyle = new GUIStyle("SearchCancelButton");
                    public static GUIStyle SearchCancelButtonEmptyStyle = new GUIStyle("SearchCancelButtonEmpty");
                }

                public string OnGUI(Rect position, string value)
                {
                    GUIStyle cancelBtnStyle = string.IsNullOrEmpty(value) ? Styles.SearchCancelButtonEmptyStyle : Styles.SearchCancelButtonStyle;
                    position.width -= cancelBtnStyle.fixedWidth;

                    Styles.SearchTextFieldStyle.fixedHeight = position.height;
                    cancelBtnStyle.fixedHeight = position.height;

                    Styles.SearchTextFieldStyle.alignment = TextAnchor.MiddleLeft;
                    while (Styles.SearchTextFieldStyle.lineHeight <position.height - 15)
                    {
                        Styles.SearchTextFieldStyle.fontSize++;
                    }
                    GUI.SetNextControlName(FocusID);

                    value = GUI.TextField(new Rect(position.x,
                                                                     position.y + 1,
                                                                     position.width,
                                                                     position.height - 1),
                                                                     value,
                                                                     Styles.SearchTextFieldStyle);
                    if (GUI.Button(new Rect(position.x + position.width,
                                            position.y + 1,
                                            cancelBtnStyle.fixedWidth,
                                            cancelBtnStyle.fixedHeight
                                            ),
                                            GUIContent.none,
                                            cancelBtnStyle))
                    {
                        value = string.Empty;
                        GUI.changed = true;
                        GUIUtility.keyboardControl = 0;
                    }


                    Event e = Event.current;
                    if (position.Contains(e.mousePosition))
                    {
                        if (!Focused)
                            if ((e.type == EventType.MouseDown /*&& e.clickCount == 2*/) /*|| e.keyCode == KeyCode.F2*/)
                            {
                                Focused = true;
                                GUIFocusControl.Focus(this);
                                if (e.type != EventType.Repaint && e.type != EventType.Layout)
                                    Event.current.Use();
                            }
                    }
                    //if ((/*e.keyCode == KeyCode.Return ||*/ e.keyCode == KeyCode.Escape || e.character == '\n'))
                    //{
                    //    GUIFocusControl.Focus(null, Focused);
                    //    Focused = false;
                    //    if (e.type != EventType.Repaint && e.type != EventType.Layout)
                    //        Event.current.Use();
                    //}
                    return value;
                }

            }

            private const float ROW_HEIGHT = 16.0f;
            private const float ROW_INDENT = 8.0f;
            public static void Show(Rect activatorRect, string[] options, int current, Action<int,string> onSelectionMade)
            {
                SearchablePopup win =
                    new SearchablePopup(options, current, onSelectionMade);
                PopupWindow.Show(activatorRect, win);
            }
            private static void Repaint()
            { EditorWindow.focusedWindow.Repaint(); }
            private static void DrawBox(Rect rect, Color tint)
            {
                Color c = GUI.color;
                GUI.color = tint;
                GUI.Box(rect, "", Selection);
                GUI.color = c;
            }
            private class FilteredList
            {
                public struct Entry
                {
                    public int Index;
                    public string Text;
                }
                private readonly string[] allItems;
                public FilteredList(string[] items)
                {
                    allItems = items;
                    Entries = new List<Entry>();
                    UpdateFilter("");
                }

                public string Filter { get; private set; }

                public List<Entry> Entries { get; private set; }

                public int MaxLength
                { get { return allItems.Length; } }

                public bool UpdateFilter(string filter)
                {
                    if (Filter == filter)
                        return false;

                    Filter = filter;
                    Entries.Clear();

                    for (int i = 0; i < allItems.Length; i++)
                    {
                        if (string.IsNullOrEmpty(Filter) || allItems[i].ToLower().Contains(Filter.ToLower()))
                        {
                            Entry entry = new Entry
                            {
                                Index = i,
                                Text = allItems[i]
                            };
                            if (string.Equals(allItems[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                                Entries.Insert(0, entry);
                            else
                                Entries.Add(entry);
                        }
                    }
                    return true;
                }
            }

            private readonly Action<int,string> onSelectionMade;

            private readonly int currentIndex;

            private readonly FilteredList list;

            private Vector2 scroll;

            private int hoverIndex;


            private int scrollToIndex;

            private float scrollOffset;

            private static GUIStyle Selection = "SelectionRect";

            private SearchablePopup(string[] names, int currentIndex, Action<int,string> onSelectionMade)
            {
                list = new FilteredList(names);
                this.currentIndex = currentIndex;
                this.onSelectionMade = onSelectionMade;

                hoverIndex = currentIndex;
                scrollToIndex = currentIndex;
                scrollOffset = GetWindowSize().y - ROW_HEIGHT * 2;
            }

            public override void OnOpen()
            {
                base.OnOpen();
                EditorApplication.update += Repaint;
            }

            public override void OnClose()
            {
                base.OnClose();
                EditorApplication.update -= Repaint;
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(base.GetWindowSize().x,
                    Mathf.Min(600, list.MaxLength * ROW_HEIGHT +
                    EditorStyles.toolbar.fixedHeight));
            }

            public override void OnGUI(Rect rect)
            {
                Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
                Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

                HandleKeyboard();
                DrawSearch(searchRect);
                DrawSelectionArea(scrollRect);
            }
            private InnerSearchField searchField = new InnerSearchField();

            private void DrawSearch(Rect rect)
            {
                GUI.Label(rect, "", EditorStyles.toolbar);

                if (list.UpdateFilter(searchField.OnGUI(rect.Zoom(AnchorType.MiddleCenter, -2), list.Filter)))
                {
                    hoverIndex = 0;
                    scroll = Vector2.zero;
                }
            }

            private void DrawSelectionArea(Rect scrollRect)
            {
                Rect contentRect = new Rect(0, 0,
                    scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                    list.Entries.Count * ROW_HEIGHT);

                scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

                Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);

                for (int i = 0; i < list.Entries.Count; i++)
                {
                    if (scrollToIndex == i &&
                        (Event.current.type == EventType.Repaint
                         || Event.current.type == EventType.Layout))
                    {
                        Rect r = new Rect(rowRect);
                        r.y += scrollOffset;
                        GUI.ScrollTo(r);
                        scrollToIndex = -1;
                        scroll.x = 0;
                    }

                    if (rowRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseMove ||
                            Event.current.type == EventType.ScrollWheel)
                            hoverIndex = i;
                        if (Event.current.type == EventType.MouseDown)
                        {
                            onSelectionMade(list.Entries[i].Index,list.Entries[i].Text);
                            EditorWindow.focusedWindow.Close();
                        }
                    }

                    DrawRow(rowRect, i);

                    rowRect.y = rowRect.yMax;
                }

                GUI.EndScrollView();
            }

            private void DrawRow(Rect rowRect, int i)
            {
                if (list.Entries[i].Index == currentIndex)
                    DrawBox(rowRect, Color.cyan);
                else if (i == hoverIndex)
                    DrawBox(rowRect, Color.white);
                Rect labelRect = new Rect(rowRect);
                labelRect.xMin += ROW_INDENT;
                GUI.Label(labelRect, list.Entries[i].Text);
            }

            private void HandleKeyboard()
            {
                Event e = Event.current;
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.DownArrow)
                    {
                        hoverIndex = Mathf.Min(list.Entries.Count - 1, hoverIndex + 1);
                        Event.current.Use();
                        scrollToIndex = hoverIndex;
                        scrollOffset = ROW_HEIGHT;
                    }
                    if (e.keyCode == KeyCode.UpArrow)
                    {
                        hoverIndex = Mathf.Max(0, hoverIndex - 1);
                        Event.current.Use();
                        scrollToIndex = hoverIndex;
                        scrollOffset = -ROW_HEIGHT;
                    }
                    if (e.keyCode == KeyCode.Return || e.character == '\n')
                    {
                        if (hoverIndex >= 0 && hoverIndex < list.Entries.Count)
                        {
                            onSelectionMade(list.Entries[hoverIndex].Index, list.Entries[hoverIndex].Text);
                            EditorWindow.focusedWindow.Close();
                        }
                    }
                    if (Event.current.keyCode == KeyCode.Escape)
                    {
                        EditorWindow.focusedWindow.Close();
                    }
                }
            }
        }

    }
}
