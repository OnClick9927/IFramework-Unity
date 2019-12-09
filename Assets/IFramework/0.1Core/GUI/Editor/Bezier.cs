/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
	public class Bezier
	{
        public static void DrawBezier(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = Color.white;
            for (int i = 0; i < 3; i++)
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 3);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 3);
        }
        public static void DrawBezier(Vector3 start, Rect end)
        {
            Vector3 startPos = start;
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = Color.white;
            for (int i = 0; i < 3; i++)
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 3);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 3);
        }
        public static void DrawBezier(Rect start, Vector3 end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = end;
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = Color.white;
            for (int i = 0; i < 3; i++)
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 3);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 3);
        }
        public static void DrawBezier(Vector3 start, Vector3 end)
        {
            Vector3 startPos = start;
            Vector3 endPos = end;
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = Color.white;
            for (int i = 0; i < 3; i++)
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 3);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 3);
        }
    }
}
