/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-01-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
namespace IFramework
{
    public class CreateAssetAction : EndNameEditAction
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