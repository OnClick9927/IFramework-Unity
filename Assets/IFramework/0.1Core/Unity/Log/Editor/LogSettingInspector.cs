/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using IFramework.GUITool;
namespace IFramework
{
    [CustomEditor(typeof(LogSetting))]
    class LogSettingInspector : Editor,ILayoutGUIDrawer
	{
        private LogSetting info;
        private void OnEnable()
        {
            if (info == null)
            {
                info = target as LogSetting;
            }
        }
        public override void OnInspectorGUI()
        {
            if (info == null)
            {
                info = target as LogSetting;
            }
            this.serializedObject.Update();
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LogLevel"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("WarnningLevel"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("ErrLevel"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Enable"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LogEnable"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("WarnningEnable"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("ErrEnable"));
            }
            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < info.Infos.Count; i++)
                {
                    LogEliminateItem item = info.Infos[i];
                    EditorGUILayout.ObjectField(item.Text, typeof(TextAsset), false);
                }
            }
        }

    }
}
