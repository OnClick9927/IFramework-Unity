/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-19
 *Description:    IFramework    Source:https://answers.unity.com/questions/382973/programmatically-change-editor-layout.html
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace IFramework
{
	public static class EditorLayoutUtil
	{
        private static MethodInfo _miLoadWindowLayout;
        private static MethodInfo _miSaveWindowLayout;
        private static MethodInfo _miReloadWindowLayoutMenu;

        private static bool _available;

        static EditorLayoutUtil()
        {
            Type tyWindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
            Type tyEditorUtility = Type.GetType("UnityEditor.EditorUtility,UnityEditor");
            Type tyInternalEditorUtility = Type.GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor");

            if (tyWindowLayout != null && tyEditorUtility != null && tyInternalEditorUtility != null)
            {
                _miLoadWindowLayout = tyWindowLayout.GetMethod("LoadWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(bool) }, null);
                _miSaveWindowLayout = tyWindowLayout.GetMethod("SaveWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                _miReloadWindowLayoutMenu = tyInternalEditorUtility.GetMethod("ReloadWindowLayoutMenu", BindingFlags.Public | BindingFlags.Static);

                if (_miLoadWindowLayout == null || _miSaveWindowLayout == null || _miReloadWindowLayoutMenu == null)
                    return;

                _available = true;
            }
        }

        public static bool IsAvailable
        {
            get { return _available; }
        }

        public static void SaveLayoutToAsset(string assetPath)
        {
            SaveLayout(Path.Combine(Directory.GetCurrentDirectory(), assetPath));
            //EditorUtility.SetDirty( AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
            AssetDatabase.Refresh();
        }

        public static void LoadLayoutFromAsset(string assetPath)
        {
            if (_miLoadWindowLayout != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
                _miLoadWindowLayout.Invoke(null, new object[] { path, false });
            }
        }

        private static void SaveLayout(string path)
        {
            if (_miSaveWindowLayout != null)
                _miSaveWindowLayout.Invoke(null, new object[] { path });
        }
        //.wlt
    }


}
