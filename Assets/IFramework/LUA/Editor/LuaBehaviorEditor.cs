/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace IFramework
{
    [CustomEditor(typeof(LuaBehavior))]
	public class LuaBehaviorEditor:Editor
	{
        private LuaBehavior self { get { return target as LuaBehavior; } }
        private ReorderableList list;
        private void OnEnable()
        {
            var p = this.serializedObject.FindProperty("injections");
            list = ReorderableListUtil.Create(p,
                new List<ReorderableListUtil.Column>()
                {
                    new ReorderableListUtil.Column() {DisplayName="Name" ,Width=60},
                    new ReorderableListUtil.Column() {DisplayName="GameObject" },
                }
                , 10);
        }
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
          //  base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Lua Script",GUILayout.Width(100));
            self.luaScript= EditorGUILayout.ObjectField(self.luaScript, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.EndHorizontal();

            ReorderableListUtil.Draw(list);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
