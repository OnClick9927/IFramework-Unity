/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;
namespace IFramework
{
    public class CopyAsset : UnityEditor.Editor
    {
        private static string GetSelectedPath()
        {
            string selectedPath = "Assets";
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            foreach (Object obj in selection)
            {
                selectedPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(selectedPath) && File.Exists(selectedPath))
                {
                    selectedPath = Path.GetDirectoryName(selectedPath);
                    break;
                }
            }
            return selectedPath;
        }
        public static void CopyNewAsset(string newFileName, string sourcePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
           ScriptableObject.CreateInstance<CreateAssetAction>(),
           /*Path.Combine(GetSelectedPath(), newFileName)*/ newFileName, null, sourcePath);
        }
       
    }        
}