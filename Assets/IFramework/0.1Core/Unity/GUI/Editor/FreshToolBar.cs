/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
	public class FreshToolBar:IRectGUIDrawer,ILayoutGUIDrawer
	{
        private static GUIContent refreshContent;
        private static GUIContent ReFreshContent {
            get {
                if (refreshContent==null)
                {
                    refreshContent = EditorGUIUtility.IconContent("d_TreeEditor.Refresh");
                }
                return refreshContent;
            }
        }

        public static int OnGUI(Action <bool> action, bool ShowRefresh,Vector2 freshSize, int selected, string[] texts)
        {
           /* Rect rect =*/ EditorGUILayout.BeginHorizontal(GUILayout.Height(freshSize.y));


            GUILayout.Space(10);
            if (ShowRefresh)
                if (GUILayout.Button(ReFreshContent,
                   GUILayout.Width(freshSize.x), GUILayout.Height(freshSize.y)))
                {
                    if (action != null)
                        action(true);
                }
            else
                GUILayout.Space(freshSize.x);
            selected = GUILayout.Toolbar(selected, texts, GUILayout.Height(freshSize.y));

            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
            return selected;
        }

        public static int OnGUI(Action<bool> action, bool ShowRefresh,Rect pos, int selected, string[] texts)
        {
            if (ShowRefresh)
                if (GUI.Button(new Rect(pos.x + 10, pos.y, pos.height, pos.height), ReFreshContent))
                {
                    if (action != null)
                        action(true);
                }

            selected= GUI.Toolbar(new Rect(
                pos.x+pos.height+15,
                pos.y,
                pos.width-pos.height-25,
                pos.height
                ), selected, texts);

            return selected;
        }

    }
}
