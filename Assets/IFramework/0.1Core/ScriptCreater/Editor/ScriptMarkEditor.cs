/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CanEditMultipleObjects,CustomEditor(typeof(ScriptMark))]
    public class ScriptMarkEditor : Editor
    {
        public ScriptMark SM { get { return this.target as ScriptMark; } }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            Component[] cps = SM.GetComponents<Component>();
            if (cps == null || cps.Length <= 0) return;
            List<string> names = new List<string>();
            cps.ToList().ForEach((c) =>
            {
                if (c != null)
                    names.Add(c.GetType().Name);
                //else
                //    DestroyImmediate(c);
            });
            names = names.Distinct().ToList();
            SM.isPublic= EditorGUILayout.Toggle("IsPublic", SM.isPublic);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type");
            SM.SelectTypeIndex = EditorGUILayout.Popup(SM.SelectTypeIndex, names.ToArray());
            SM.fieldType = names[SM.SelectTypeIndex];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            SM.fieldName = EditorGUILayout.TextField(SM.fieldName);
            SM.fieldName = string.IsNullOrEmpty(SM.fieldName) ? SM.name : SM.fieldName;
            if (!SM.fieldName.IsLegalFieldName()) SM.fieldName = SM.name;
            SM.fieldName = SM.fieldName.Replace(" ", "").Replace("(", "").Replace(")", "");

            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Description");
            SM.description = EditorGUILayout.TextArea(SM.description, GUILayout.Height(40));
            serializedObject.Update();
        }
    }
}
