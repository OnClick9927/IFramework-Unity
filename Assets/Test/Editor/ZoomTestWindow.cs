using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;




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

public static class GUIScaleUtility
{
    // General
    private static bool compabilityMode;
    private static bool initiated;
    // Delegates to the reflected methods
    private static Func<Rect> GetTopRectDelegate;
    //private static Func<Rect> topmostRectDelegate;
    // Delegate accessors
    public static Rect getTopRect { get { return (Rect)GetTopRectDelegate.Invoke(); } }
    // Rect stack for manipulating groups
    public static List<Rect> currentRectStack { get; private set; }
    private static List<List<Rect>> rectStackGroups;
    // Matrices stack
    private static List<Matrix4x4> GUIMatrices;
    private static List<bool> adjustedGUILayout;
    public static void CheckInit()
    {
        if (!initiated)
            Init();
    }
    public static void Init()
    {
        // Fetch rect acessors using Reflection
        Assembly UnityEngine = Assembly.GetAssembly(typeof(UnityEngine.GUI));
        Type GUIClipType = UnityEngine.GetType("UnityEngine.GUIClip", true);
        PropertyInfo topmostRect = GUIClipType.GetProperty("topmostRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo GetTopRect = GUIClipType.GetMethod("GetTopRect", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo ClipRect = GUIClipType.GetMethod("Clip", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, new Type[] { typeof(Rect) }, new ParameterModifier[] { });
        if (GUIClipType == null || topmostRect == null || GetTopRect == null || ClipRect == null)
        {
            Debug.LogWarning("GUIScaleUtility cannot run on this system! Compability mode enabled. For you that means you're not able to use the Node Editor inside more than one group:( Please PM me (Seneral @UnityForums) so I can figure out what causes this! Thanks!");
            Debug.LogWarning((GUIClipType == null ? "GUIClipType is Null, " : "") + (topmostRect == null ? "topmostRect is Null, " : "") + (GetTopRect == null ? "GetTopRect is Null, " : "") + (ClipRect == null ? "ClipRect is Null, " : ""));
            compabilityMode = true;
            initiated = true;
            return;
        }
        // Create simple acessor delegates
        GetTopRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), GetTopRect);
        //topmostRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), topmostRect.GetGetMethod());
        if (GetTopRectDelegate == null /*|| topmostRectDelegate == null*/)
        {
            Debug.LogWarning("GUIScaleUtility cannot run on this system! Compability mode enabled. For you that means you're not able to use the Node Editor inside more than one group:( Please PM me (Seneral @UnityForums) so I can figure out what causes this! Thanks!");
            Debug.LogWarning((GUIClipType == null ? "GUIClipType is Null, " : "") + (topmostRect == null ? "topmostRect is Null, " : "") + (GetTopRect == null ? "GetTopRect is Null, " : "") + (ClipRect == null ? "ClipRect is Null, " : ""));
            compabilityMode = true;
            initiated = true;
            return;
        }
        // As we can call Begin/Ends inside another, we need to save their states hierarchial in Lists (not Stack, as we need to iterate over them!):
        currentRectStack = new List<Rect>();
        rectStackGroups = new List<List<Rect>>();
        GUIMatrices = new List<Matrix4x4>();
        adjustedGUILayout = new List<bool>();
        initiated = true;
    }



   
    /// <summary>
    /// Begins a scaled local area. 
    /// Returns vector to offset GUI controls with to account for zooming to the pivot. 
    /// Using adjustGUILayout does that automatically for GUILayout rects. Theoretically can be nested!
    /// </summary>
    public static Vector2 BeginScale(ref Rect rect, Vector2 zoomPivot, float zoom, bool adjustGUILayout)
    {
        Rect screenRect;
        if (compabilityMode)
        {
            GUI.EndGroup();
            screenRect = rect;
        }
        else
        {
            // If it's supported, we take the completely generic way using reflected calls
            GUIScaleUtility.BeginNoClip();
            screenRect = GUIScaleUtility.GUIToScaledSpace(rect);
        }
        rect = Scale(screenRect, screenRect.position + zoomPivot, new Vector2(zoom, zoom));
        // Now continue drawing using the new clipping group
        GUI.BeginGroup(rect);
        rect.position = Vector2.zero; // Adjust because we entered the new group
        // Because I currently found no way to actually scale to a custom pivot rather than (0, 0),
        // we'll make use of a cheat and just offset it accordingly to let it appear as if it would scroll to the center
        // Note, due to that, controls not adjusted are still scaled to (0, 0)
        Vector2 zoomPosAdjust = rect.center - screenRect.size / 2 + zoomPivot;
        // For GUILayout, we can make this adjustment here if desired
        adjustedGUILayout.Add(adjustGUILayout);
        if (adjustGUILayout)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(rect.center.x - screenRect.size.x + zoomPivot.x);
            GUILayout.BeginVertical();
            GUILayout.Space(rect.center.y - screenRect.size.y + zoomPivot.y);
        }
        // Take a matrix backup to restore back later on
        GUIMatrices.Add(GUI.matrix);
        // Scale GUI.matrix. After that we have the correct clipping group again.
        GUIUtility.ScaleAroundPivot(new Vector2(1 / zoom, 1 / zoom), zoomPosAdjust);
        return zoomPosAdjust;
    }
    public static void EndScale()
    {
        // Set last matrix and clipping group
        if (GUIMatrices.Count == 0 || adjustedGUILayout.Count == 0)
            throw new UnityException("GUIScaleUtility: You are ending more scale regions than you are beginning!");


        GUI.matrix = GUIMatrices[GUIMatrices.Count - 1];
        GUIMatrices.RemoveAt(GUIMatrices.Count - 1);


        // End GUILayout zoomPosAdjustment
        if (adjustedGUILayout[adjustedGUILayout.Count - 1])
        {


            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        adjustedGUILayout.RemoveAt(adjustedGUILayout.Count - 1);


        // End the scaled group
        GUI.EndGroup();


        if (compabilityMode)
        {
            // In compability mode, we don't know the previous group rect, but as we cannot use top groups there either way, we restore the screen group
            GUI.BeginClip(new Rect(0, 23, Screen.width, Screen.height - 23));
        }
        else
        {
            // Else, restore the clips (groups)
            GUIScaleUtility.RestoreClips();
        }
    }
   


    /// <summary>
    /// Begins a field without groups. They should be restored using RestoreClips. Can be nested!
    /// </summary>
    public static void BeginNoClip()
    {
        // Record and close all clips one by one, from bottom to top, until we hit the 'origin'
        List<Rect> rectStackGroup = new List<Rect>();
        Rect topMostClip = getTopRect;
        while (topMostClip != new Rect(-10000, -10000, 40000, 40000))
        {
            rectStackGroup.Add(topMostClip);
            GUI.EndClip();
            topMostClip = getTopRect;
        }
        // Store the clips appropriately
        rectStackGroup.Reverse();
        rectStackGroups.Add(rectStackGroup);
        currentRectStack.AddRange(rectStackGroup);
    }


    /// <summary>
    /// Restores the clips removed in BeginNoClip or MoveClipsUp
    /// </summary>
    public static void RestoreClips()
    {
        if (rectStackGroups.Count == 0)
        {
            Debug.LogError("GUIClipHierarchy: BeginNoClip/MoveClipsUp - RestoreClips count not balanced!");
            return;
        }
        // Read and restore clips one by one, from top to bottom
        List<Rect> rectStackGroup = rectStackGroups[rectStackGroups.Count - 1];
        for (int clipCnt = 0; clipCnt < rectStackGroup.Count; clipCnt++)
        {
            GUI.BeginClip(rectStackGroup[clipCnt]);
            currentRectStack.RemoveAt(currentRectStack.Count - 1);
        }
        rectStackGroups.RemoveAt(rectStackGroups.Count - 1);
    }




    #region Space Transformations


    /// <summary>
    /// Scales the rect around the pivot with scale
    /// </summary>
    public static Rect Scale(Rect rect, Vector2 pivot, Vector2 scale)
    {
        rect.position = Vector2.Scale(rect.position - pivot, scale) + pivot;
        rect.size = Vector2.Scale(rect.size, scale);
        return rect;
    }
    public static Vector2 GUIToScaledSpace(Vector2 guiPosition)
    {
        if (rectStackGroups == null || rectStackGroups.Count == 0)
            return guiPosition;
        // Iterate through the clips and add positions ontop
        List<Rect> rectStackGroup = rectStackGroups[rectStackGroups.Count - 1];
        for (int clipCnt = 0; clipCnt < rectStackGroup.Count; clipCnt++)
            guiPosition += rectStackGroup[clipCnt].position;
        return guiPosition;
    }
    /// <summary>
    /// Transforms the rect to the new space aquired with BeginNoClip or MoveClipsUp.
    /// DOES NOT scale the rect, only offsets it!
    /// It's way faster to call GUIToScreenSpace before modifying the space though!
    /// </summary>
    public static Rect GUIToScaledSpace(Rect guiRect)
    {
        if (rectStackGroups == null || rectStackGroups.Count == 0)
            return guiRect;
        guiRect.position = GUIToScaledSpace(guiRect.position);
        return guiRect;
    }


    #endregion
}




