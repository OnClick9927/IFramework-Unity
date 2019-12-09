using System.Diagnostics;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using IFramework;

public class Foo : EditorWindow
{
    static IFPreview IFPreview;
    float sRate = 0.5f;
    private Vector2 _drag;

    [MenuItem("Tool/Foo")]
    static void Setup()
    {

        GetWindow<Foo>();
    }
    private void OnEnable()
    {

        string prePath = @"Assets\Test\Editor/Shaderball2.prefab";

        string localPath = @"Assets\Test\Editor/Shaderball2.FBX";
        if (!System.IO.File.Exists(prePath))
        {
            EditorUtil.CreatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(localPath), prePath);
        }
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prePath);
        IFPreview = new IFPreview(prefab, prefab.GetComponentsInChildren<MeshFilter>(true), prefab.GetComponentsInChildren<Renderer>(true));
        IFPreview.Preview.camera.farClipPlane = 500;
        IFPreview.Preview.camera.clearFlags = CameraClearFlags.Skybox;
        IFPreview.Preview.camera.transform.position = new Vector3(0, 0.2f, -10);
    }

    private void OnDisable()
    {
        if (IFPreview != null)
            IFPreview.Cleanup();
    }
    void OnGUI()
    {
        if (Event.current.isScrollWheel)
        {
            sRate -= Event.current.delta.y / 30.0f;
            sRate = Mathf.Clamp(sRate, 0.2f, 1.2f);
        }
        _drag = Drag2D(_drag, new Rect(0, 0, 500, 500));
        Quaternion rt = Quaternion.Euler(new Vector3(0, _drag.x, _drag.y));
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rt, Vector3.one * sRate);
        Texture tx = IFPreview.Render(new Rect(0, 0, 500, 500), m);
        GUI.Box(IFPreview.rect, tx);
        this.Repaint();

    }

    public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
    {
        int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
        Event current = Event.current;
        switch (current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition) && position.width > 50f)
                {
                    GUIUtility.hotControl = controlID;
                    current.Use();
                    EditorGUIUtility.SetWantsMouseJumping(1);
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;
                }
                EditorGUIUtility.SetWantsMouseJumping(0);
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
                    scrollPosition.y = Mathf.Clamp(scrollPosition.y, -180f, 180f);
                    current.Use();
                    GUI.changed = true;
                }
                break;
        }
        return scrollPosition;
    }
}


