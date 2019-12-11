/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    public static class RectExtension
    {
        public static void DrawOutLine(this Rect rect, float width, Color color)
        {
            Handles.color = color;

            Handles.DrawAAPolyLine(2, new Vector3(rect.x,
                                         rect.y,
                                         0),
                          new Vector3(rect.x,
                                         rect.yMax,
                                         0),
                          new Vector3(rect.xMax,
                                         rect.yMax,
                                         0),
                          new Vector3(rect.xMax,
                                         rect.y,
                                         0),
                          new Vector3(rect.x,
                                         rect.y,
                                         0)
                            );

            Handles.color = Color.white;
        }
    }
}
