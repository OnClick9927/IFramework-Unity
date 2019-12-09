/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.ProjectWindowCallback;

namespace IFramework
{

    public class CopyAsset : Editor
    {
        public static void CopyNewAsset(string newFileName, string sourcePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            CreateInstance<CreateAssetAction>(),
           /*Path.Combine(GetSelectedPath(), newFileName)*/ newFileName, null, sourcePath);
        }
        class CreateAssetAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object obj = CreateAssetFormTemplate(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(obj);
            }

            internal static Object CreateAssetFormTemplate(string pathName, string resourceFile)
            {
                string fullName = Path.GetFullPath(pathName);
                StreamReader reader = new StreamReader(resourceFile);
                string content = reader.ReadToEnd();
                reader.Close();
                string fileName = Path.GetFileNameWithoutExtension(pathName);
                content = content.Replace("#NAME", fileName);
                StreamWriter writer = new StreamWriter(fullName, false, System.Text.Encoding.UTF8);
                writer.Write(content);
                writer.Close();
                AssetDatabase.ImportAsset(pathName);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            }
        }

    }
}