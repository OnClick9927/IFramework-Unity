/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IFramework.UI
{
    [CustomEditor(typeof(Empty4Raycast))]
    class Empty4RaycastEditor : Editor
    {
        class SelectionInfo
        {
            public int pointIndex = -1;
            public bool mouseIsOverPoint;
            public bool pointIsSelected;
            public Vector3 positionAtStartOfDrag;

            public int lineIndex = -1;
            public bool mouseIsOverLine;
        }

        private float pointRadius => 0.8f * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);
        private bool editting = false;
        private bool needsRepaint = false;
        private Vector2 pivot;
        private Vector3[] corners = new Vector3[4];

        private Empty4Raycast obj;
        private RectTransform objTrans;
        private Canvas rootCanvas = null;
        private SelectionInfo selectionInfo;
        static Vector2 InverseTransformPoint(RectTransform rectTrans, Vector3[] corners, Vector3 mousePos)
        {
            var localPos = rectTrans.InverseTransformPoint(mousePos);
            rectTrans.GetLocalCorners(corners);

            localPos.x = localPos.x < corners[0].x ? corners[0].x : localPos.x;
            localPos.x = localPos.x > corners[2].x ? corners[2].x : localPos.x;
            localPos.y = localPos.y < corners[0].y ? corners[0].y : localPos.y;
            localPos.y = localPos.y > corners[2].y ? corners[2].y : localPos.y;

            return localPos;
        }

        static void OnPivotChangedAdjust(RectTransform rectTrans, Vector2 originPivot, List<Vector2> points)
        {
            var deltaPivot = rectTrans.pivot - originPivot;
            var size = rectTrans.sizeDelta;
            Vector2 deltaDrift = new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);

            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Vector2(points[i].x - deltaDrift.x, points[i].y - deltaDrift.y);
            }
        }


        private void OnEnable()
        {
            obj = (Empty4Raycast)target;
            objTrans = obj.GetComponent<RectTransform>();
            UpdatePivot();

            var canvas = obj.GetComponentInParent<Canvas>();
            rootCanvas = canvas == null ? null : canvas.rootCanvas;
            selectionInfo = new SelectionInfo();

            Undo.undoRedoPerformed -= UpdatePivot;
            Undo.undoRedoPerformed += UpdatePivot;
        }
        private void OnDisable()
        {
            editting = false;
            UpdateHandles();

            Undo.undoRedoPerformed -= UpdatePivot;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Edit Shape", GUILayout.Width(100));
                bool tempBool = GUILayout.Toggle(editting, EditorGUIUtility.TrIconContent("d_EditCollider"), "Button", GUILayout.Width(30));
                if (tempBool != editting)
                {
                    SceneView.RepaintAll();
                    editting = tempBool;
                    needsRepaint = true;

                    UpdateHandles();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Adjust Shape", GUILayout.Width(100));

                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    Undo.RecordObject(obj, "Adjust Pivot");
                    OnPivotChangedAdjust(objTrans, pivot, obj.Points);
                    UpdatePivot();

                    needsRepaint = true;
                    SceneView.RepaintAll();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RaycastTarget"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("points"));

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {

            Event guiEvent = Event.current;

            switch (guiEvent.type)
            {
                case EventType.Repaint when editting:
                    DrawWithDisc();
                    break;

                case EventType.Repaint:
                    Draw();
                    break;

                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    break;

                default:
                    HandleInput(guiEvent);
                    if (needsRepaint) HandleUtility.Repaint();
                    break;
            }
        }

        private void UpdatePivot()
        {
            pivot = objTrans.pivot;
        }

        private void UpdateHandles()
        {
            Tools.hidden = editting;
            if (objTrans)
                objTrans.hideFlags = editting ? HideFlags.NotEditable : HideFlags.None;
        }

        private void HandleInput(Event guiEvent)
        {
            if (!editting) return;

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            Plane plane = new Plane(objTrans.forward, objTrans.position);
            plane.Raycast(mouseRay, out var enter);
            Vector3 worldPos = mouseRay.GetPoint(enter);

            if (guiEvent.modifiers == EventModifiers.None && guiEvent.button == 0)
            {
                switch (guiEvent.type)
                {
                    case EventType.MouseDown:
                        HandleLeftMouseDown(worldPos);
                        break;

                    case EventType.MouseUp:
                        HandleLeftMouseUp(worldPos);
                        break;

                    case EventType.MouseDrag:
                        HandleLeftMouseDrag(worldPos);
                        break;
                }
            }
            else if (guiEvent.modifiers == EventModifiers.Control && guiEvent.button == 0)
            {
                if (guiEvent.type == EventType.MouseUp)
                {
                    HandleControlAndLeftMouseUp();
                }
            }

            if (!selectionInfo.pointIsSelected)
            {
                UpdateMouseOverInfo(worldPos);
            }
        }

        private void HandleLeftMouseDown(Vector3 mousePosition)
        {
            if (!selectionInfo.mouseIsOverPoint)
            {
                int newPointIndex = selectionInfo.mouseIsOverLine ? selectionInfo.lineIndex + 1 : obj.Points.Count;
                Undo.RecordObject(obj, "Add PolygonImage Point");
                selectionInfo.pointIndex = newPointIndex;
                selectionInfo.mouseIsOverPoint = true;
                selectionInfo.lineIndex = -1;

                if (newPointIndex == obj.Points.Count)
                {
                    obj.Points.Add(InverseTransformPoint(objTrans, corners, mousePosition));
                }
                else
                {
                    obj.Points.Insert(newPointIndex, InverseTransformPoint(objTrans, corners, mousePosition));
                }
            }

            selectionInfo.pointIsSelected = true;
            selectionInfo.positionAtStartOfDrag = mousePosition;
            needsRepaint = true;
        }

        private void HandleLeftMouseUp(Vector3 mousePosition)
        {
            if (selectionInfo.pointIsSelected)
            {
                obj.Points[selectionInfo.pointIndex] = InverseTransformPoint(objTrans, corners, selectionInfo.positionAtStartOfDrag);
                Undo.RecordObject(obj, "Move Point");
                obj.Points[selectionInfo.pointIndex] = InverseTransformPoint(objTrans, corners, mousePosition);

                selectionInfo.pointIsSelected = false;
                selectionInfo.mouseIsOverPoint = false;
                selectionInfo.pointIndex = -1;
                needsRepaint = true;
            }
        }

        private void HandleLeftMouseDrag(Vector3 mousePosition)
        {
            if (selectionInfo.pointIsSelected)
            {
                obj.Points[selectionInfo.pointIndex] = InverseTransformPoint(objTrans, corners, mousePosition);
                needsRepaint = true;
            }
        }

        private void HandleControlAndLeftMouseUp()
        {
            if (!selectionInfo.mouseIsOverPoint) return;

            Undo.RecordObject(obj, "Remove Point");
            obj.Points.RemoveAt(selectionInfo.pointIndex);
        }

        private void UpdateMouseOverInfo(Vector3 mousePosition)
        {
            int mouseOverPointIndex = -1;
            //pointRadius = _pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);

            for (int i = 0; i < obj.Points.Count; i++)
            {
                if (Vector3.Distance(mousePosition, objTrans.TransformPoint(obj.Points[i])) < pointRadius)
                {
                    mouseOverPointIndex = i;
                    break;
                }
            }

            if (mouseOverPointIndex != selectionInfo.pointIndex)
            {
                selectionInfo.pointIndex = mouseOverPointIndex;
                selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

                needsRepaint = true;
            }

            if (selectionInfo.mouseIsOverPoint)
            {
                selectionInfo.mouseIsOverLine = false;
                selectionInfo.lineIndex = -1;
            }
            else
            {
                var count = obj.Points.Count;
                int mouseOverLineIndex = -1;
                float closestLineDst = pointRadius;

                for (int i = 0; i < count; i++)
                {
                    var currentPoint = objTrans.TransformPoint(obj.Points[i]);
                    var nextPoint = objTrans.TransformPoint(obj.Points[(i + 1) % count]);
                    float distFromMouseToLine = HandleUtility.DistancePointLine(mousePosition, currentPoint, nextPoint);

                    if (distFromMouseToLine < closestLineDst)
                    {
                        closestLineDst = distFromMouseToLine;
                        mouseOverLineIndex = i;
                    }
                }

                if (selectionInfo.lineIndex != mouseOverLineIndex)
                {
                    selectionInfo.lineIndex = mouseOverLineIndex;
                    selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                    needsRepaint = true;
                }
            }
        }

        private void DrawWithDisc()
        {
            //pointRadius = _pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);
            var count = obj.Points.Count;

            for (int i = 0; i < count; i++)
            {
                Vector3 start = objTrans.TransformPoint(obj.Points[i]);
                Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % count]);

                Handles.color = i == selectionInfo.lineIndex ? Color.green : Color.yellow;
                if (i == selectionInfo.lineIndex)
                {
                    Handles.DrawLine(start, end);
                }
                else
                {
                    Handles.DrawDottedLine(start, end, 4);
                }
            }

            for (int i = 0; i < count; i++)
            {
                Handles.color = i != selectionInfo.pointIndex ? Color.yellow : (selectionInfo.pointIsSelected ? Color.cyan : Color.green);
                Handles.DrawSolidDisc(objTrans.TransformPoint(obj.Points[i]), objTrans.forward, pointRadius);
            }

            needsRepaint = false;
        }

        private void Draw()
        {
            Handles.color = Color.yellow;
            var count = obj.Points.Count;

            for (int i = 0; i < obj.Points.Count; i++)
            {
                Vector3 start = objTrans.TransformPoint(obj.Points[i]);
                Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % count]);
                Handles.DrawLine(start, end);
            }

            needsRepaint = false;
        }
    }

}
