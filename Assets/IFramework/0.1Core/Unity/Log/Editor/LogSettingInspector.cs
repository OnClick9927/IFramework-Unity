/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
namespace IFramework
{
    [CustomEditor(typeof(LogSetting))]
	internal class LogSettingInspector : Editor
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
            GUI.enabled = false;
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LogLevel"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("WarnningLevel"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("ErrLevel"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Enable"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LogEnable"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("WarnningEnable"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("ErrEnable"));

            GUI.enabled = true;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Ignore List Count: " + info.Infos.Count);
            for (int i = 0; i < info.Infos.Count; i++)
            {
                LogEliminateItem item = info.Infos[i];
                EditorGUILayout.ObjectField(item.Text, typeof(TextAsset), false);
            }
            EditorGUI.EndDisabledGroup();
        }

    }
}
