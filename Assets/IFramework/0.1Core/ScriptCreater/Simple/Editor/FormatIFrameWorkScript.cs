/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEditor;
using System.IO;
using System;
using System.Text;
using UnityEngine;

namespace IFramework
{
    class  FormatIFrameWorkScript
    {
        const string key = "FormatIFrameWorkScript";

        private class FormatIFrameWorkScriptProcessor : UnityEditor.AssetModificationProcessor
        {
            public static void OnWillCreateAsset(string metaPath)
            {
                if (!EditorPrefs.GetBool(key, false)) return;                   
                string filePath = metaPath.Replace(".meta", "");
                if (!filePath.EndsWith(".cs")) return;
                string realPath = filePath.ToAbsPath();
                string txt = File.ReadAllText(realPath);
                if (!txt.Contains("#FAuthor#")) return;
                txt = txt.Replace("#FAuthor#", FrameworkConfig.Author)
                         .Replace("#FNameSpace#", FrameworkConfig.FrameworkName)
                         .Replace("#FDescription#", FrameworkConfig.Description)
                         .Replace("#FSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                         .Replace("#FVERSION#", FrameworkConfig.Version)
                         .Replace("#FUNITYVERSION#", Application.unityVersion)
                         .Replace("#FDATE#", DateTime.Now.ToString("yyyy-MM-dd"));
                File.WriteAllText(realPath, txt, Encoding.UTF8);
                EditorPrefs.SetBool(key, false);
            }
        }
        private static string newScriptName = "newScript.cs";
        private static string originScriptPath = FrameworkConfig.CorePath.CombinePath(@"ScriptCreater/Simple/Editor/CSharpScript.txt");

        [MenuItem("Assets/IFramework/CSharpScript")]
        public static void CreateUIBase()
        {
            CreateOriginIfNull();
            CopyAsset.CopyNewAsset(newScriptName, originScriptPath);
            EditorPrefs.SetBool(key, true);
        }

        private static  void CreateOriginIfNull()
        {
            if (File.Exists(originScriptPath)) return;
            using (FileStream fs = new FileStream(originScriptPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.Lock(0, fs.Length);
                    sw.WriteLine("/*********************************************************************************");
                    sw.WriteLine(" *Author:         #FAuthor#");
                    sw.WriteLine(" *Version:        #FVERSION#");
                    sw.WriteLine(" *UnityVersion:   #FUNITYVERSION#");
                    sw.WriteLine(" *Date:           #FDATE#");
                    sw.WriteLine(" *Description:    #FDescription#");
                    sw.WriteLine(" *History:        2018.11--");
                    sw.WriteLine("*********************************************************************************/");
                    sw.WriteLine("namespace #FNameSpace#");
                    sw.WriteLine("{");
                    sw.WriteLine("\tpublic class #FSCRIPTNAME#");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t");
                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                    fs.Unlock(0, fs.Length);
                    sw.Flush();
                    fs.Flush();
                }
            }
            AssetDatabase.Refresh();
        }
    }

}