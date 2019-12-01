using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using IFramework;
using IFramework.GUITool;

public class NodeEditor : EditorWindow
{
    public static float zoomDelta = 0.01f;
    public static float minZoom = 1f;
    public static float maxZoom = 8f;
    public static float panSpeed = 1.2f;
    private Vector2 _zoomAdjustment;
    private Vector2 _zoom = Vector2.one;
    public Vector2 panOffset = Vector2.zero;


    public void Pan(Vector2 delta)
    {
        panOffset += delta * ZoomScale * panSpeed;
    }
    public void Zoom(float zoomDirection)
    {
        float scale = (zoomDirection < 0f) ? (1f - zoomDelta) : (1f + zoomDelta);
        _zoom *= scale;
        float cap = Mathf.Clamp(_zoom.x, minZoom, maxZoom);
        _zoom.Set(cap, cap);
    }


    public float ZoomScale
    {
        get { return _zoom.x; }
        set
        {
            float z = Mathf.Clamp(value, minZoom, maxZoom);
            _zoom.Set(z, z);
        }
    }


    public Rect Size
    {
        get { return new Rect(Vector2.zero, position.size); }
    }
    [MenuItem("Tool/NodeEditor  #")]
    static void Init()
    {
        GetWindow<NodeEditor>();

    }
    private void OnEnable()
    {
        GUIScaleUtility.CheckInit();

    }

    List<Rect> rs = new List<Rect>();
    private void OnGUI()
    {
        Rect graphRect = Size;
        var center = graphRect.size / 2f;
        EditorGUILayout.RectField("Rect", graphRect);


        _zoomAdjustment = GUIScaleUtility.BeginScale(ref graphRect, center, ZoomScale, false);
        DrawGrid(graphRect, 100 / ZoomScale, 10, panOffset/ZoomScale, Color.gray);

        EditorGUILayout.RectField("Rect", graphRect);

        drawAxes();
        
        GUI.Box(new Rect(GraphToScreenSpace(new Vector2(0,0)), new Vector2(100,100)), "");
        GUI.Box(new Rect(Event.current.mousePosition, new Vector2(100, 100)), "");
        if (Event.current.clickCount == 2)
        {
            Rect r = new Rect(Event.current.mousePosition-rect.center, new Vector2(100, 100));
            rs.Add(r);
        }
        for (int i = 0; i < rs.Count; i++)
        {
            Rect r = rs[i];
            r.position = GraphToScreenSpace(r.position);
            GUI.Box(r, "");
        }
        GUIScaleUtility.EndScale();
        if (Event.current.type == EventType.ScrollWheel)
        {
            Zoom(Event.current.delta.y);
            Repaint();
        }
        if (Event.current.type == EventType.MouseDrag)
        {
            Pan(Event.current.delta);
            Repaint();
        }
        EditorGUILayout.BeginHorizontal("Toolbar");
        if (DropButton("ReCenter", kToolbarButtonWidth + 10f))
        {
            panOffset = Vector2.zero;
        }
        EditorGUILayout.EndHorizontal();
    }
    private void DrawGrid(Rect pos, float space, float width, Vector2 offset, Color color)
    {
        Handles.BeginGUI();
        Handles.color = color;
        offset = new Vector2(offset.x % space, offset.y % space);
        float temp = rect.center.y;
        while (temp < pos.height + space)
        {
            Handles.DrawAAPolyLine(width, new Vector3(-space,
                                       temp + offset.y, 0),
                                new Vector3(pos.width,
                                temp + offset.y, 0f));
            temp += space;
        }
        temp = rect.center.y - space;
        while (temp > -space)
        {
            Handles.DrawAAPolyLine(width, new Vector3(-space,
                                       temp + offset.y, 0),
                                new Vector3(pos.width,
                                temp + offset.y, 0f));
            temp -= space;
        }

        temp = rect.center.x  ;

        while (temp < pos.width + space)
        {
            Handles.DrawAAPolyLine(width, new Vector3(temp + offset.x, -space, 0), new Vector3(temp + offset.x, pos.height, 0f));
            temp += space;
        }
        temp = rect.center.x - space;
        while (temp > - space)
        {
            Handles.DrawAAPolyLine(width, new Vector3(temp + offset.x, -space, 0), new Vector3(temp + offset.x, pos.height, 0f));
            temp -= space;
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    public const float kToolbarButtonWidth = 50f;


    public bool DropButton(string name, float width)
    {


        return GUILayout.Button(name, EditorStyles.miniButton, GUILayout.Width(width));
    }







    public Rect GraphToScreenSpace(Rect rect)
    {
        Rect r = rect;
        r.position = GraphToScreenSpace(r.position);
        return r;
    }


    public Vector2 GraphToScreenSpace(Vector2 graphPos)
    {
        return graphPos + _zoomAdjustment + panOffset;
    }
    Rect rect = new Rect();
    private void drawAxes()
    {
        Vector2 down = Vector2.up * Size.height * ZoomScale;
        Vector2 right = Vector2.right * Size.width * ZoomScale;
        Vector2 up = -down;
        Vector2 left = -right;


        //up.y -= panOffset.y;
        //down.y -= panOffset.y;
        //right.x -= panOffset.x;
        //left.x -= panOffset.x;

        up = GraphToScreenSpace(up);
        down = GraphToScreenSpace(down);
        right = GraphToScreenSpace(right);
        left = GraphToScreenSpace(left);
        rect.Set(left.x, up.y, right.x - left.x, down.y - up.y);
        DrawLine(right, left, Color.white);
        DrawLine(up, down, Color.white);

        EditorGUILayout.Vector2Field("left", left);
        EditorGUILayout.Vector2Field("right", right);
        EditorGUILayout.Vector2Field("up", up);
        EditorGUILayout.Vector2Field("down", down);

        EditorGUILayout.Vector2Field("mousePosition", Event.current.mousePosition);

        Repaint();
        //GUI.Box(new Rect(300, 300, 100, 100), "");
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        var handleColor = Handles.color;
        Handles.color = color;
        Handles.DrawLine(start, end);
        Handles.color = handleColor;
    }










}





