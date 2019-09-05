/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
     class HierarchyOverride
	 {
        private const string menuItemPath = "IFramework/Tool/HierarchyExtension";

        [InitializeOnLoadMethod]
        private static void Check()   
        {
            active = EditorPrefs.GetBool("Hierarchy Extension");             
            if (active)
                EditorApplication.hierarchyWindowItemOnGUI += HierarchWindowOnGui;
            else
                EditorApplication.hierarchyWindowItemOnGUI -= HierarchWindowOnGui;
            //Menu.SetChecked(menuItemPath, active); 
            EditorApplication.RepaintHierarchyWindow();

        }
        private static bool active;
        [MenuItem(menuItemPath)]
        private static void Pan()
        {
            //bool flag = Menu.GetChecked(menuItemPath); 
            //active = !flag;
            active = !active;
            if (active)
                EditorApplication.hierarchyWindowItemOnGUI += HierarchWindowOnGui;
            else
                EditorApplication.hierarchyWindowItemOnGUI -= HierarchWindowOnGui;
            Menu.SetChecked(menuItemPath, active);
            EditorPrefs.SetBool("Hierarchy Extension", active);
            EditorApplication.RepaintHierarchyWindow(); 
        }

        private static void HierarchWindowOnGui(int instanceID, Rect selectionRect)
        {
            if (!active) return;
            //selectionRect.DrawOutLine(10, Color.grey);
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj == null) return;
            var go = (GameObject)obj;// as GameObject;
            GUILayout.BeginArea(selectionRect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    DrawRectIcon<MeshRenderer>(go);
                    DrawRectIcon<SkinnedMeshRenderer>(go);
                    DrawRectIcon<BoxCollider>(go);
                    DrawRectIcon<SphereCollider>(go);
                    DrawRectIcon<CapsuleCollider>(go);
                    DrawRectIcon<MeshCollider>(go);
                    DrawRectIcon<CharacterController>(go);
                    DrawRectIcon<Rigidbody>(go);
                    DrawRectIcon<Light>(go);
                    DrawRectIcon<Animator>(go);
                    DrawRectIcon<Animation>(go);
                    DrawRectIcon<Camera>(go);
                    DrawRectIcon<Projector>(go);
                    DrawRectIcon<ParticleSystem>(go);
                    DrawRectIcon<AudioSource>(go);
                    go.SetActive(EditorGUILayout.Toggle(go.activeSelf, GUIUtil.Width(18)));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();



        }
        private static void DrawRectIcon<T>( GameObject go) where T : Component
        {
            if (go.GetComponent<T>() == null) return;
            var icon = EditorGUIUtility.ObjectContent(null, typeof(T)).image;
            GUILayout.Label(icon, GUIUtil.Width(18), GUIUtil.Height(18));
        }
    }
}
